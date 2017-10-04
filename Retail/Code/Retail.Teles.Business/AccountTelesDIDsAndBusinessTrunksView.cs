using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Teles.Business
{
    public class AccountTelesDIDsAndBusinessTrunksView : AccountViewDefinitionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("F11DB886-8893-441F-B5A4-3261D43E8C0F"); }
        }

        public override string RuntimeEditor
        {
            get
            {
                return "retail-teles-accounttelesdidsandbusinesstrunks-view";
            }
            set
            {

            }
        }

        public override bool DoesUserHaveAccess(IAccountViewDefinitionCheckAccessContext context)
        {
            var securityManager = new Vanrise.Security.Business.SecurityManager();
           return securityManager.HasPermissionToActions("Retail_Teles/TelesEnterprise/GetFilteredEnterpriseDIDs&Retail_Teles/TelesEnterprise/GetFilteredEnterpriseBusinessTrunks", context.UserId);
        }
        public Guid VRConnectionId { get; set; }
    }
}


