using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Common.Business;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SupplierOtherRateManager
    {

        #region Public Methods
        public Vanrise.Entities.IDataRetrievalResult<SupplierOtherRateDetail> GetFilteredSupplierOtherRates(Vanrise.Entities.DataRetrievalInput<SupplierOtherRateQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new SupplierOtherRateRequestHandler());
        }

        #endregion

       
        #region Mappers
        private SupplierOtherRateDetail SupplierOtherRateDetailMapper(SupplierOtherRate supplierOtherRate)
        {
            CurrencyManager currencyManager = new CurrencyManager();
            int currencyId;

            if (supplierOtherRate.CurrencyId.HasValue)
                currencyId = supplierOtherRate.CurrencyId.Value;
            else
            {
                SupplierPriceListManager manager = new SupplierPriceListManager();
                SupplierPriceList priceList = manager.GetPriceList(supplierOtherRate.PriceListId);
                currencyId = priceList.CurrencyId;
            }

            return new SupplierOtherRateDetail()
            {
                Entity = supplierOtherRate,
                CurrencyName = currencyManager.GetCurrencySymbol(currencyId)
            };
        }
        #endregion

        #region Private Classes

        private class SupplierOtherRateRequestHandler : BigDataRequestHandler<SupplierOtherRateQuery, SupplierOtherRate, SupplierOtherRateDetail>
        {
            public override SupplierOtherRateDetail EntityDetailMapper(SupplierOtherRate entity)
            {
                SupplierOtherRateManager manager = new SupplierOtherRateManager();
                return manager.SupplierOtherRateDetailMapper(entity);
            }

            public override IEnumerable<SupplierOtherRate> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<SupplierOtherRateQuery> input)
            {
                ISupplierOtherRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierOtherRateDataManager>();
                return dataManager.GetFilteredSupplierOtherRates(input.Query);
            }
        }

        #endregion
    }
}
