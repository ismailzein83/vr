using System;
using System.ComponentModel;

namespace Retail.BusinessEntity.Entities
{
    public enum EntityType {
        [Description("Account")]
        Account = 0,
        [Description("Account Service")]
        AccountService = 1 }
}
