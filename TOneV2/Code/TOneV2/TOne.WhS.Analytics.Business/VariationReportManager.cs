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
            IVariationReportDataManager dataManager = AnalyticsDataManagerFactory.GetDataManager<IVariationReportDataManager>();
            VariationReportBigResult variationReportBigResult = dataManager.GetFilteredVariationReportRecords(input);
            SetDimensionNames(input.Query.ReportType, variationReportBigResult.Data);
            return DataRetrievalManager.Instance.ProcessResult(input, variationReportBigResult);
        }
        
        #endregion

        #region Private Methods

        void SetDimensionNames(VariationReportType reportType, IEnumerable<VariationReportRecord> reportRecords)
        {
            switch (reportType)
            {
                case VariationReportType.InBoundMinutes:
                case VariationReportType.OutBoundMinutes:
                    foreach (VariationReportRecord reportRecord in reportRecords)
                        reportRecord.DimensionName = _carrierAccountManager.GetCarrierAccountName((int)reportRecord.DimensionId);
                    break;
                case VariationReportType.TopDestinationMinutes:
                    foreach (VariationReportRecord reportRecord in reportRecords)
                        reportRecord.DimensionName = _saleZoneManager.GetSaleZoneName((long)reportRecord.DimensionId);
                    break;
            }
        }
        
        #endregion
    }
}
