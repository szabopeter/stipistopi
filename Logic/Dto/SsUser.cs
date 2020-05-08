namespace Logic.Dto
{
    /// <summary>
    /// Used on the public interface, transient
    /// </summary>
    public class SsUser
    {
        public string UserName { get; }
        public string Password { get; }

        public SsUser(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }
    }
}