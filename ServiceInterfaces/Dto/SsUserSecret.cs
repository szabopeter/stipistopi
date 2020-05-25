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
        // TODO Make these read-only again
        public string UserName { get; set; }
        public string Salt { get; set; }
        public string PasswordHash { get; set; }
        public UserRole Role { get; set; }

        [Obsolete("Only for LiteDB")]
        public SsUserSecret()
        {
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            
            return ((SsUserSecret)obj).UserName.Equals(UserName);
        }
        
        public override int GetHashCode()
        {
            return UserName.GetHashCode();
        }

        public SsUserSecret(SsUser ssUser)
        {
            Contract.Requires(ssUser != null);

            UserName = NormalizeUserName(ssUser.UserName);
            Role = ssUser.Role;
            Salt = Guid.NewGuid().ToString();
            PasswordHash = HashPassword(ssUser.Password, Salt);
        }

        public bool IsValid(SsUser user)
        {
            Contract.Requires(user != null);

            if (!UserName.Equals(user.UserName))
                return false;

            var hash = HashPassword(user.Password, Salt);
            return hash == PasswordHash;
        }

        // [SuppressMessage("Microsoft.Design", "CA1822")]
        public string HashPassword(string password, string salt)
        {
            var combined = password + salt;
            return combined.GetHashCode(StringComparison.InvariantCulture).ToString(CultureInfo.InvariantCulture);
        }

        public static string NormalizeUserName(string userName)
        {
            return userName.ToLowerInvariant();
        }
    }
}