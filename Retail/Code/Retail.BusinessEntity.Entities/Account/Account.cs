using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class BaseAccount
    {
        public long AccountId { get; set; }

        public string Name { get; set; }

        public Guid TypeId { get; set; }

        public AccountSettings Settings { get; set; }
    }

    public class AccountSettings
    {
        public AccountPartCollection Parts { get; set; }

        /// <summary>
        /// has value if it only has chargeable status in the account type's supported statuses
        /// </summary>
        public int? StatusChargingSetId { get; set; }
    }

    public class Account : BaseAccount
    {
        public const string BUSINESSENTITY_DEFINITION_NAME = "Retail_BE_SubscriberAccount";
        public Guid StatusId { get; set; }
        public long? ParentAccountId { get; set; }
        public string SourceId { get; set; }
        public ExecutedActionsData ExecutedActionsData { get; set; }


    }

    public class AccountToEdit : BaseAccount
    {
        public string SourceId { get; set; }
    }



    public interface IAccountPayment
    {
        int PaymentMethodId { get; }

        int CurrencyId { get; }

        int CreditClassId { get; }

        string BankDetails { get; }

        int StatusChargingSetId { get; }
    }

    public class PaymentMethod
    {
        public int PaymentMethodId { get; set; }

        public string Name { get; set; }

        public PaymentMethodSettings Settings { get; set; }
    }

    public class PaymentMethodSettings
    {
    }
}
