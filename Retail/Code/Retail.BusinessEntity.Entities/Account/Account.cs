﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public struct AccountDefinition
    {
        public Guid AccountBEDefinitionId { get; set; }

        public long AccountId { get; set; }
    }

    public class BaseAccount
    {
        public long AccountId { get; set; }

        public string Name { get; set; }

        public Guid TypeId { get; set; }

        public AccountSettings Settings { get; set; }

        public DateTime CreatedTime { get; set; }
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
        public Dictionary<string, BaseAccountExtendedSettings> ExtendedSettings { get; set; }
    }

    public class AccountToInsert : BaseAccount
    {
        public string SourceId { get; set; }

        public Guid AccountBEDefinitionId { get; set; }

        public Guid StatusId { get; set; }

        public long? ParentAccountId { get; set; }

        public Dictionary<string, BaseAccountExtendedSettings> ExtendedSettings { get; set; }
    }

    public class AccountToEdit : BaseAccount
    {
        public string SourceId { get; set; }

        public Guid AccountBEDefinitionId { get; set; }
    }

    public interface IAccountPayment
    {
        int CurrencyId { get; }

        int ProductId { get; set; }
    }

    public interface IAccountProfile
    {
        string Address { get; }

        List<string> Faxes { get; }

        List<string> PhoneNumbers { get; }
        int? CityId { get; }
    }

    public interface IOperatorSetting
    {
        bool IsMobileOperator { get; }
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
    public abstract class BaseAccountExtendedSettings
    {

    }
}
