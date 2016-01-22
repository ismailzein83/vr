using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
namespace TOne.WhS.BusinessEntity.Business
{
    public class SalePriceListManager
    {



        public Vanrise.Entities.IDataRetrievalResult<SalePriceListDetail> GetPricelists(Vanrise.Entities.DataRetrievalInput<string> input) 
        {
            var salePricelists = GetCachedSalePriceLists();

            Func<SalePriceList, bool> filterExpression = (priceList) =>
                 ( priceList.OwnerId > 0 );

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, salePricelists.ToBigResult(input, filterExpression, SalePricelistDetailMapper));

        }
 

        public SalePriceList GetPriceList(int priceListId)
        {
            List<SalePriceList> salePriceLists = GetCachedSalePriceLists();
            var salePriceList = salePriceLists.FindRecord(x => x.PriceListId == priceListId);
            return salePriceList;
        }

        List<SalePriceList> GetCachedSalePriceLists()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(String.Format("GetCashedSalePriceLists"),
               () =>
               {
                   ISalePriceListDataManager dataManager = BEDataManagerFactory.GetDataManager<ISalePriceListDataManager>();
                   return dataManager.GetPriceLists();
               });
        }
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ISalePriceListDataManager _dataManager = BEDataManagerFactory.GetDataManager<ISalePriceListDataManager>();
            object _updateHandle;

            public override Vanrise.Caching.CacheObjectSize ApproximateObjectSize
            {
                get
                {
                    return Vanrise.Caching.CacheObjectSize.Large;
                }
            }

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.ArGetSalePriceListsUpdated(ref _updateHandle);
            }
        }

        private string GetCurrencyName(int? currencyId)
        {
            if (currencyId != null)
            {
                CurrencyManager manager = new CurrencyManager();
                Currency currency = manager.GetCurrency(currencyId.Value);

                if (currency != null)
                    return currency.Name;
            }

            return "Currency Not Found";
        }


        #region Mappers

        private SalePriceListDetail SalePricelistDetailMapper(SalePriceList priceList)
        {
            SalePriceListDetail pricelistDetail = new SalePriceListDetail();
            CarrierAccountManager accountManager = new CarrierAccountManager();
            SellingProductManager productManager = new SellingProductManager();
            pricelistDetail.Entity = priceList;
            if (priceList.OwnerType == 0)
            {
                pricelistDetail.ownerType = "Selling Product";
            }
            else
            {
                pricelistDetail.ownerType = "Customer";
            }
             
            if (priceList.OwnerType != SalePriceListOwnerType.Customer)
            {
                pricelistDetail.OwnerName = productManager.GetSellingProductName(priceList.OwnerId);
            }


            else {
                pricelistDetail.OwnerName = accountManager.GetCarrierAccountName(priceList.OwnerId);
            }
          
           
            pricelistDetail.CurrencyName = GetCurrencyName(priceList.CurrencyId);
             return pricelistDetail;
        }

        #endregion

    }
}
