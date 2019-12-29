namespace AuthenticationDemo.Services
{
    /// <summary>
    /// DTO Objekt für den HTTP Request Body.
    /// </summary>
    public class UserCredentials
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
    }
}
