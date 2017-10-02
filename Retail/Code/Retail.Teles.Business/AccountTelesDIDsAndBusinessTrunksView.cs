    using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
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
                return "retail-teles-accountTelesdidsandbusinesstrunks-view";
            }
            set
            {

            }
        }

        public override bool DoesUserHaveAccess(IAccountViewDefinitionCheckAccessContext context)
        {
            return base.DoesUserHaveAccess(context);
        }
        public Guid VRConnectionId { get; set; }
    }
}


