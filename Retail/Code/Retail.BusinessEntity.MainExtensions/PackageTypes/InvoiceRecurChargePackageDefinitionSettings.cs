using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Retail.BusinessEntity.MainExtensions.PackageTypes
{
    
    public class InvoiceRecurChargePackageDefinitionSettings : PackageDefinitionExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("E326482A-9AB5-4715-848F-11CAF4940040"); }
        }

        public override string RuntimeEditor
        {
            get
            {
                return "retail-be-packagesettings-extendedsettings-invoicerecurcharge";
            }
        }

        public Guid ChargeableEntityBEDefinitionId { get; set; }

        public Guid ChargeableEntityId { get; set; }

        public RecurringChargeEvaluatorDefinitionSettings EvaluatorDefinitionSettings { get; set; }
    }
}
