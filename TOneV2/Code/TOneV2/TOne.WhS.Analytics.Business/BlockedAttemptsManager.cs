using System;
using System.Collections.Generic;
using TOne.WhS.Analytics.Data;
using TOne.WhS.Analytics.Entities;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
namespace TOne.WhS.Analytics.Business
{
    public class BlockedAttemptsManager
    {
        private readonly CarrierAccountManager _carrierAccountManager;
        private readonly SaleZoneManager _saleZoneManager;
        private readonly SwitchReleaseCauseManager _switchReleaseCauseManager;


        public BlockedAttemptsManager()
        {
            _carrierAccountManager = new CarrierAccountManager();
            _saleZoneManager = new SaleZoneManager();
            _switchReleaseCauseManager = new SwitchReleaseCauseManager();
        }

        public Vanrise.Entities.IDataRetrievalResult<BlockedAttemptDetail> GetBlockedAttemptData(Vanrise.Entities.DataRetrievalInput<BlockedAttemptQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new BlockedAttemptRequestHandler());
        }


        public BlockedAttemptDetail BlockedAttemptDetailMapper(BlockedAttempt blockedAttempt, int? switchId)
        {
            BlockedAttemptDetail blockedAttemptDetail = new BlockedAttemptDetail
            {
                Entity = blockedAttempt,
                CustomerName = _carrierAccountManager.GetCarrierAccountName(blockedAttempt.CustomerID),
                SaleZoneName = _saleZoneManager.GetSaleZoneName(blockedAttempt.SaleZoneID),
                ReleaseCodeDescription = _switchReleaseCauseManager.GetReleaseCodeDescription(blockedAttempt.ReleaseCode, switchId)

            };
            return blockedAttemptDetail;
        }


        #region Private Classes

        private class BlockedAttemptRequestHandler : BigDataRequestHandler<BlockedAttemptQuery, BlockedAttempt, BlockedAttemptDetail>
        {
            BlockedAttemptsManager _manager = new BlockedAttemptsManager();
            public override BlockedAttemptDetail EntityDetailMapper(BlockedAttempt entity)
            {
                throw new NotImplementedException();
            }

            public override IEnumerable<BlockedAttempt> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<BlockedAttemptQuery> input)
            {
                IBlockedAttemptDataManager dataManager = AnalyticsDataManagerFactory.GetDataManager<IBlockedAttemptDataManager>();
                return dataManager.GetAllFilteredBlockedAttempts(input);
            }

            protected override ResultProcessingHandler<BlockedAttemptDetail> GetResultProcessingHandler(DataRetrievalInput<BlockedAttemptQuery> input, BigResult<BlockedAttemptDetail> bigResult)
            {
                return new ResultProcessingHandler<BlockedAttemptDetail>
                {
                    ExportExcelHandler = new BlockedAttemptsExcelExportHandler()
                };
            }

            protected override BigResult<BlockedAttemptDetail> AllRecordsToBigResult(DataRetrievalInput<BlockedAttemptQuery> input, IEnumerable<BlockedAttempt> allRecords)
            {
                int? switchId = null;
                if (input.Query.Filter != null && input.Query.Filter.SwitchIds != null && input.Query.Filter.SwitchIds.Count >= 1)
                    switchId = input.Query.Filter.SwitchIds[0];
                return allRecords.ToBigResult(input, null, (entity) => _manager.BlockedAttemptDetailMapper(entity, switchId));
            }
        }

        private class BlockedAttemptsExcelExportHandler : ExcelExportHandler<BlockedAttemptDetail>
        {
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<BlockedAttemptDetail> context)
            {
                ExportExcelSheet sheet = new ExportExcelSheet()
                {
                    SheetName = "Blocked Attempts",
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() }
                };

                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Customer", Width = 50 });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Sale Zone", Width = 40 });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Blocked Attempts" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Release Code" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Release Source" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "First Attempt" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Last Attempt" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Caller Number" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Dialed Number" });

                sheet.Rows = new List<ExportExcelRow>();
                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    foreach (var record in context.BigResult.Data)
                    {
                        if (record.Entity != null)
                        {
                            var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                            sheet.Rows.Add(row);
                            row.Cells.Add(new ExportExcelCell { Value = record.CustomerName });
                            row.Cells.Add(new ExportExcelCell { Value = record.SaleZoneName });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.BlockedAttempts });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.ReleaseCode });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.ReleaseSource });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.FirstAttempt });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.LastAttempt });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.CGPN });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.CDPN });
                        }
                    }
                }
                context.MainSheet = sheet;
            }
        }

        #endregion

    }

}
