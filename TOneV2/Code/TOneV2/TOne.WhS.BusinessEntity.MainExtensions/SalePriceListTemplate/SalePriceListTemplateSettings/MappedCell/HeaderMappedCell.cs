using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common.Business;

namespace TOne.WhS.BusinessEntity.MainExtensions
{
    public enum HeaderFiledType
    {
        CompanyName = 0,
        CustomerName = 1,
        PricelistDate = 2,
        PricelistCurrency = 3,
        PricelistType = 4,
    }

    public class HeaderMappedCell : MappedCell
    {
        public override Guid ConfigId
        {
            get { return new Guid("89BDA522-A5FC-4B1C-8E9B-4D390EB8F6AF"); }
        }
        public HeaderFiledType HeaderField { get; set; }

        public override void Execute(Entities.IMappedCellContext context)
        {
           
            switch (HeaderField)
            {
                case HeaderFiledType.CompanyName:
                    context.Value = getCompanyName(context.CustomerId);
                    break;
                case HeaderFiledType.CustomerName:
                    context.Value = getCustomerName(context.CustomerId);
                    break;
                case HeaderFiledType.PricelistDate:
                    context.Value = context.PricelistDate;
                    break;
                case HeaderFiledType.PricelistCurrency:
                    context.Value = getPricelistCurrencyName(context.PricelistCurrencyId);
                    break;
                case HeaderFiledType.PricelistType:
                    context.Value = getPricelistType(context.PricelistType);
                    break;
                default:
                    context.Value = null;
                    break;
            }
        }

        private string getCompanyName(int customerId)
        {
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            return carrierAccountManager.GetCompanySetting(customerId).CompanyName;
        }
        private string getCustomerName(int customerId)
        {
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            return carrierAccountManager.GetCarrierAccountName(customerId);
        }
        private string getPricelistCurrencyName(int currencyId)
        {
            CurrencyManager currencyManager = new CurrencyManager();
            return currencyManager.GetCurrencyName(currencyId);
        }
        private string getPricelistType(SalePriceListType pricelistType)
        {
            switch (pricelistType)
            {
                case SalePriceListType.Full:
                    return "Full";
                case SalePriceListType.Country:
                    return "Country";
                case SalePriceListType.RateChange:
                    return "Rate Change";
            }
            return null;
        }

    }
}