using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Analytics.Data;
using TOne.WhS.Analytics.Entities;
using TOne.WhS.BusinessEntity.Business;
using Vanrise.Common;
using Vanrise.Entities;

namespace TOne.WhS.Analytics.Business
{
    public class VariationReportManager
    {
        #region Fields

        CarrierAccountManager _carrierAccountManager = new CarrierAccountManager();
        SaleZoneManager _saleZoneManager = new SaleZoneManager();
        
        #endregion

        #region Public Methods

        public IDataRetrievalResult<VariationReportRecord> GetFilteredVariationReportRecords(Vanrise.Entities.DataRetrievalInput<VariationReportQuery> input)
        {
            ValidateVariationReportQuery(input.Query);
            IVariationReportDataManager dataManager = AnalyticsDataManagerFactory.GetDataManager<IVariationReportDataManager>();
            VariationReportBigResult variationReportBigResult = dataManager.GetFilteredVariationReportRecords(input);
            SetDimensionNames(input.Query.ReportType, variationReportBigResult.Data);
            return DataRetrievalManager.Instance.ProcessResult(input, variationReportBigResult);
        }
        
        #endregion

        #region Private Methods

        void ValidateVariationReportQuery(VariationReportQuery query)
        {
            if (!Enum.IsDefined(typeof(VariationReportType), query.ReportType))
                throw new ArgumentException(String.Format("query.ReportType '{0}' is invalid", query.ReportType));
            if (query.ToDate == null)
                throw new ArgumentNullException(String.Format("query.ToDate"));
            if (!Enum.IsDefined(typeof(VariationReportTimePeriod), query.TimePeriod))
                throw new ArgumentException(String.Format("query.TimePeriod '{0}' is invalid", query.TimePeriod));
            if (query.NumberOfPeriods < 1)
                throw new ArgumentException(String.Format("query.NumberOfPeriods '{0}' is < 1"));
        }

        void SetDimensionNames(VariationReportType reportType, IEnumerable<VariationReportRecord> reportRecords)
        {
            switch (reportType)
            {
                case VariationReportType.InBoundMinutes:
                case VariationReportType.OutBoundMinutes:
                case VariationReportType.InOutBoundMinutes:
                case VariationReportType.InBoundAmount:
                case VariationReportType.OutBoundAmount:
                case VariationReportType.InOutBoundAmount:
                    foreach (VariationReportRecord reportRecord in reportRecords)
                        reportRecord.DimensionName = String.Format("{0} {1}", _carrierAccountManager.GetCarrierAccountName((int)reportRecord.DimensionId), reportRecord.DimensionSuffix);
                    break;
                default:
                    foreach (VariationReportRecord reportRecord in reportRecords)
                        reportRecord.DimensionName = String.Format("{0}{1}", _saleZoneManager.GetSaleZoneName((long)reportRecord.DimensionId), reportRecord.DimensionSuffix);
                    break;
            }
        }
        
        #endregion
    }
}
