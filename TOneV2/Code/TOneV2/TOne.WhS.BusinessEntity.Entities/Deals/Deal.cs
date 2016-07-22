using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class Deal
    {
        public int DealId { get; set; }

        public DealSettings Settings { get; set; }
    }

    public enum DealContractType { BalancedAmount = 0, BalancedDuration = 1, UnBalanced = 2 }

    public enum DealAgreementType { Gentlemen = 0, Commitment = 1 }

    public class DealSettings
    {
        public string Description { get; set; }

        public int CarrierAccountId { get; set; }

        public DateTime BeginDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int GracePeriod { get; set; }
        public Decimal SellingAmount { get; set; }
        public Decimal SellingDuration { get; set; }
        public Decimal BuyingAmount { get; set; }
        public Decimal BuyingDuration { get; set; }
        public bool Active { get; set; }

        public DealContractType ContractType { get; set; }

        public DealAgreementType AgreementType { get; set; }

        public List<DealSellingPart> SellingParts { get; set; }

        public List<DealBuyingPart> BuyingParts { get; set; }
    }
}