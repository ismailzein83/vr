using Retail.BusinessEntity.Business;
using Retail.Teles.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
namespace Retail.Teles.Business
{
    public class UsersNotMappedToAccountFilter : ITelesUserFilter
    {
        public string EditedUserId { get; set; }
        public long AccountId { get; set; }
        public bool IsExcluded(ITelesUserFilterContext context)
        {
            TelesUserManager telesUserManager = new TelesUserManager();
            var cachedAccountsByUsers = telesUserManager.GetCachedAccountsByUsers(context.AccountBEDefinitionId);
            if (this.EditedUserId != null && EditedUserId == context.UserId)
                return false;
            if (cachedAccountsByUsers != null && cachedAccountsByUsers.ContainsKey(context.UserId))
                return true;
            return false;
        }
    }
}
