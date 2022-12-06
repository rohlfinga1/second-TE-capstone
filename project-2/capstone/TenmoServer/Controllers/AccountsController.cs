using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;
using TenmoServer.DAO;

namespace TenmoServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountDao accountDao; // have private variable to represent dao object

        public AccountsController(IAccountDao accountDao)
        {
            this.accountDao = accountDao;
        }

        // GET: AccountController
        [HttpGet("{userId}")]
        public ActionResult<Account> GetBalance(int userId)
        {
            Account specificAccount = accountDao.GetAccount(userId); // TODO: get balance for specific id, do we need to use token from return-user model to find user id?
            if (specificAccount != null)
            {
                return specificAccount;
            }
            else
            {
                return NotFound(); // don't give invalid user
            }
            //also should not let user view any balance but their own
        }
    }
}
