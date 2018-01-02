using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace Vanrise.CaseManagement.Business
{
    public class VRCaseViewSettings : ViewSettings
    {
        public List<CaseManagementViewSettingItem> Settings { get; set; }
        public override string GetURL(View view)
        {
            return String.Format("#/viewwithparams/VR_CaseManagement/Elements/CaseManagement/Views/VRCaseManagement/{{\"viewId\":\"{0}\"}}", view.ViewId);
        }
        public override bool DoesUserHaveAccess(IViewUserAccessContext context)
        {
          //  return new CaseManagementDefinitionManager().DoesUserHaveViewAccess(context.UserId, this.InvoiceTypeId);
            return true;
        }
    }
    public class CaseManagementViewSettingItem
    {
        public Guid VRCaseDefinitionId { get; set; }
    }
}
