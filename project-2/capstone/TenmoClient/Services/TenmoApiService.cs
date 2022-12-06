using RestSharp;
using System.Collections.Generic;
using TenmoClient.Models;

namespace TenmoClient.Services
{
    public class TenmoApiService : AuthenticatedApiService
    {
        public readonly string ApiUrl;

        public TenmoApiService(string apiUrl) : base(apiUrl) { }

        public Account GetAccount(ApiUser user)
        {
            RestRequest request = new RestRequest($"/accounts/{user.UserId}");
            IRestResponse<Account> response = client.Get<Account>(request);

            CheckForError(response);
            return response.Data;
            // TODO:  unable to reach server to test if this method actually works
        }

        public List<TransferSent> GetPastTransfers(int userId)
        {
            RestRequest request = new RestRequest($"transfers/list/{userId}");
            IRestResponse<List<TransferSent>> response = client.Get<List<TransferSent>>(request);

            CheckForError(response);
            return response.Data;

        }

        public TransferSent ViewPreviousTransfer(ApiUser user, int transferId)
        {
            RestRequest request = new RestRequest($"/transfers/{user.UserId}/{transferId}");
            IRestResponse<TransferSent> response = client.Get<TransferSent>(request);

            CheckForError(response);
            return response.Data;
        }
        public Transfer ViewSpecificTransfer(ApiUser user, int transferId)
        {
            RestRequest request = new RestRequest($"/transfers/{transferId}");
            IRestResponse<Transfer> response = client.Get<Transfer>(request);

            CheckForError(response);
            return response.Data;
        }

        public Transfer CreateSendingTransfer(int fromUser, int toUser, decimal amt)//instead of passing in a new transfer bc datatbase crsates the transfer 
        {
            RestRequest req = new RestRequest("transfers");
            req.AddJsonBody(new Transfer { AccountFrom = fromUser, AccountTo = toUser, Amount = amt });
            IRestResponse<Transfer> response = client.Post<Transfer>(req);
            CheckForError(response);
            return response.Data;
        }
        public List<ApiUser> GetListOfUsers()
        {
            RestRequest req = new RestRequest("transfers/userList");
            IRestResponse<List<ApiUser>> response = client.Get<List<ApiUser>>(req);
            CheckForError(response);
            return response.Data;

        }

        public Transfer UpdateBalanceForTransferAccounts(Transfer updatingTransfer)
        {
            RestRequest req = new RestRequest($"transfers/{updatingTransfer.TransferId}");
            req.AddJsonBody(updatingTransfer);
            IRestResponse<Transfer> response = client.Put<Transfer>(req);
            CheckForError(response);
            return response.Data;

        }

        //#8. Get Pending Transfers
        public List<TransferRequest> GetPendingTransfers(int userId)
        {
            RestRequest request = new RestRequest($"transfers/list/pending/{userId}");
            IRestResponse<List<TransferRequest>> response = client.Get<List<TransferRequest>>(request);

            CheckForError(response);
            return response.Data;
        }

    }
}
