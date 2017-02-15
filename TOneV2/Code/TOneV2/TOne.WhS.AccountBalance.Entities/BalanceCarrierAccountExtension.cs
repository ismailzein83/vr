using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.AccountBalance.Entities
{
    public enum PaymentType { Postpaid = 0, Prepaid = 1 }

    public class BalanceCarrierAccountExtension
    {
        public bool IsNetting { get; set; }

        public PaymentType? CustomerPaymentType { get; set; }

        public PaymentType? SupplierPaymentType { get; set; }

        /// <summary>
        /// only applicable for postpaid and Netting
        /// </summary>
        public Decimal? CustomerCreditLimit { get; set; }

        /// <summary>
        /// only applicable for postpaid and Netting
        /// </summary>
        public Decimal? SupplierCreditLimit { get; set; }

        /// <summary>
        /// only applicable for prepaid
        /// </summary>
        public Decimal? CustomerEstimatedMaxBalance { get; set; }

        /// <summary>
        /// only applicable for prepaid
        /// </summary>
        public Decimal? SupplierEstimatedMaxBalance { get; set; }
    }
}
