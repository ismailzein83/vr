using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.AccountBEActionTypes
{
    public class ChangeStatusActionSettings : AccountActionDefinitionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("FA494842-57C6-4972-A85F-3D9AA11C695D"); }
        }

        public override string ClientActionName
        {
            get { return "ChangeStatusAction"; }
        }
        public Guid StatusId { get; set; }
        public bool ApplyToChildren { get; set; }
        public bool AllowOverlapping { get; set; }
        public List<Guid> ApplicableOnStatuses { get; set; }
        public override bool DoesUserHaveAccess(IAccountActionDefinitionCheckAccessContext context)
        {
            return new AccountBEDefinitionManager().DoesUserHaveEditAccess(context.UserId, context.AccountBEDefinitionId);
        }
    }
}
