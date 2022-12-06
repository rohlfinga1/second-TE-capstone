using System;
using System.Collections.Generic;
using TenmoClient.Models;
using System.Linq;

namespace TenmoClient.Services
{
    public class TenmoConsoleService : ConsoleService
    {
        /************************************************************
            Print methods
        ************************************************************/
        public void PrintLoginMenu()
        {
            Console.Clear();
            Console.WriteLine("");
            Console.WriteLine("Welcome to TEnmo!");
            Console.WriteLine("1: Login");
            Console.WriteLine("2: Register");
            Console.WriteLine("0: Exit");
            Console.WriteLine("---------");
        }

        public void PrintMainMenu(string username)
        {
            Console.Clear();
            Console.WriteLine("");
            Console.WriteLine($"Hello, {username}!");
            Console.WriteLine("1: View your current balance");
            Console.WriteLine("2: View your past transfers");
            Console.WriteLine("3: View your pending requests");
            Console.WriteLine("4: Send TE bucks");
            Console.WriteLine("5: Request TE bucks");
            Console.WriteLine("6: Log out");
            Console.WriteLine("0: Exit");
            Console.WriteLine("---------");
        }
        public LoginUser PromptForLogin()
        {
            string username = PromptForString("User name");
            if (String.IsNullOrWhiteSpace(username))
            {
                return null;
            }
            string password = PromptForHiddenString("Password");

            LoginUser loginUser = new LoginUser
            {
                Username = username,
                Password = password
            };
            return loginUser;
        }

        // Add application-specific UI methods here...

        public void GetBalance(decimal balance)
        {
            Console.WriteLine($"Your current account balance is: ${balance}");
        }

        public void ViewPastTransfers(List<TransferSent> transfers, string username)
        {
            Console.WriteLine("-------------------------------------------");
            Console.WriteLine("Transfers");
            Console.WriteLine("ID           From/ To                 Amount");
            Console.WriteLine("------------------------------------------");

            foreach (TransferSent transfer in transfers) // Get list of users to call for the username
            {
                if (transfer.TransferStatus == "Approved")
                {
                    string fromOrTo = "";
                    if (transfer.Sender == username)
                    {
                        fromOrTo = $"To: {transfer.Recipient}";//who we sent it to
                    }
                    else
                    {
                        fromOrTo = $"From: {transfer.Sender}";//who its from
                    }
                    Console.WriteLine($"{transfer.TransferId}          {fromOrTo}          ${transfer.Amount}");
                }
            }
            Console.WriteLine("-----------------------------------------");
        }

        public void SendBucks(List<Transfer> transfers, int transferId, ApiUser user, decimal amount) // print
        {

            Console.WriteLine("");
            Console.WriteLine("| --------------Users-------------- |");
            Console.WriteLine("|    Id | Username                  |");
            Console.WriteLine("| -------+---------------------------|");
            foreach (Transfer fransfer in transfers)
                {
                Console.WriteLine($"|  {transferId} | {user.Username}  |");
                }   
            Console.WriteLine("| -----------------------------------|");
            Console.WriteLine($"Sending TE Bucks to {user.Username}");
            Console.WriteLine($"Amount sending: {amount}");
        }

        internal int PromptForTransferToUser(List<ApiUser> users)
        {
            Console.WriteLine("");
            Console.WriteLine("| --------------Users-------------- |");
            Console.WriteLine("|    Id | Username                  |");
            Console.WriteLine("| -------+---------------------------|");
            foreach (ApiUser u in users)
            {
                Console.WriteLine($"| {u.UserId} | {u.Username} |");
            }
            Console.WriteLine("| -------+---------------------------|");
            bool isValdUserId = false;
            int targetUserId = 0;
            while (!isValdUserId)
            {
                targetUserId = PromptForInteger("Id of the user you are sending to", 0);
                if (users.Select(u => u.UserId).Contains(targetUserId))//
                {
                    isValdUserId = true;
                }
                else
                { Console.WriteLine("Please try again."); }
            }


            return targetUserId;
        }

        internal decimal PromptForTransferAmount(decimal sendersBalance)
        {
            bool isValidAmount = false;
            decimal amountToSend = 0;
            while(!isValidAmount)
            {
                amountToSend = PromptForDecimal("Enter amount to send");
                if (amountToSend <= sendersBalance)
                {
                    isValidAmount = true;
                }
                else
                {
                    Console.WriteLine("please enter valid amount");
                }
            }
            return amountToSend;
        }

        //internal // output: transfer ids, other usernames, amounts 
        // input list transfers & list of users, account list?

        public void ViewSpecificTransfer(TransferSent transfer)
        {
            Console.WriteLine("--------------------------------------------");
            Console.WriteLine("Transfer Details");
            Console.WriteLine("--------------------------------------------");
            Console.WriteLine($"Id: {transfer.TransferId}");
            Console.WriteLine($"From: {transfer.Sender}"); // username
            Console.WriteLine($"To: {transfer.Recipient}");
            Console.WriteLine($"Type: Send"); // read from sql?
            Console.WriteLine($"Status: Approved");
            Console.WriteLine($"Amount: {transfer.Amount}");
        }


        // #8. View Pending Transfers
        public void ViewPendingTransfers(List<TransferRequest> transfers, string username)
        {
            Console.WriteLine("-------------------------------------------");
            Console.WriteLine("Transfers");
            Console.WriteLine("ID           From/ To                 Amount");
            Console.WriteLine("------------------------------------------");
            
            foreach (TransferRequest transfer in transfers) // Get list of users to call for the username
            {
                if (transfer.TransferStatus == "Pending")
                {
                    string fromOrTo = "";
                    if (transfer.Sender == username)
                    {
                        fromOrTo = $"To: {transfer.Recipient}";//who we sent it to
                    }
                    else
                    {
                        fromOrTo = $"From: {transfer.Sender}";//who its from
                    }
                    Console.WriteLine($"{transfer.TransferId}          {fromOrTo}          ${transfer.Amount}");
                }
                
            }
            Console.WriteLine("-----------------------------------------");
        }
    }
}

