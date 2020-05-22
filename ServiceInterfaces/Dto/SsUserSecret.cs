using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;

namespace ServiceInterfaces.Dto
{
    /// <summary>
    /// Used internally, persistent
    /// </summary>
    public class SsUserSecret
    {
        public string UserName { get; }
        public string Salt { get; }
        public string PasswordHash { get; }

        public SsUserSecret(SsUser ssUser)
        {
            Contract.Requires(ssUser != null);

            UserName = ssUser.UserName;
            Salt = Guid.NewGuid().ToString();
            PasswordHash = HashPassword(ssUser.Password, Salt);
        }

        public bool IsValid(SsUser user)
        {
            Contract.Requires(user != null);

            if (!HasName(user.UserName))
                return false;

            var hash = HashPassword(user.Password, Salt);
            return hash == PasswordHash;
        }

        [SuppressMessage("Microsoft.Design", "CA1822")]
        public string HashPassword(string password, string salt)
        {
            var combined = password + salt;
            return combined.GetHashCode(StringComparison.InvariantCulture).ToString(CultureInfo.InvariantCulture);
        }

        public bool HasName(string userName)
        {
            return string.Equals(userName, UserName, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}