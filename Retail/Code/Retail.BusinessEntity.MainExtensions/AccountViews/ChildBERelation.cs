using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;

namespace Retail.BusinessEntity.MainExtensions.AccountViews
{
    public class ChildBERelation : AccountViewDefinitionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("871CEED6-F7E0-4D4F-9A30-8F2869B6E0EE"); }
        }

        public override string RuntimeEditor
        {
            get
            {
                return "retail-be-childberelations-view";
            }
        }

        public Guid BEParentChildRelationDefinitionId { get; set; }

        public override bool DoesUserHaveAccess(IAccountViewDefinitionCheckAccessContext context)
        {
            BEParentChildRelationDefinitionManager beParentChildManager = new BEParentChildRelationDefinitionManager();
            return beParentChildManager.DoesUserHaveViewAccess(this.BEParentChildRelationDefinitionId);
        }
    }
}
