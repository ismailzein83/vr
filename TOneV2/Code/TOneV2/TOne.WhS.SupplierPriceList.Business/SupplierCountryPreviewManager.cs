using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Data;
using TOne.WhS.SupplierPriceList.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class SupplierCountryPreviewManager
    {
        public Vanrise.Entities.IDataRetrievalResult<CountryPreviewDetail> GetFilteredCountryPreview(Vanrise.Entities.DataRetrievalInput<SPLPreviewQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new SupplierCountryPreviewRequestHandler());
        }


        private CountryPreviewDetail CountryPreviewDetailMapper(CountryPreview countryPreview)
        {
            CountryPreviewDetail countryPreviewDetail = new CountryPreviewDetail();
            countryPreviewDetail.Entity = countryPreview;

            CountryManager manager = new CountryManager();
            countryPreviewDetail.CountryName = manager.GetCountryName(countryPreview.CountryId);
            return countryPreviewDetail;
        }


        #region Private Classes

        private class SupplierCountryPreviewRequestHandler : BigDataRequestHandler<SPLPreviewQuery, CountryPreview, CountryPreviewDetail>
        {
            public override CountryPreviewDetail EntityDetailMapper(CountryPreview entity)
            {
                SupplierCountryPreviewManager manager = new SupplierCountryPreviewManager();
                return manager.CountryPreviewDetailMapper(entity);
            }

            public override IEnumerable<CountryPreview> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<SPLPreviewQuery> input)
            {
                ISupplierCountryPreviewDataManager dataManager = SupPLDataManagerFactory.GetDataManager<ISupplierCountryPreviewDataManager>();
                return dataManager.GetFilteredCountryPreview(input.Query);
            }
        }

        #endregion


    }
}
