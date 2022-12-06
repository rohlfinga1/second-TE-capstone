using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;
using TenmoServer.DAO;

namespace TenmoServer.DAO
{
    public class TransferSqlDao : ITransferDao
    {
        private readonly string connectionString;
        
        public TransferSqlDao(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        //      I should be able to choose from a list of users to send TE Bucks to.
        public List<TransferRecipient> GetListOfUsers()
        {
            List<User> users = new List<User>();

            try // try reading from SQL all data where we have given uder id
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT username, user_id FROM tenmo_user", conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        User u = GetUserNamesFromReader(reader);
                        users.Add(u);
                    }
                    return users.Select(u => new TransferRecipient(u.UserId, u.Username)).ToList();//this lambda function takes each user and "massages" them into transfer recipients
                }
            }
            catch (SqlException)
            {
                throw;
            }


        }
        //      A transfer should include the User IDs of the from and to users and the amount of TE Bucks.

        // As an authenticated user of the system, I need to be able to see transfers I have sent or received.
        public List<TransferSent> GetTransfers(int userId)//changed from user to user id
        {
            List<TransferSent> transfers = new List<TransferSent>();

            try // try reading from SQL all data where we have given uder id
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT transfer_id, " +
                        "(SELECT transfer_status_desc FROM transfer_status WHERE transfer_status_id = 2) AS transfer_status, " +
                        "(SELECT transfer_type_desc FROM transfer_type WHERE transfer_type_id = 2) AS transfer_type, " +
                        "(SELECT username FROM tenmo_user WHERE user_id = " +
                            "(SELECT user_id FROM account WHERE account_id = account_from)) AS sender, " +
                        "(SELECT username FROM tenmo_user WHERE user_id = " +
                            "(SELECT user_id FROM account WHERE account_id = account_to)) AS recipient, " +
                        "amount FROM transfer " +
                    "WHERE" +
                        "(SELECT account_id FROM account WHERE user_id = @user_id) " +
                        "IN(account_from, account_to);", conn);

                    cmd.Parameters.AddWithValue("@user_id", userId);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        TransferSent t = GetTransferHistoryFromReader(reader);
                        transfers.Add(t);
                    }
                    if (transfers == null)
                    {
                        return null;
                    }
                    //return transfers.Select(t => new Transfer(t.TransferId, t.TransferStatusId, t.TransferTypeId, t.AccountFrom, t.AccountTo, t.Amount)).ToList();//this lambda function takes each user and "massages" them into transfer recipients
                }
            }
            catch (SqlException)
            {
                throw;
            } 
            return transfers;

        }
        public TransferSent GetPreviousTransfer(int userId, int transferId) 
        {
            TransferSent returnTransfer = null; // set up initial account

            try // try reading from SQL all data where we have given uder id
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT transfer_id, " +
                        "(SELECT username FROM tenmo_user WHERE user_id = " +
                            "(SELECT user_id FROM account WHERE account_id = account_from)) AS sender, " +
                        "(SELECT username FROM tenmo_user WHERE user_id = " +
                            "(SELECT user_id FROM account WHERE account_id = account_to)) AS recipient, " +
                        "amount FROM transfer " +
                    "WHERE" +
                        "(SELECT account_id FROM account WHERE user_id = @user_id) " +
                        "IN(account_from, account_to) AND transfer_id = @transfer_id;", conn);
                    cmd.Parameters.AddWithValue("@transfer_id", transferId);
                    cmd.Parameters.AddWithValue("@user_id", userId);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read()) // should only read 1 row of table
                    {
                        returnTransfer = GetTransferHistoryFromReader(reader);
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }
            return returnTransfer;
        }
        // As an authenticated user of the system, I need to be able to retrieve the details of any transfer based upon the transfer ID.
        public Transfer GetSpecificTransfer(int transferId)//removed user object param and if user is null
        {
            Transfer returnTransfer = null; // set up initial account
            
            try // try reading from SQL all data where we have given uder id
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT * FROM transfer WHERE transfer_id = @transfer_id;", conn);
                    cmd.Parameters.AddWithValue("@transfer_id", transferId);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read()) // should only read 1 row of table
                    {
                        returnTransfer = GetTransferFromReader(reader);
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }
            return returnTransfer;
        }
        // As an authenticated user of the system, I need to be able to send a transfer of a specific amount of TE Bucks to a registered user.
        public Transfer CreateSendingTransfer(Transfer transfer) //TODO
        {
            int newTransferId;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("INSERT INTO transfer (transfer_status_id, transfer_type_id, account_to, account_from, amount) " +
                                                "OUTPUT INSERTED.transfer_id " +
                                                "VALUES (@transfer_status_id, @transfer_type_id, @acccount_to, @account_from, @amount);", conn);
                cmd.Parameters.AddWithValue("@transfer_status_id", 2);
                cmd.Parameters.AddWithValue("@transfer_type_id", 2);
                cmd.Parameters.AddWithValue("@acccount_to", transfer.AccountTo);
                cmd.Parameters.AddWithValue("@account_from", transfer.AccountFrom);
                cmd.Parameters.AddWithValue("@amount", transfer.Amount);

                newTransferId = Convert.ToInt32(cmd.ExecuteScalar());
            }
            
            return GetSpecificTransfer(newTransferId);
        }

        //      A Sending Transfer has an initial status of Approved.

        //      The receiver's account balance is increased by the amount of the transfer.
        public bool UpdateBalanceForTransferAccounts(Transfer transfer) // possibly edit transfer
        {
            try // try reading from SQL all data where we have given uder id
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    
                    SqlCommand cmdUpdateSender = new SqlCommand("UPDATE  account SET balance = (SELECT (balance + @amount) FROM account WHERE account_id = @account_id) WHERE account_id = @account_id;", conn);
                    cmdUpdateSender.Parameters.AddWithValue("@amount", transfer.Amount * -1);
                    cmdUpdateSender.Parameters.AddWithValue("@account_id", transfer.AccountFrom);
                    cmdUpdateSender.ExecuteNonQuery();
                    
                    SqlCommand cmdUpdateRecipient = new SqlCommand("UPDATE  account SET balance = (SELECT (balance + @amount) FROM account WHERE account_id = @account_id) WHERE account_id = @account_id;", conn);
                    cmdUpdateRecipient.Parameters.AddWithValue("@amount", transfer.Amount);
                    cmdUpdateRecipient.Parameters.AddWithValue("@account_id", transfer.AccountTo);
                    cmdUpdateRecipient.ExecuteNonQuery();
                 
                    return true;
                }
            }
            catch (SqlException)
            {
                throw;
            }
                
            
        }
        //      The sender's account balance is decreased by the amount of the transfer.

        //      I must not be allowed to send money to myself.
        //      I can't send more TE Bucks than I have in my account.
        //      I can't send a zero or negative amount.

        //#8. Get Pending Transfers List
        public List<TransferRequest> GetPendingTransfers(int userId)//changed from user to user id
        {
            List<TransferRequest> transfers = new List<TransferRequest>();

            try // try reading from SQL all data where we have given uder id
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT transfer_id, " +
                        "(SELECT transfer_status_desc FROM transfer_status WHERE transfer_status_id = 1) AS transfer_status, " +
                        "(SELECT transfer_type_desc FROM transfer_type WHERE transfer_type_id = 1) AS transfer_type, " +
                        "(SELECT username FROM tenmo_user WHERE user_id = " +
                            "(SELECT user_id FROM account WHERE account_id = account_from)) AS sender, " +
                        "(SELECT username FROM tenmo_user WHERE user_id = " +
                            "(SELECT user_id FROM account WHERE account_id = account_to)) AS recipient, " +
                        "amount FROM transfer " +
                    "WHERE" +
                        "(SELECT account_id FROM account WHERE user_id = @user_id) " +
                        "IN(account_from, account_to);", conn);

                    cmd.Parameters.AddWithValue("@user_id", userId);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        TransferRequest t = GetPendingTransfersFromReader(reader);
                        transfers.Add(t);
                    }
                    if (transfers == null)
                    {
                        return null;
                    }
                    //return transfers.Select(t => new Transfer(t.TransferId, t.TransferStatusId, t.TransferTypeId, t.AccountFrom, t.AccountTo, t.Amount)).ToList();//this lambda function takes each user and "massages" them into transfer recipients
                }
            }
            catch (SqlException)
            {
                throw;
            }
            return transfers;

        }

        private Transfer GetTransferFromReader(SqlDataReader reader) // privately build POCO based on sql row
        {
            Transfer t = new Transfer()
            {
                TransferId = Convert.ToInt32(reader["transfer_id"]),
                TransferTypeId = Convert.ToInt32(reader["transfer_type_id"]),
                TransferStatusId = Convert.ToInt32(reader["transfer_status_id"]),
                AccountFrom = Convert.ToInt32(reader["account_from"]),
                AccountTo = Convert.ToInt32(reader["account_to"]),
                Amount = Convert.ToDecimal(reader["amount"])
            };

            return t;
        }


        private TransferSent GetTransferHistoryFromReader(SqlDataReader reader)
        {
            return new TransferSent()
            {
                TransferId = Convert.ToInt32(reader["transfer_id"]),
                TransferStatus = Convert.ToString(reader["transfer_status"]),
                TransferType = Convert.ToString(reader["transfer_type"]),
                Sender = Convert.ToString(reader["sender"]),
                Recipient = Convert.ToString(reader["recipient"]),
                Amount = Convert.ToDecimal(reader["amount"])
            };

        }

        private TransferRequest GetPendingTransfersFromReader(SqlDataReader reader)
        {
            return new TransferRequest()
            {
                TransferId = Convert.ToInt32(reader["transfer_id"]),
                TransferStatus = Convert.ToString(reader["transfer_status"]),
                TransferType = Convert.ToString(reader["transfer_type"]),
                Sender = Convert.ToString(reader["sender"]),
                Recipient = Convert.ToString(reader["recipient"]),
                Amount = Convert.ToDecimal(reader["amount"])
            };

        }

        private User GetUserNamesFromReader(SqlDataReader reader)
        {
            User u = new User()
            {
                UserId = Convert.ToInt32(reader["user_id"]),
                Username = Convert.ToString(reader["username"]),
            };

            return u;
        }

        private decimal GetBalanceFromReader(SqlDataReader reader) // privately build POCO based on sql row
        {
            return Convert.ToDecimal(reader["balance"]); // decimal balance
        }

        public List<Transfer> GetTransfers(User user)
        {
            throw new NotImplementedException();
        }

        //private ViewableTransfer GetViewableTransferFromReader(SqlDataReader reader) // privately build POCO based on sql row
        //{
        //    ViewableTransfer t = new ViewableTransfer()
        //    {
        //        TransferId = Convert.ToInt32(reader["transfer_id"]),
        //        TransferTypeId = Convert.ToInt32(reader["transfer_type_id"]),
        //        TransferStatusId = Convert.ToInt32(reader["transfer_status_id"]),
        //        Sender = Convert.ToString(reader["sender"]),
        //        Recipient = Convert.ToString(reader["recipient"]),
        //        Amount = Convert.ToDecimal(reader["amount"])
        //    };

        //    return t;
        //}

        // public List<Transfer> GetTransfersFor(userId, type)
        //{
        //    string sqlFilter = type == "Sent" ? "account_from" : "account_to";
        //    //continue previous query
        //    return 
        //}

        // public List<Transfer> GetSentTransfersFor(userId)
        //{
        //    return GetTransfersFor(userId, 'Sent');
        //}

        // public List<Transfer> GetRecdTransfersFor(userId)
        //{
        //    return GetTransfersFor(userId, "Recd")
        //}
    }
}
