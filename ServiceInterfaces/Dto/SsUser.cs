namespace ServiceInterfaces.Dto
{
    /// <summary>
    /// Used on the public interface, transient
    /// </summary>
    public class SsUser
    {
        public string UserName { get; }
        public string Password { get; }

        public UserRole Role { get; }

        public SsUser(string userName, string password, UserRole role = UserRole.Regular)
        {
            UserName = userName;
            Password = password;
            Role = role;
        }
    }
}