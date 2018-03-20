using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class WHSFinancialAccount : Vanrise.Entities.IDateEffectiveSettings
    {
        public const string STATICBUSINESSENTITY_DEFINITION_NAME = "WhS_BE_FinancialAccount";
        public static Guid STATICBUSINESSENTITY_DEFINITION_ID = new Guid("2A148F4E-8A99-481A-875B-3221C24C5977");

        public int FinancialAccountId { get; set; }

        public int? CarrierProfileId { get; set; }

        public int? CarrierAccountId { get; set; }

        public Guid FinancialAccountDefinitionId { get; set; }

        public WHSFinancialAccountSettings Settings { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }

        public DateTime CreatedTime { get; set; }

        public int? CreatedBy { get; set; }

        public int? LastModifiedBy { get; set; }

        public DateTime? LastModifiedTime { get; set; }
    }

    public class WHSFinancialAccountSettings
    {
        public WHSFinancialAccountExtendedSettings ExtendedSettings { get; set; }
        public List<FinancialAccountInvoiceSetting> FinancialAccountInvoiceSettings { get; set; }
    }
    public enum CommissionType { Display = 0 , DoNotDisplay = 1}
    public class FinancialAccountInvoiceSetting
    {
        public Guid InvoiceTypeId { get; set; }
        public FinancialAccountCommission FinancialAccountCommission { get; set; }
    }
    public class FinancialAccountCommission
    {
        public decimal? Commission { get; set; }
        public CommissionType CommissionType { get; set; }
    }

    public abstract class WHSFinancialAccountExtendedSettings
    {
        public virtual void FillCustomerExtraData(IWHSFinancialAccountFillCustomerExtraDataContext context)
        {

        }

        public virtual void FillSupplierExtraData(IWHSFinancialAccountFillSupplierExtraDataContext context)
        {

        }
    }

    public interface IWHSFinancialAccountFillCustomerExtraDataContext
    {
        WHSCarrierFinancialAccountData FinancialAccountData { get; }
    }

    public interface IWHSFinancialAccountFillSupplierExtraDataContext
    {
        WHSCarrierFinancialAccountData FinancialAccountData { get; }
    }
}
