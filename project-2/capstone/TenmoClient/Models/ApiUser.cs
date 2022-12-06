namespace TenmoClient.Models
{
    /// <summary>
    /// Return value from login endpoint
    /// </summary>
    public class ApiUser
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Token { get; set; }
        public string Message { get; set; }

        public ApiUser() { }

        public ApiUser(int userId, string username, string token)
        {
            this.UserId = userId;
            this.Username = username;
            this.Token = token;
        }
        public ApiUser(int userId, string username, string token, string message)
        {
            this.UserId = userId;
            this.Username = username;
            this.Token = token;
            this.Message = message;
        }
    }

    


}
