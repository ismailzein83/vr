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
using Vanrise.Security.Entities;

namespace TOne.WhS.Analytics.Business
{
    public class VariationReportManager
    {
        AnalyticHelper _analyticHelper = new AnalyticHelper();

        #region Public Methods

        public IDataRetrievalResult<VariationReportRecord> GetFilteredVariationReportRecords(Vanrise.Entities.DataRetrievalInput<VariationReportQuery> input)
        {
            ValidateVariationReportQuery(input.Query);


            return BigDataManager.Instance.RetrieveData(input, new VariationReportRequestHandler()
            {
                Query = input.Query
            });
        }


        public bool DoesUserHaveVariationReportViewAccess(VariationReportType type)
        {
            int userId = ContextFactory.GetContext().GetLoggedInUserId();
            switch (type)
            {
                case VariationReportType.InBoundAmount:
                case VariationReportType.OutBoundAmount:
                case VariationReportType.InOutBoundAmount:
                case VariationReportType.OutBoundProfit:
                case VariationReportType.Profit:
                case VariationReportType.TopDestinationAmount:
                case VariationReportType.TopDestinationProfit:
                    return _analyticHelper.DoesUserHaveBillingViewAccess(userId);

                case VariationReportType.InBoundMinutes:
                case VariationReportType.OutBoundMinutes:
                case VariationReportType.TopDestinationMinutes:
                case VariationReportType.InOutBoundMinutes:
                    return _analyticHelper.DoesUserHaveTrafficViewAccess(userId);

                default:
                    return false;
            }
        }
        #endregion

        #region Private Classes

        class VariationReportRequestHandler : BigDataRequestHandler<VariationReportQuery, VariationReportRecord, VariationReportRecord>
        {
            #region Fields / Constructors

            public VariationReportQuery Query { get; set; }
            public CarrierAccountManager CarrierAccountManager = new CarrierAccountManager();
            public CarrierProfileManager CarrierProfileManager = new CarrierProfileManager();
            public SaleZoneManager SaleZoneManager = new SaleZoneManager();
            public List<TimePeriod> TimePeriods;

            public VariationReportRequestHandler()
            {
            }

            #endregion

            protected override BigResult<VariationReportRecord> AllRecordsToBigResult(DataRetrievalInput<VariationReportQuery> input, IEnumerable<VariationReportRecord> allRecords)
            {
                var variationReportBigResult = new VariationReportBigResult();

                if (input.Query.ParentDimensions == null)
                    variationReportBigResult.Summary = GetSummary(allRecords, input.Query.NumberOfPeriods);

                BigResult<VariationReportRecord> bigResult = allRecords.ToBigResult(input, null, this.EntityDetailMapper);

                variationReportBigResult.ResultKey = bigResult.ResultKey;
                variationReportBigResult.Data = bigResult.Data;
                variationReportBigResult.TotalCount = bigResult.TotalCount;

                variationReportBigResult.DimensionTitle = GetDimensionTitle();
                variationReportBigResult.TimePeriods = TimePeriods;
                variationReportBigResult.DrillDownDimensions = GetDrillDownDimensions();

                return variationReportBigResult;
            }

            public override IEnumerable<VariationReportRecord> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<VariationReportQuery> input)
            {
                Query = input.Query;
                TimePeriods = GetTimePeriods();
                IVariationReportDataManager dataManager = AnalyticsDataManagerFactory.GetDataManager<IVariationReportDataManager>();
                return dataManager.GetFilteredVariationReportRecords(input, TimePeriods);
            }

            protected override ResultProcessingHandler<VariationReportRecord> GetResultProcessingHandler(DataRetrievalInput<VariationReportQuery> input, BigResult<VariationReportRecord> bigResult)
            {
                var resultProcessingHandler = new ResultProcessingHandler<VariationReportRecord>()
                {
                    ExportExcelHandler = new VariationReportRecordExportExcelHandler()
                };
                return resultProcessingHandler;
            }

            public override VariationReportRecord EntityDetailMapper(VariationReportRecord entity)
            {
                SetVariationReportRecordDimension(entity);

                switch (Query.ReportType)
                {
                    case VariationReportType.TopDestinationMinutes:
                    case VariationReportType.TopDestinationAmount:
                    case VariationReportType.TopDestinationProfit:
                        entity.DimensionName = SaleZoneManager.GetSaleZoneName((long)entity.DimensionId);
                        break;
                    default:
                        entity.DimensionName = (Query.GroupByProfile) ?
                            CarrierProfileManager.GetCarrierProfileName((int)entity.DimensionId) :
                            CarrierAccountManager.GetCarrierAccountName((int)entity.DimensionId);
                        break;
                }

                if (entity.DimensionSuffix != VariationReportRecordDimensionSuffix.None)
                    entity.DimensionName = String.Format("{0}{1}", entity.DimensionName, String.Format(" / {0}", entity.DimensionSuffix));

                return entity;
            }

