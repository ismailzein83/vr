using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Analytics.Data;
using TOne.WhS.Analytics.Entities;
using TOne.WhS.BusinessEntity.Business;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.Analytics.Business.BillingReports
{
    public class BillingReportManager
    {
        #region Public Methods

        public IDataRetrievalResult<BillingReportRecord> GetFilteredBillingReportRecords(Vanrise.Entities.DataRetrievalInput<BillingReportQuery> input)
        {
            ValidateBillingReportQuery(input.Query);
            return BigDataManager.Instance.RetrieveData(input, new BillingReportManager.BillingReportRequestHandler(input.Query));
        }

        #endregion

        #region Private Classes

        class BillingReportRequestHandler : BigDataRequestHandler<BillingReportQuery, BillingReportRecord, BillingReportRecord>
        {
            #region Fields / Constructors

            BillingReportQuery _query;
            CarrierAccountManager _carrierAccountManager;
            SaleZoneManager _saleZoneManager;

            public BillingReportRequestHandler(BillingReportQuery query)
            {
                _query = query;
                _carrierAccountManager = new CarrierAccountManager();
                _saleZoneManager = new SaleZoneManager();
            }

            #endregion

            public override IDataRetrievalResult<BillingReportRecord> AllRecordsToDataResult(DataRetrievalInput<BillingReportQuery> input, IEnumerable<BillingReportRecord> allRecords)
            {
                var variationReportBigResult = new BillingReportBigResult();

                BigResult<BillingReportRecord> bigResult = allRecords.ToBigResult(input, null, this.EntityDetailMapper);

                variationReportBigResult.ResultKey = bigResult.ResultKey;
                variationReportBigResult.Data = bigResult.Data;
                variationReportBigResult.TotalCount = bigResult.TotalCount;
                return variationReportBigResult;
            }

            public override IEnumerable<BillingReportRecord> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<BillingReportQuery> input)
            {
                IGenericBillingDataManager dataManager = AnalyticsDataManagerFactory.GetDataManager<IGenericBillingDataManager>();
                return dataManager.GetFilteredBillingReportRecords(input);
            }

            public override BillingReportRecord EntityDetailMapper(BillingReportRecord entity)
            {


                return entity;
            }

            #region Private Methods

            #endregion
        }

        #endregion

        #region Private Methods

        void ValidateBillingReportQuery(BillingReportQuery query)
        {
            if (query.FromDate == default(DateTime))
                throw new ArgumentNullException(String.Format("query.FromDate"));
            if (query.ToDate == default(DateTime))
                throw new ArgumentNullException(String.Format("query.ToDate"));
        }

        #endregion
    }
}
