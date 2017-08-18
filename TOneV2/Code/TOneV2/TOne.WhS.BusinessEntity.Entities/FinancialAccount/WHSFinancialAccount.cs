using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class WHSFinancialAccount : Vanrise.Entities.IDateEffectiveSettings
    {
        public int FinancialAccountId { get; set; }

        public int? CarrierProfileId { get; set; }

        public int? CarrierAccountId { get; set; }

        public Guid FinancialAccountDefinitionId { get; set; }

        public WHSFinancialAccountSettings Settings { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
    }

    public class WHSFinancialAccountSettings
    {
        public WHSFinancialAccountExtendedSettings ExtendedSettings { get; set; }
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
