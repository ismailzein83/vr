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
            ISupplierCountryPreviewDataManager dataManager = SupPLDataManagerFactory.GetDataManager<ISupplierCountryPreviewDataManager>();

            BigResult<CountryPreview> countriesPreview = dataManager.GetCountryPreviewFilteredFromTemp(input);
            BigResult<CountryPreviewDetail> countryPreviewDetailResult = new BigResult<CountryPreviewDetail>()
            {
                ResultKey = countriesPreview.ResultKey,
                TotalCount = countriesPreview.TotalCount,
                Data = countriesPreview.Data.MapRecords(CountryPreviewDetailMapper)
            };

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, countryPreviewDetailResult);

        }

        private CountryPreviewDetail CountryPreviewDetailMapper(CountryPreview countryPreview)
        {
            CountryPreviewDetail countryPreviewDetail = new CountryPreviewDetail();
            countryPreviewDetail.Entity = countryPreview;

            CountryManager manager = new CountryManager();
            countryPreviewDetail.CountryName = manager.GetCountryName(countryPreview.CountryId);
            return countryPreviewDetail;
        }


    }
}
