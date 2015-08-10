using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;
using Vanrise.Web.Base;

namespace  Vanrise.Fzero.FraudAnalysis.Web.Controllers
{

    public class UserController : BaseAPIController
    {

        [HttpGet]

        public IEnumerable<User> GetUsers()
        {
            UserManager manager = new UserManager();

            return manager.GetUsers();
        }

    }
}