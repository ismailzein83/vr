using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Invoice.Entities;
using Vanrise.Common;
namespace Retail.BusinessEntity.MainExtensions
{
    public class AccountCitySerialNumberPart : VRConcatenatedPartSettings<IInvoiceSerialNumberConcatenatedPartContext>
    {
        public override Guid ConfigId { get { return new Guid("9B59A5E8-923A-4E9E-8338-9549150EC88C"); } }
        public override string GetPartText(IInvoiceSerialNumberConcatenatedPartContext context)
        {
            FinancialAccountManager financialAccountManager = new FinancialAccountManager();
            Guid accountBEDefinitionId;
            long accountId;
            FinancialAccountData financialAccountData;
            financialAccountManager.ResolveInvoiceAccountId(context.InvoiceTypeId, context.Invoice.PartnerId, out accountBEDefinitionId, out accountId, out financialAccountData);
            AccountBEManager accountBEManager = new AccountBEManager();
            int cityId = accountBEManager.GetCityId(accountBEDefinitionId, accountId, true);
            CityManager cityManager = new CityManager();
            var city = cityManager.GetCity(cityId);
            city.ThrowIfNull("city", cityId);
            city.Settings.ThrowIfNull("city.Settings");
            return city.Settings.Abbreviation;
        }
    }
}
