using System;
using System.Diagnostics.Contracts;
using System.Security.Cryptography;
using System.Text;

namespace ServiceInterfaces.Dto
{
    /// <summary>
    /// Used internally, persistent
    /// </summary>
    public class SsUserSecret
    {
        public string UserName { get; set; }
        public string Salt { get; set; }
        public string PasswordHash { get; set; }
        public UserRole Role { get; set; }

        /// <summary>
        /// Only for serialization
        /// </summary>
        public SsUserSecret()
        {
        }

        public override bool Equals(object obj)
        {
            if (!(obj is SsUserSecret other) || GetType() != other.GetType())
            {
                return false;
            }

            return string.Equals(other.UserName, UserName, StringComparison.InvariantCulture);
        }

        public override int GetHashCode()
        {
            return string.GetHashCode(UserName, StringComparison.InvariantCulture);
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

            if (!string.Equals(UserName, user.UserName, StringComparison.InvariantCulture))
                return false;

            var hash = HashPassword(user.Password, Salt);
            return hash == PasswordHash;
        }

        // TODO Extract hashing to another class
        public static string HashPassword(string password, string salt)
        {
            var combined = password + salt;
            var bytes = Encoding.UTF8.GetBytes(combined);
            using var sha256 = new SHA256Managed();
            var hashBytes = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hashBytes);
        }

        public static string NormalizeUserName(string userName)
        {
            Contract.Requires(userName != null);
            return userName.ToUpperInvariant();
        }
    }
}