using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.ProductTypes.PostPaid
{
    public class PostPaidDefinitionSettings : ProductDefinitionExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("AC102D41-B0DB-4E02-A26B-DB8D6BFE47F3"); }
        }
    }
}
