using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TenmoServer.Models
{
    public class Account
    {
        [Required(ErrorMessage = "The field \'Account ID\' is required.")]
        public int AccountId { get; set; } // properties from account table
        [Required(ErrorMessage = "The field \'User ID\' is required.")]
        public int UserId { get; set; }
        [Range(0.00, 1000000000000, ErrorMessage = "The field \' balance\' cannot be negative.")]
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
