using System;

namespace logic
{
    /// <summary>
    /// Used internally, persistent
    /// </summary>
    public class SsUserSecret
    {
        public string UserName { get;}
        public string Salt { get; }
        public string PasswordHash { get;}

        public SsUserSecret(SsUser ssUser)
        {
            UserName = ssUser.UserName;
            Salt = Guid.NewGuid().ToString();
            PasswordHash = HashPassword(ssUser.Password, Salt);
        }

        public bool IsValid(SsUser user)
        {
            if (!HasName(user.UserName))
                return false;

            var hash = HashPassword(user.Password, Salt);
            return hash == PasswordHash;
        }

        public string HashPassword(string password, string salt)
        {
            var combined = password + salt;
            return combined.GetHashCode().ToString();
        }

        public bool HasName(string userName)
        {
            return string.Equals(userName, UserName, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}