            #region Private Methods

            string GetDimensionTitle()
            {
                string dimensionTitle = null;
                switch (Query.ReportType)
                {
                    case VariationReportType.InBoundMinutes:
                    case VariationReportType.InBoundAmount:
                    case VariationReportType.Profit:
                        dimensionTitle = "Customer";
                        break;

                    case VariationReportType.OutBoundMinutes:
                    case VariationReportType.OutBoundAmount:
                    case VariationReportType.OutBoundProfit:
                        dimensionTitle = "Supplier";
                        break;

                    case VariationReportType.TopDestinationMinutes:
                    case VariationReportType.TopDestinationAmount:
                    case VariationReportType.TopDestinationProfit:
                        dimensionTitle = "Zone";
                        break;

                    case VariationReportType.InOutBoundMinutes:
                    case VariationReportType.InOutBoundAmount:
                        dimensionTitle = "Carrier";
                        break;
                }
                return dimensionTitle;
            }

            #region Time Periods

            List<TimePeriod> GetTimePeriods()
            {
                var timePeriods = new List<TimePeriod>();

                DateTime toDate = Query.ToDate.Date.AddDays(1); // Ignore ToDate's time and add 1 day to include it in the result
                string dateFormat = "dd/MM/yyyy";

                for (int i = 0; i < Query.NumberOfPeriods; i++)
                {
                    DateTime fromDate = GetFromDate(toDate);
                    timePeriods.Add(new TimePeriod()
                    {
                        PeriodDescription = (Query.TimePeriod == VariationReportTimePeriod.Daily) ?
                            fromDate.ToString(dateFormat) : String.Format("{0} - {1}", fromDate.ToString(dateFormat), toDate.AddDays(-1).ToString(dateFormat)),
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
                switch (Query.TimePeriod)
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
                switch (Query.ReportType)
                {
                    case VariationReportType.InBoundMinutes:
                    case VariationReportType.InBoundAmount:
                    case VariationReportType.Profit:
                        variationReportRecord.Dimension = VariationReportDimension.Customer;
                        break;
                    case VariationReportType.OutBoundMinutes:
                    case VariationReportType.OutBoundAmount:
                    case VariationReportType.OutBoundProfit:
                        variationReportRecord.Dimension = VariationReportDimension.Supplier;
                        break;
                    case VariationReportType.TopDestinationMinutes:
                    case VariationReportType.TopDestinationAmount:
                    case VariationReportType.TopDestinationProfit:
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
                switch (Query.ReportType)
                {
                    case VariationReportType.InOutBoundMinutes:
                    case VariationReportType.InOutBoundAmount:
                    case VariationReportType.Profit:
                        drillDownDimensions.Add(VariationReportDimension.Zone);
                        break;

                    case VariationReportType.TopDestinationProfit:
                        drillDownDimensions.Add(VariationReportDimension.Supplier);
                        break;

                    case VariationReportType.InBoundMinutes:
                    case VariationReportType.InBoundAmount:
                    case VariationReportType.OutBoundMinutes:
                    case VariationReportType.OutBoundAmount:
                        if (Query.ParentDimensions == null)
                            drillDownDimensions.Add(VariationReportDimension.Zone);
                        break;

                    case VariationReportType.TopDestinationMinutes:
                    case VariationReportType.TopDestinationAmount:
                        if (Query.ParentDimensions == null)
                        {
                            drillDownDimensions.Add(VariationReportDimension.Customer);
                            drillDownDimensions.Add(VariationReportDimension.Supplier);
                        }
                        else
                        {
                            ParentDimension directParentDimension = Query.ParentDimensions.ElementAt(Query.ParentDimensions.Count() - 1);
                            if (directParentDimension.Dimension == VariationReportDimension.Customer)
                                drillDownDimensions.Add(VariationReportDimension.Supplier);
                        }
                        break;
                }
                return (drillDownDimensions.Count > 0) ? drillDownDimensions : null;
            }

            #region Summary

            VariationReportRecord GetSummary(IEnumerable<VariationReportRecord> data, int numberOfPeriods)
            {
                var summary = new VariationReportRecord();
                summary.TimePeriodValues = new List<decimal>();

                for (int i = 0; i < numberOfPeriods; i++)
                    summary.TimePeriodValues.Add(0);

                foreach (VariationReportRecord record in data)
                {
                    summary.Average += record.Average;
                    for (int i = 0; i < record.TimePeriodValues.Count; i++)
                        summary.TimePeriodValues[i] += record.TimePeriodValues[i];
                }

                summary.Percentage = (summary.TimePeriodValues[0] - summary.Average) / GetDenominator(summary.Average) * 100;
                summary.PreviousPeriodPercentage = (summary.TimePeriodValues[0] - summary.TimePeriodValues[1]) / GetDenominator(summary.TimePeriodValues[1]) * 100;

                return summary;
            }

            decimal GetDenominator(decimal average)
            {
                return (average > 0) ? average : Decimal.MaxValue;
            }

            #endregion

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

        private class VariationReportRecordExportExcelHandler : ExcelExportHandler<VariationReportRecord>
        {
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<VariationReportRecord> context)
            {
                if (context.BigResult == null || context.BigResult.Data == null)
                    return;

                var sheet = new ExportExcelSheet()
                {
                    SheetName = "Variation Report",
                    Header = new ExportExcelHeader() { Cells = new List<ExportExcelHeaderCell>() }
                };

                var result = context.BigResult as VariationReportBigResult;

                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = result.DimensionTitle });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Periods AVG" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Periods %" });


                int i = 0;

                do
                {
                    if (i == 1){
                        sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Previous Period %" });
                        var period = result.TimePeriods[i];
                        sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = period.PeriodDescription, Width = 40 });
                    }
                        
