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
        #region Public Methods

        public Vanrise.Entities.IDataRetrievalResult<SalePriceListDetail> GetFilteredPricelists(Vanrise.Entities.DataRetrievalInput<SalePriceListQuery> input)
        {
            var salePricelists = GetCachedSalePriceLists();

            Func<SalePriceList, bool> filterExpression = (priceList) =>

                     (input.Query.OwnerId == null || input.Query.OwnerId.Contains(priceList.OwnerId)) &&
                      (input.Query.OwnerType == null || priceList.OwnerType == input.Query.OwnerType);


            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, salePricelists.ToBigResult(input, filterExpression, SalePricelistDetailMapper));

        }
        
        public SalePriceList GetPriceList(int priceListId)
        {
            return GetCachedSalePriceLists().GetRecord(priceListId);
        }

        public long ReserveIdRange(int numberOfIds)
        {
            long startingId;
            IDManager.Instance.ReserveIDRange(this.GetType(), numberOfIds, out startingId);
            return startingId;
        }

        public int GetSalePriceListTypeId()
        {
            return Vanrise.Common.Business.TypeManager.Instance.GetTypeId(this.GetSalePriceListType());
        }

        public Type GetSalePriceListType()
        {
            return this.GetType();
        }

        public bool UpdateSalePriceList(SalePriceList salePriceList)
        {
            ISalePriceListDataManager dataManager = BEDataManagerFactory.GetDataManager<ISalePriceListDataManager>();
            return dataManager.Update(salePriceList);
        }

        public bool AddSalePriceList(SalePriceList salePriceList)
        {
            ISalePriceListDataManager dataManager = BEDataManagerFactory.GetDataManager<ISalePriceListDataManager>();
            return dataManager.Insert(salePriceList);
        }
        public IEnumerable<SalePriceList> GetCustomerSalePriceListsByProcessInstanceId(long processInstanceId)
        {
            Dictionary<int, SalePriceList> allSalePriceLists = GetCachedSalePriceLists();
            SalePriceListOwnerType customerOwnerType = SalePriceListOwnerType.Customer;

            return allSalePriceLists.Values.FindAllRecords(itm => itm.ProcessInstanceId == processInstanceId && itm.OwnerType == customerOwnerType);
        }

        public bool IsSalePriceListDeleted(int priceListId)
        {
            Dictionary<int, SalePriceList> allSalePriceLists = this.GetCachedSalePriceListsWithDeleted();
            SalePriceList salePriceList = allSalePriceLists.GetRecord(priceListId);

            if (salePriceList == null)
                throw new DataIntegrityValidationException(string.Format("Sale Price List with Id {0} does not exist", priceListId));

            return salePriceList.IsDeleted;
        }

        #endregion

        #region  Private Members

        public Dictionary<int, SalePriceList> GetCachedSalePriceLists()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(String.Format("GetCashedSalePriceLists"),
               () =>
               {
                   ISalePriceListDataManager dataManager = BEDataManagerFactory.GetDataManager<ISalePriceListDataManager>();
                   Dictionary<int, SalePriceList> allSalePriceLists = this.GetCachedSalePriceListsWithDeleted();
                   Dictionary<int, SalePriceList> dic = new Dictionary<int, SalePriceList>();

                   foreach (SalePriceList item in allSalePriceLists.Values)
                   {
                       if (!item.IsDeleted)
                           dic.Add(item.PriceListId, item);
                   }
                   return dic;
               });
        }

        private Dictionary<int, SalePriceList> GetCachedSalePriceListsWithDeleted()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(String.Format("AllSalePriceLists"),
               () =>
               {
                   ISalePriceListDataManager dataManager = BEDataManagerFactory.GetDataManager<ISalePriceListDataManager>();
                   IEnumerable<SalePriceList> salePriceLists = dataManager.GetPriceLists();
                   Dictionary<int, SalePriceList> dic = new Dictionary<int, SalePriceList>();
                   CarrierAccountManager carrierAccountManager = new CarrierAccountManager();

                   foreach (SalePriceList item in salePriceLists)
                   {
                       if (item.OwnerType == SalePriceListOwnerType.Customer && carrierAccountManager.IsCarrierAccountDeleted(item.OwnerId))
                           item.IsDeleted = true;

                       dic.Add(item.PriceListId, item);
                   }
                   return dic;
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
            if (currencyId.HasValue)
            {
                CurrencyManager manager = new CurrencyManager();
                return manager.GetCurrencySymbol(currencyId.Value);

            }

            return "Currency Not Found";
        }

        #endregion

        #region Mappers

        private SalePriceListDetail SalePricelistDetailMapper(SalePriceList priceList)
        {
            SalePriceListDetail pricelistDetail = new SalePriceListDetail();
            pricelistDetail.Entity = priceList;
            pricelistDetail.OwnerType = Vanrise.Common.Utilities.GetEnumDescription(priceList.OwnerType);
            pricelistDetail.PriceListTypeName = Vanrise.Common.Utilities.GetEnumDescription(priceList.PriceListType);


            if (priceList.OwnerType != SalePriceListOwnerType.Customer)
            {
                SellingProductManager productManager = new SellingProductManager();
                pricelistDetail.OwnerName = productManager.GetSellingProductName(priceList.OwnerId);
            }

            else
            {
                CarrierAccountManager accountManager = new CarrierAccountManager();
                pricelistDetail.OwnerName = accountManager.GetCarrierAccountName(priceList.OwnerId);
            }


            pricelistDetail.CurrencyName = GetCurrencyName(priceList.CurrencyId);
            return pricelistDetail;
        }

        #endregion

    }
}
