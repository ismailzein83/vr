using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Retail.BusinessEntity.MainExtensions.VRObjectTypes
{
    public class AccountCreditLimitObjectType : VRObjectType
    {
        public override Guid ConfigId { get { return new Guid("2876106F-53E6-438F-8999-B88F479164C4"); } }
        public Guid AccountBEDefinitionId { get; set; }
        public override object CreateObject(IVRObjectTypeCreateObjectContext context)
        {
            return null;
        }
    }
}
