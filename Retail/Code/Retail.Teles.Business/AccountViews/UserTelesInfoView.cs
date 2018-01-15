using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Teles.Business
{
    public class UserTelesInfoView : AccountViewDefinitionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("7FC4E765-6E0C-4FD9-94F7-35CD4E7F98BF"); }
        }

        public override string RuntimeEditor
        {
            get
            {
                return "retail-teles-usertelesinfo-view";
            }
            set
            {

            }
        }

        public override bool DoesUserHaveAccess(IAccountViewDefinitionCheckAccessContext context)
        {
            var securityManager = new Vanrise.Security.Business.SecurityManager();
            return securityManager.HasPermissionToActions("Retail_Teles/TelesUser/GetUserTelesInfo", context.UserId);
        }
        public Guid VRConnectionId { get; set; }
    }
}


