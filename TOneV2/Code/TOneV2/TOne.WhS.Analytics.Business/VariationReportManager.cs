using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Analytics.Data;
using TOne.WhS.Analytics.Entities;
using TOne.WhS.BusinessEntity.Business;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.Analytics.Business
{
    public class VariationReportManager
    {
        #region Public Methods

        public IDataRetrievalResult<VariationReportRecord> GetFilteredVariationReportRecords(Vanrise.Entities.DataRetrievalInput<VariationReportQuery> input)
        {
            ValidateVariationReportQuery(input.Query);
            return BigDataManager.Instance.RetrieveData(input, new VariationReportRequestHandler(input.Query));
        }

        #endregion

        #region Private Classes

        class VariationReportRequestHandler : BigDataRequestHandler<VariationReportQuery, VariationReportRecord, VariationReportRecord>
        {
            #region Fields / Constructors

            VariationReportQuery _query;
            CarrierAccountManager _carrierAccountManager;
            SaleZoneManager _saleZoneManager;
            List<TimePeriod> _timePeriods;

            public VariationReportRequestHandler(VariationReportQuery query)
            {
                _query = query;
                _carrierAccountManager = new CarrierAccountManager();
                _saleZoneManager = new SaleZoneManager();
                _timePeriods = GetTimePeriods();
            }

            #endregion

            public override IDataRetrievalResult<VariationReportRecord> AllRecordsToDataResult(DataRetrievalInput<VariationReportQuery> input, IEnumerable<VariationReportRecord> allRecords)
            {
                var variationReportBigResult = new VariationReportBigResult();

                BigResult<VariationReportRecord> bigResult = allRecords.ToBigResult(input, null, this.EntityDetailMapper);

                variationReportBigResult.ResultKey = bigResult.ResultKey;
                variationReportBigResult.Data = bigResult.Data;
                variationReportBigResult.TotalCount = bigResult.TotalCount;

                variationReportBigResult.DimensionTitle = GetDimensionTitle();
                variationReportBigResult.TimePeriods = _timePeriods;
                variationReportBigResult.DrillDownDimensions = GetDrillDownDimensions();

                return variationReportBigResult;
            }

            public override IEnumerable<VariationReportRecord> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<VariationReportQuery> input)
            {
                IVariationReportDataManager dataManager = AnalyticsDataManagerFactory.GetDataManager<IVariationReportDataManager>();
                return dataManager.GetFilteredVariationReportRecords(input, _timePeriods);
            }

            public override VariationReportRecord EntityDetailMapper(VariationReportRecord entity)
            {
                SetVariationReportRecordDimension(entity);

                entity.DimensionName = (_query.ReportType == VariationReportType.TopDestinationMinutes || _query.ReportType == VariationReportType.TopDestinationAmount) ?
                    _saleZoneManager.GetSaleZoneName((long)entity.DimensionId) :
                    _carrierAccountManager.GetCarrierAccountName((int)entity.DimensionId);

                if (entity.DimensionSuffix != VariationReportRecordDimensionSuffix.None)
                    entity.DimensionName = String.Format("{0}{1}", entity.DimensionName, String.Format(" / {0}", entity.DimensionSuffix));

                return entity;
            }

            #region Private Methods

            string GetDimensionTitle()
            {
                string dimensionTitle = null;
                switch (_query.ReportType)
                {
                    case VariationReportType.InBoundMinutes:
                    case VariationReportType.InBoundAmount:
                        dimensionTitle = "Customer";
                        break;
                    case VariationReportType.OutBoundMinutes:
                    case VariationReportType.OutBoundAmount:
                    case VariationReportType.InOutBoundMinutes:
                    case VariationReportType.InOutBoundAmount:
                        dimensionTitle = "Supplier";
                        break;
                    case VariationReportType.TopDestinationMinutes:
                    case VariationReportType.TopDestinationAmount:
                        dimensionTitle = "Zone";
                        break;
                }
                return dimensionTitle;
            }

            #region Time Periods

            List<TimePeriod> GetTimePeriods()
            {
                var timePeriods = new List<TimePeriod>();

                DateTime toDate = _query.ToDate.Date.AddDays(1); // Ignore ToDate's time and add 1 day to include it in the result

                for (int i = 0; i < _query.NumberOfPeriods; i++)
                {
                    DateTime fromDate = GetFromDate(toDate);
                    timePeriods.Add(new TimePeriod()
                    {
                        PeriodDescription = (_query.TimePeriod == VariationReportTimePeriod.Daily) ?
                            fromDate.ToShortDateString() : String.Format("{0} - {1}", fromDate.ToShortDateString(), toDate.AddDays(-1).ToShortDateString()),
                        From = fromDate,
                        To = toDate
                    });
                    toDate = fromDate;
                }

                return timePeriods;
            }

            DateTime GetFromDate(DateTime toDate)
            {
                var fromDate = new DateTime();
                switch (_query.TimePeriod)
                {
                    case VariationReportTimePeriod.Daily:
                        fromDate = toDate.AddDays(-1);
                        break;
                    case VariationReportTimePeriod.Weekly:
                        fromDate = toDate.AddDays(-7);
                        break;
                    case VariationReportTimePeriod.Monthly:
                        fromDate = toDate.AddMonths(-1);
                        break;
                }
                return fromDate;
            }

            #endregion

            void SetVariationReportRecordDimension(VariationReportRecord variationReportRecord)
            {
                switch (_query.ReportType)
                {
                    case VariationReportType.InBoundMinutes:
                    case VariationReportType.InBoundAmount:
                        variationReportRecord.Dimension = VariationReportDimension.Customer;
                        break;
                    case VariationReportType.OutBoundMinutes:
                    case VariationReportType.OutBoundAmount:
                        variationReportRecord.Dimension = VariationReportDimension.Supplier;
                        break;
                    case VariationReportType.TopDestinationMinutes:
                    case VariationReportType.TopDestinationAmount:
                        variationReportRecord.Dimension = VariationReportDimension.Zone;
                        break;
                    default:
                        switch (variationReportRecord.DimensionSuffix)
                        {
                            case VariationReportRecordDimensionSuffix.In:
                                variationReportRecord.Dimension = VariationReportDimension.Customer;
                                break;
                            case VariationReportRecordDimensionSuffix.Out:
                                variationReportRecord.Dimension = VariationReportDimension.Supplier;
                                break;
                        }
                        break;
                }
            }

            List<VariationReportDimension> GetDrillDownDimensions()
            {
                var drillDownDimensions = new List<VariationReportDimension>();
                if (_query.ParentReportType == null)
                {
                    switch (_query.ReportType)
                    {
                        case VariationReportType.TopDestinationMinutes:
                        case VariationReportType.TopDestinationAmount:
                            drillDownDimensions.Add(VariationReportDimension.Customer);
                            drillDownDimensions.Add(VariationReportDimension.Supplier);
                            break;
                        default:
                            drillDownDimensions.Add(VariationReportDimension.Zone);
                            break;
                    }
                }
                return (drillDownDimensions.Count > 0) ? drillDownDimensions : null;
            }

            #endregion
        }

        #endregion

        #region Private Methods

        void ValidateVariationReportQuery(VariationReportQuery query)
        {
            if (!Enum.IsDefined(typeof(VariationReportType), query.ReportType))
                throw new ArgumentException(String.Format("query.ReportType '{0}' is invalid", query.ReportType));
            if (query.ToDate == default(DateTime))
                throw new ArgumentNullException(String.Format("query.ToDate"));
            if (!Enum.IsDefined(typeof(VariationReportTimePeriod), query.TimePeriod))
                throw new ArgumentException(String.Format("query.TimePeriod '{0}' is invalid", query.TimePeriod));
            if (query.NumberOfPeriods < 1)
                throw new ArgumentException(String.Format("query.NumberOfPeriods '{0}' is < 1"));
        }

        #endregion
    }
}
