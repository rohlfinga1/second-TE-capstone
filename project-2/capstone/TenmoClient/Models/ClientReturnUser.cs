using System;
using System.Collections.Generic;
using System.Text;

namespace TenmoClient.Models
{
    public class ClientReturnUser
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Token { get; set; }

        public ClientReturnUser() { }

        public ClientReturnUser(int userId, string username, string token)
        {
            this.UserId = userId;
            this.Username = username;
            this.Token = token;
        }
    }
}
