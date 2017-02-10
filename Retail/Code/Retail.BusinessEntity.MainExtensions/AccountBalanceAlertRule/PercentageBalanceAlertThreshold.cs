using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using Retail.BusinessEntity.MainExtensions.AccountParts;
using Retail.BusinessEntity.MainExtensions.ProductTypes.PostPaid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Business.Extensions;
using Vanrise.Notification.Business;
using Vanrise.Notification.Entities;

namespace Retail.BusinessEntity.MainExtensions.AccountBalanceAlertRule
{
    public class PercentageBalanceAlertThreshold : VRBalanceAlertThreshold
    {
        public Decimal Percentage { get; set; }
        public override Guid ConfigId
        {
            get { return new Guid("30B37A0A-63D8-4323-899B-3A2782FC5A05"); }
        }

        public override decimal GetThreshold(IVRBalanceAlertThresholdContext context)
        {
            VRAccountBalanceManager vRAccountBalanceManager = new VRAccountBalanceManager();
            var accountBEDefinitionId = vRAccountBalanceManager.GetAccountBEDefinitionIdByAlertRuleTypeId(context.AlertRuleTypeId);
            AccountBEManager accountBEManager = new AccountBEManager();
            AccountPart accountPart;

            if (!accountBEManager.TryGetAccountPart(accountBEDefinitionId, Convert.ToInt64(context.EntityBalanceInfo.EntityId), AccountPartFinancial._ConfigId, false, out accountPart))
                throw new NullReferenceException("accountPart");
            var accountPartFinancial = accountPart.Settings as AccountPartFinancial;
            if (accountPartFinancial == null)
                throw new NullReferenceException("accountPartFinancial");

            ProductManager productManager = new ProductManager();
            var product = productManager.GetProduct(accountPartFinancial.ProductId);
            if (product == null)
                throw new NullReferenceException("product");

            var postPaidSettings = product.Settings.ExtendedSettings as PostPaidSettings;
            if (postPaidSettings == null)
                throw new NullReferenceException("postPaidSettings");

            return -(postPaidSettings.CreditLimit * (100 - this.Percentage) / 100);
        }
    }
}
