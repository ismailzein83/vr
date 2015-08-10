using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Security.Business;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Web.Base;

namespace  Vanrise.Fzero.FraudAnalysis.Web.Controllers
{

    public class UserController : BaseAPIController
    {

        [HttpGet]

        public IEnumerable<User> GetUsers()
        {
            UserManager manager = new UserManager();

            return UserMapper(manager.GetUsers());
        }


        private List<User> UserMapper(List<Vanrise.Security.Entities.User> listUsers)
        {
            List<User> CDRUsers = new List<User>();

            foreach ( var user in listUsers)
            {
                CDRUsers.Add(new User() { Name = user.Name, UserId = user.UserId });
            }
            return CDRUsers;
        }

    }
}