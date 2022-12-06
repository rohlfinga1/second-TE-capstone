using System;
using System.Collections.Generic;
using TenmoClient.Models;
using TenmoClient.Services;
using System.Linq;


namespace TenmoClient
{
    public class TenmoApp
    {
        private readonly TenmoConsoleService console = new TenmoConsoleService();
        private readonly TenmoApiService tenmoApiService;
        private ApiUser user = null;
        

        public TenmoApp(string apiUrl)
        {
            tenmoApiService = new TenmoApiService(apiUrl);
        }

        public void Run()
        {
            bool keepGoing = true;
            while (keepGoing)
            {
                // The menu changes depending on whether the user is logged in or not
                if (tenmoApiService.IsLoggedIn)
                {
                    keepGoing = RunAuthenticated();
                }
                else // User is not yet logged in
                {
                    keepGoing = RunUnauthenticated();
                }
            }
        }

        private bool RunUnauthenticated()
        {
            console.PrintLoginMenu();
            int menuSelection = console.PromptForInteger("Please choose an option", 0, 2, 1);
            while (true)
            {
                if (menuSelection == 0)
                {
                    return false;   // Exit the main menu loop
                }

                if (menuSelection == 1)
                {
                    // Log in
                    Login();
                    return true;    // Keep the main menu loop going
                }

                if (menuSelection == 2)
                {
                    // Register a new user
                    Register();
                    return true;    // Keep the main menu loop going
                }
                console.PrintError("Invalid selection. Please choose an option.");
                console.Pause();
            }
        }

        private bool RunAuthenticated()
        {
            console.PrintMainMenu(tenmoApiService.Username);
            int menuSelection = console.PromptForInteger("Please choose an option", 0, 6);
            if (menuSelection == 0)
            {
                // Exit the loop
                return false;
            }

            if (menuSelection == 1)
            {
                // View your current balance
                // GetBalance method, call it, display console.writeline, make sure the user still has access to the menu 
                GetBalance();
            }

            if (menuSelection == 2)
            {
                //View your past transfers
                DisplayPastTransfers();
            }

            if (menuSelection == 3)
            {
                // View your pending requests
                DisplayPendingTransfers();
            }

            if (menuSelection == 4)
            {
                // Send TE bucks
                SendBucks();
            }

            if (menuSelection == 5)
            {
                // Request TE bucks
            }

            if (menuSelection == 6)
            {
                // Log out
                tenmoApiService.Logout();
                console.PrintSuccess("You are now logged out");
            }

            return true;    // Keep the main menu loop going
        }

        private void GetBalance()
        {
            Account account = tenmoApiService.GetAccount(user);
            console.GetBalance(account.Balance);
            console.Pause();
            //Console.WriteLine(balance);
        }

        private void DisplayPastTransfers()
        {
            List<TransferSent> transfers = tenmoApiService.GetPastTransfers(user.UserId);
            console.ViewPastTransfers(transfers, user.Username);
            // would you like to view specific transfer? 
            bool willViewTransfer = false;
            while (!willViewTransfer)
            {
                int transferId = console.PromptForInteger("Please enter transfer ID to view details(0 to cancel)");
                if (transferId == 0)
                {
                    break;
                }
                for(int i = 0; i < transfers.Count; i++)
                {
                    if (transfers[i].TransferId == transferId)
                    {
                        ViewSpecificTransfer(transferId);
                        willViewTransfer = true;
                    }
                }
            }
            console.Pause();
            // readline?
            // tenmoapiservice view specific transfer
        }

        private void SendBucks()
        {
            List<ApiUser> users = tenmoApiService.GetListOfUsers();//thank you david for the lambda
            int transferToUserId = console.PromptForTransferToUser(users.Where(u => u.UserId != user.UserId).ToList());//this is filtering the current user from list
            Account toAccount = tenmoApiService.GetAccount(new ApiUser { UserId = transferToUserId });
            //TODO: filter on database side
            Account fromAccount = tenmoApiService.GetAccount(user);
            decimal amount = console.PromptForTransferAmount(fromAccount.Balance);
            tenmoApiService.CreateSendingTransfer(fromAccount.AccountId, toAccount.AccountId, amount);
            console.PrintSuccess("Transfer successful.");
            console.Pause();
        }

        private void ViewSpecificTransfer(int transferId) //TODO
        {
            //int transferId = console.PromptForInteger("");
            TransferSent transfer = tenmoApiService.ViewPreviousTransfer(user, transferId);
            console.ViewSpecificTransfer(transfer);
            //console.Pause();
        }


        // #8. View Pending Transfers
        private void DisplayPendingTransfers()
        {
            List<TransferRequest> transfers = tenmoApiService.GetPendingTransfers(user.UserId);
            console.ViewPendingTransfers(transfers, user.Username);
            // would you like to view specific transfer? 
            bool willViewTransfer = false;
            while (!willViewTransfer)
            {
                int transferId = console.PromptForInteger("Please enter transfer ID to view details(0 to cancel)");
                if (transferId == 0)
                {
                    break;
                }
                for (int i = 0; i < transfers.Count; i++)
                {
                    if (transfers[i].TransferId == transferId)
                    {
                        //ViewSpecificTransfer(transferId); #9
                        willViewTransfer = true;
                    }
                }
            }
            console.Pause();
            // readline?
            // tenmoapiservice view specific transfer
            //TODO: CURRENTLY, this function shows previous transfers and transfers created now.
            // Essentially, this programm does not differentiate transfers. Code needs clarification!!!!
        }

        private void Login()
        {
            LoginUser loginUser = console.PromptForLogin();
            if (loginUser == null)
            {
                return;
            }

            try
            {
                ApiUser user = tenmoApiService.Login(loginUser);
                if (user == null)
                {
                    console.PrintError("Login failed.");
                }
                else
                {
                    console.PrintSuccess("You are now logged in");
                    this.user = user;
                }
            }
            catch (Exception)
            {
                console.PrintError("Login failed.");
            }
            console.Pause();
        }

        private void Register()
        {
            LoginUser registerUser = console.PromptForLogin();
            if (registerUser == null)
            {
                return;
            }
            try
            {
                bool isRegistered = tenmoApiService.Register(registerUser);
                if (isRegistered)
                {
                    console.PrintSuccess("Registration was successful. Please log in.");
                }
                else
                {
                    console.PrintError("Registration was unsuccessful.");
                }
            }
            catch (Exception)
            {
                console.PrintError("Registration was unsuccessful.");
            }
            console.Pause();


        }
    }
}
