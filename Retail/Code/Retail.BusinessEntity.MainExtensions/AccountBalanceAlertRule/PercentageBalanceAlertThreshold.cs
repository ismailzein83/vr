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
using Vanrise.Common;

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
                throw new NullReferenceException(String.Format("accountPart. Account '{0}'", context.EntityBalanceInfo.EntityId));
            var accountPartFinancial = accountPart.Settings.CastWithValidate<AccountPartFinancial>("accountPart.Settings", context.EntityBalanceInfo.EntityId);

            ProductManager productManager = new ProductManager();
            var product = productManager.GetProduct(accountPartFinancial.ProductId);
            product.ThrowIfNull("product", accountPartFinancial.ProductId);
            product.Settings.ThrowIfNull("product.Settings", accountPartFinancial.ProductId);

            var postPaidSettings = product.Settings.ExtendedSettings.CastWithValidate<PostPaidSettings>("product.Settings.ExtendedSettings", accountPartFinancial.ProductId);
            
            return -(postPaidSettings.CreditLimit * (100 - this.Percentage) / 100);
        }
    }
}
