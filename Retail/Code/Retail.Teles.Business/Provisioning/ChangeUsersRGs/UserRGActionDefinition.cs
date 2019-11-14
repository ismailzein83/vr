using Retail.BusinessEntity.Entities;
using Retail.Teles.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Notification.Entities;

namespace Retail.Teles.Business
{
    public class UserRGActionDefinition : VRActionDefinitionExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("BDDFED8C-4D51-4D53-8D71-C9399CA91517"); }
        }
        public override string RuntimeEditor { get { return "retail-teles-provisioner-action-blockinternationalcalls"; } }

        public Guid VRConnectionId { get; set; }
        public Guid CompanyTypeId { get; set; }
        public Guid SiteTypeId { get; set; }
        public Guid? UserTypeId { get; set; }
        public bool SaveChangesToAccountState { get; set; }

        /// <summary>
        /// only applicable if SaveChangesToAccountState is true
        /// </summary>
        public string ActionType { get; set; }
        public NewRGNoMatchHandling NewRGNoMatchHandling { get; set; }
        public NewRGMultiMatchHandling NewRGMultiMatchHandling { get; set; }
        public RoutingGroupCondition NewRoutingGroupCondition { get; set; }
        public ExistingRGNoMatchHandling ExistingRGNoMatchHandling { get; set; }
        public RoutingGroupCondition ExistingRoutingGroupCondition { get; set; }

        public override bool IsApplicable(IVRActionDefinitionIsApplicableContext context)
        {
            return true;
        }
    }
}
