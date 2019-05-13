using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Entities;
using Vanrise.Security.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class AffectedUsersFilterInAccountManager : IUserFilter
    {
        public int? UserId { get; set; }
        public bool IsExcluded(User user)
        {
            if (UserId != null &&  user.UserId == UserId)
                return false;

            AccountManagerManager accountManagerManager = new AccountManagerManager();
            return accountManagerManager.IsUserAssignedToAccountManager(user.UserId) ;
        }
    }
}
