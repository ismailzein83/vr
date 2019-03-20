    using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Retail.BusinessEntity.MainExtensions.AccountViews
{
   public  class GenericBEAccountSubview : AccountViewDefinitionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("E3148386-1049-416A-876F-19C6C42B30A0"); }
        }

        public string BusinessEntityDefinitionId { get; set; }
        public string AccountIdMappingField { get; set; }
        public string AccountBEDefinitionMappingField { get; set; }
        public override string RuntimeEditor
        {
            get
            {
                return "retail-be-genericbeaccount-view";
            }
            set
            {

            }
        }

        public override bool DoesUserHaveAccess(IAccountViewDefinitionCheckAccessContext context)
        {
            return base.DoesUserHaveAccess(context);
        }
    }
}

