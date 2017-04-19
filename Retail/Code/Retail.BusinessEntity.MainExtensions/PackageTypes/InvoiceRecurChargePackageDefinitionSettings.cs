using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.PackageTypes
{
    public class InvoiceRecurChargePackageDefinitionSettings : PackageDefinitionExtendedSettings
    {
        public override Guid ConfigId
        {
            get { throw new NotImplementedException(); }
        }

        public override string RuntimeEditor
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Guid ChargeableEntityBEDefinitionId { get; set; }

        public Guid ChargeableEntityId { get; set; }
    }
}
