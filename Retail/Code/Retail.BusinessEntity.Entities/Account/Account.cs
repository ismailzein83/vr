﻿using System;
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

        public AccountType Type { get; set; }

        public AccountSettings Settings { get; set; }
    }

    public enum AccountType { Residential = 0, Enterprise = 1 }

    public class AccountSettings
    {
        public AccountContactSettings ContactSettings { get; set; }

        public AccountBillingSettings BillingSettings { get; set; }

        public List<AccountPart> Parts { get; set; }
    }

    public class Account : BaseAccount
    {
        public const string BUSINESSENTITY_DEFINITION_NAME = "Retail_BE_SubscriberAccount";
        public long? ParentAccountId { get; set; }
    }

    public abstract class AccountPart
    {
        public int ConfigId { get; set; }
    }

    public class AccountToEdit : BaseAccount
    {

    }

    public class AccountContactSettings
    {
        public int? CountryId { get; set; }

        public int? CityId { get; set; }

        public string Town { get; set; }

        public string Street { get; set; }

        public string POBox { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Fax { get; set; }
    }

    public class AccountBillingSettings
    {
        public string ContactName { get; set; }

        public string ContactEmail { get; set; }

        public string ContactPhone { get; set; }

        public PaymentMethod? PaymentMethod { get; set; }
    }

    public enum PaymentMethod { Prepaid = 0, Postpaid = 1 }
}
