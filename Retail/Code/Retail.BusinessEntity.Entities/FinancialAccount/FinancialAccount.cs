using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public enum FinancialAccountEffective { EffectiveOnly = 0, All = 1 }

    public class FinancialAccount : Vanrise.Entities.IDateEffectiveSettings
    {
        public int SequenceNumber { get; set; }

        public Guid FinancialAccountDefinitionId { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }

        public FinancialAccountExtendedSettings ExtendedSettings { get; set; }
    }

    public abstract class FinancialAccountExtendedSettings
    {
        public abstract void FillExtraData(IFinancialAccountFillExtraDataContext context);
    }

    public interface IFinancialAccountFillExtraDataContext
    {
        FinancialAccountData FinancialAccountData { get; }
    }
}