                    else
                    {
                        var period = result.TimePeriods[i];
                        sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = period.PeriodDescription, Width = 40 });
                    }
                    i++;
                } while (i < result.TimePeriods.Count());


                sheet.Rows = new List<ExportExcelRow>();
                foreach (var record in context.BigResult.Data)
                {
                    var row = new ExportExcelRow() { Cells = new List<ExportExcelCell>() };
                    row.Cells.Add(new ExportExcelCell() { Value = record.DimensionName });
                    row.Cells.Add(new ExportExcelCell() { Value = record.Average });
                    row.Cells.Add(new ExportExcelCell() { Value = record.Percentage });
                    int j = 0;
                    do
                    {
                        if (j == 1){
                            row.Cells.Add(new ExportExcelCell() { Value = record.PreviousPeriodPercentage });
                            var value = record.TimePeriodValues[j];
                            row.Cells.Add(new ExportExcelCell() { Value = value });
                        }
                        else
                        {
                            var value = record.TimePeriodValues[j];
                            row.Cells.Add(new ExportExcelCell() { Value = value });
                        }
                        j++;
                    } while (j < record.TimePeriodValues.Count());

                    sheet.Rows.Add(row);
                }

                var emptyRow = new ExportExcelRow() { Cells = new List<ExportExcelCell>() };
                for (int k = 0; k < result.TimePeriods.Count() + 4; k++)
                    emptyRow.Cells.Add(new ExportExcelCell() { Value = "" });
                sheet.Rows.Add(emptyRow);

                var totalRow = new ExportExcelRow() { Cells = new List<ExportExcelCell>() };
                var summary = ((TOne.WhS.Analytics.Entities.VariationReportBigResult) (context.BigResult)).Summary;
                if (summary != null)
                {
                    totalRow.Cells.Add(new ExportExcelCell()
                    {
                        Value = "Total"
                    });

                    totalRow.Cells.Add(new ExportExcelCell()
                    {
                        Value = summary.Average
                    });

                    totalRow.Cells.Add(new ExportExcelCell()
                    {
                        Value = summary.Percentage
                    });

                    int totalCount = 0;
                    do
                    {
                        if (totalCount == 1)
                        {
                            totalRow.Cells.Add(new ExportExcelCell() { Value = summary.PreviousPeriodPercentage });
                            var value = summary.TimePeriodValues[totalCount];
                            totalRow.Cells.Add(new ExportExcelCell() { Value = value });
                        }
                        else
                        {
                            var value = summary.TimePeriodValues[totalCount];
                            totalRow.Cells.Add(new ExportExcelCell() { Value = value });
                        }
                        totalCount++;
                    } while (totalCount < summary.TimePeriodValues.Count());
                    sheet.Rows.Add(totalRow);
                }
             
                context.MainSheet = sheet;
            }
        }


        #endregion

    }
}
