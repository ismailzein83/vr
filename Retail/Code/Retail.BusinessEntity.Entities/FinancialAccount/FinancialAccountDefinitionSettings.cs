using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class FinancialAccountDefinitionSettings : Vanrise.Entities.VRComponentTypeSettings
    {
        public override Guid VRComponentTypeConfigId
        {
            get { throw new NotImplementedException(); }
        }

        public Guid? BalanceAccountTypeId { get; set; }

        public Guid? InvoiceTypeId { get; set; }

        public FinancialAccountDefinitionExtendedSettings ExtendedSettings { get; set; }
    }

    public abstract class FinancialAccountDefinitionExtendedSettings
    {
        public abstract Guid ConfigId { get; }
    }
}
