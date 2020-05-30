namespace ServiceInterfaces.Dto
{
    /// <summary>
    /// Used on the public interface, transient
    /// </summary>
    public class SsUser
    {
        public string UserName
        {
            get => userName;
            set => userName = SsUserSecret.NormalizeUserName(value);
        }
        private string userName;

        public string Password { get; set; }

        public UserRole Role { get; set; }

        /// <summary>For serialization</summary>
        public SsUser()
        {
        }

        public SsUser(string userName, string password, UserRole role = UserRole.Regular)
        {
            UserName = userName;
            Password = password;
            Role = role;
        }
    }
}