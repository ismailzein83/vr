using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class FinancialAccountDefinitionSettings : Vanrise.Entities.VRComponentTypeSettings
    {
        public override Guid VRComponentTypeConfigId { get { return new Guid("3ED8A0C3-99E7-486A-A560-5789BA1DEAEE"); } }

        public Guid AccountBEDefinitionId { get; set; }
        public Guid? BalanceAccountTypeId { get; set; }

        public Guid? InvoiceTypeId { get; set; }
        public List<FinancialAccountInvoiceType> InvoiceTypes { get; set; }
        public List<string> ApplicableClassifications { get; set; }
        public FinancialAccountDefinitionExtendedSettings ExtendedSettings { get; set; }
    }
    public class FinancialAccountInvoiceType
    {
        public Guid InvoiceTypeId { get; set; }
        public List<string> ApplicableClassifications { get; set; }
    }
    public abstract class FinancialAccountDefinitionExtendedSettings
    {
        public abstract Guid ConfigId { get; }
        public abstract string RuntimeEditor { get; }

    }
}
