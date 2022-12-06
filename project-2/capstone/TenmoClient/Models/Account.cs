using System;
using System.Collections.Generic;
using System.Text;

namespace TenmoClient.Models
{
        public class Account
        {
            public int AccountId { get; set; } // properties from account table
            public int UserId { get; set; }
            public decimal Balance { get; set; }

            public Account() { }

            public Account(int accountId, int userId, decimal balance)
            {
                this.AccountId = accountId;
                this.UserId = userId;
                this.Balance = balance;
            }
        
        }
    
}
