using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace Vanrise.AccountManager.Business
{
    public class AssignedUsersToAccountManagerFilter : IUserFilter
    {
        public int? EditedUserId { get; set; }
        public bool IsExcluded(User user)
        {
            if (EditedUserId != null && EditedUserId == user.UserId)
                return false;
            AccountManagerManager accountManagerManager = new AccountManagerManager();
            if (accountManagerManager.AreUsersAssignableToAccountManager(user.UserId))
                return false;
            return true;
        }
    }
}
