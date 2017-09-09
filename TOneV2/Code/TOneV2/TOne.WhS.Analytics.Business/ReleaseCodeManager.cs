using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Analytics.Data;
using TOne.WhS.Analytics.Entities;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Common.Business;

namespace TOne.WhS.Analytics.Business
{
    public class ReleaseCodeManager
    {
        private readonly CarrierAccountManager _carrierAccountManager;
        private readonly SaleZoneManager _saleZoneManager;
        private readonly SwitchManager _switchManager;
        private readonly SwitchReleaseCodeManager _switchReleaseCodeManager;

      
        public ReleaseCodeManager()
        {
            _carrierAccountManager = new CarrierAccountManager();
            _saleZoneManager = new SaleZoneManager();
            _switchManager = new SwitchManager();
            _switchReleaseCodeManager = new SwitchReleaseCodeManager();
        }
        public Vanrise.Entities.IDataRetrievalResult<ReleaseCodeStatDetail> GetAllFilteredReleaseCodes(Vanrise.Entities.DataRetrievalInput<ReleaseCodeQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new ReleaseCodeRequestHandler());
        }

        public ReleaseCodeStatDetail ReleaseCodeDetailMapper(ReleaseCodeStat releaseCode)
        {
            
            string supplierName = _carrierAccountManager.GetCarrierAccountName(releaseCode.SupplierId);
            string switchName = _switchManager.GetSwitchName(releaseCode.SwitchId);
            string zoneName = _saleZoneManager.GetSaleZoneName(releaseCode.MasterPlanZoneId);
            string releaseCodeDescription = _switchReleaseCodeManager.GetReleaseCodeDescription(releaseCode.ReleaseCode);
            ReleaseCodeStatDetail releaseCodeDetail = new ReleaseCodeStatDetail
            {
                Entity = releaseCode,
                SupplierName = supplierName,
                SwitchName = switchName,
                ZoneName = zoneName,
                ReleaseCodeDescription = releaseCodeDescription
            };
            return releaseCodeDetail;
        }

        #region Private Classes

        private class ReleaseCodeRequestHandler : BigDataRequestHandler<ReleaseCodeQuery, ReleaseCodeStat, ReleaseCodeStatDetail>
        {
            public override ReleaseCodeStatDetail EntityDetailMapper(ReleaseCodeStat entity)
            {
                ReleaseCodeManager manager = new ReleaseCodeManager();
                return manager.ReleaseCodeDetailMapper(entity);
            }

            public override IEnumerable<ReleaseCodeStat> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<ReleaseCodeQuery> input)
            {
                IReleaseCodeDataManager dataManager = AnalyticsDataManagerFactory.GetDataManager<IReleaseCodeDataManager>();
                SaleCodeManager manager = new SaleCodeManager();
               // List<string> salecodesIds = null;
                //if (input.Query.Filter.CountryIds != null && input.Query.Filter.CountryIds.Count() > 0)
                //{
                //    List<SaleCode> salecodes = manager.GetSaleCodesByCodeGroups(input.Query.Filter.CountryIds);
                //    salecodesIds = salecodes.Select(x => x.Code).ToList();
                //}
                

                return dataManager.GetAllFilteredReleaseCodes(input);
            }

            protected override ResultProcessingHandler<ReleaseCodeStatDetail> GetResultProcessingHandler(DataRetrievalInput<ReleaseCodeQuery> input, BigResult<ReleaseCodeStatDetail> bigResult)
            {
                return new ResultProcessingHandler<ReleaseCodeStatDetail>
                {
                    ExportExcelHandler = new ReleaseCodeExcelExportHandler(input.Query)
                };
            }
        }

        private class ReleaseCodeExcelExportHandler : ExcelExportHandler<ReleaseCodeStatDetail>
        {
            ReleaseCodeQuery _query;
            public ReleaseCodeExcelExportHandler(ReleaseCodeQuery query)
            {
                if (query == null)
                    throw new ArgumentNullException("query");
                _query = query;
            }
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<ReleaseCodeStatDetail> context)
            {
                ExportExcelSheet sheet = new ExportExcelSheet();
                sheet.Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() };

                if (_query.Filter.Dimession != null && _query.Filter.Dimession.Count() > 0)
                {
                    if (_query.Filter.Dimession.Contains(ReleaseCodeDimension.Supplier))
                    {
                        sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Supplier" });
                    }
                    if (_query.Filter.Dimession.Contains(ReleaseCodeDimension.MasterZone))
                    {
                        sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Master Zone" });
                    }
                    if (_query.Filter.Dimession.Contains(ReleaseCodeDimension.PortIn))
                    {
                        sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Port In" });
                    }
                    if (_query.Filter.Dimession.Contains(ReleaseCodeDimension.PortOut))
                    {
                        sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Port Out" });
                    }
                }

                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Release Code" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Release Source" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Switch" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Attempts" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Failed Attempts" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "%" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Duration (min)" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "First Attempt", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.DateTime });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Last Attempt", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.DateTime });
                sheet.Rows = new List<ExportExcelRow>();
                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    foreach (var record in context.BigResult.Data)
                    {
                        if (record.Entity != null)
                        {
                            var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                            sheet.Rows.Add(row);

                            if (_query.Filter.Dimession != null && _query.Filter.Dimession.Count() > 0)
                            {
                                if (_query.Filter.Dimession.Contains(ReleaseCodeDimension.Supplier))
                                {
                                    row.Cells.Add(new ExportExcelCell { Value = record.SupplierName });
                                }
                                if (_query.Filter.Dimession.Contains(ReleaseCodeDimension.MasterZone))
                                {
                                    row.Cells.Add(new ExportExcelCell { Value = record.ZoneName });
                                }
                                if (_query.Filter.Dimession.Contains(ReleaseCodeDimension.PortIn))
                                {
                                    row.Cells.Add(new ExportExcelCell { Value = record.Entity.PortIn });
                                }
                                if (_query.Filter.Dimession.Contains(ReleaseCodeDimension.PortOut))
                                {
                                    row.Cells.Add(new ExportExcelCell { Value = record.Entity.PortOut });
                                }
                            }
                            
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.ReleaseCode });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.ReleaseSource });
                            row.Cells.Add(new ExportExcelCell { Value = record.SwitchName });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.Attempt });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.FailedAttempt });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.Percentage });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.DurationInMinutes });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.FirstAttempt });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.LastAttempt });
                        }
                    }
                }
                context.MainSheet = sheet;
            }
        }

        #endregion
    }
}
