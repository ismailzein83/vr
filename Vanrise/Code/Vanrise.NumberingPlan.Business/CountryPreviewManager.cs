using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.NumberingPlan.Data;
using Vanrise.NumberingPlan.Entities;

namespace Vanrise.NumberingPlan.Business
{
    public class CountryPreviewManager
    {
        public Vanrise.Entities.IDataRetrievalResult<CountryPreviewDetail> GetFilteredCountryPreview(Vanrise.Entities.DataRetrievalInput<SPLPreviewQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new CountryPreviewRequestHandler());
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

        private class CountryPreviewRequestHandler : BigDataRequestHandler<SPLPreviewQuery, CountryPreview, CountryPreviewDetail>
        {
            public override CountryPreviewDetail EntityDetailMapper(CountryPreview entity)
            {
                CountryPreviewManager manager = new CountryPreviewManager();
                return manager.CountryPreviewDetailMapper(entity);
            }

            public override IEnumerable<CountryPreview> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<SPLPreviewQuery> input)
            {
                ISaleCountryPreviewDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<ISaleCountryPreviewDataManager>();
                return dataManager.GetFilteredCountryPreview(input.Query);
            }
        }

        #endregion


    }
}
