using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Data;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace Vanrise.BusinessProcess
{
    public class BPValidationMessageManager
    {
        public void Insert(IEnumerable<BPValidationMessage> messages)
        {
            IBPValidationMessageDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPValidationMessageDataManager>();
            dataManager.Insert(messages);
        }

        public List<BPValidationMessageDetail> GetBeforeId(BPValidationMessageBeforeIdInput input)
        {
            IBPValidationMessageDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPValidationMessageDataManager>();

            List<BPValidationMessage> bpValidationMessages = dataManager.GetBeforeId(input);
            List<BPValidationMessageDetail> bpValidationMessageDetails = new List<BPValidationMessageDetail>();
            foreach (BPValidationMessage bpValidationMessage in bpValidationMessages)
            {
                bpValidationMessageDetails.Add(BPValidationMessageDetailMapper(bpValidationMessage));
            }
            return bpValidationMessageDetails;
        }

        public BPValitaionMessageUpdateOutput GetUpdated(BPValidationMessageUpdateInput input)
        {
            BPValitaionMessageUpdateOutput bpValitaionMessageUpdateOutput = new BPValitaionMessageUpdateOutput();

            IBPValidationMessageDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPValidationMessageDataManager>();

            List<BPValidationMessage> bpValidationMessages = dataManager.GetUpdated(input);
            List<BPValidationMessageDetail> bpValidationMessageDetails = new List<BPValidationMessageDetail>();
            foreach (BPValidationMessage bpValidationMessage in bpValidationMessages)
            {
                bpValidationMessageDetails.Add(BPValidationMessageDetailMapper(bpValidationMessage));
            }

            bpValitaionMessageUpdateOutput.ListValidationMessageDetails = bpValidationMessageDetails;
            return bpValitaionMessageUpdateOutput;
        }

        public Vanrise.Entities.IDataRetrievalResult<BPValidationMessageDetail> GetFilteredBPValidationMessage(Vanrise.Entities.DataRetrievalInput<BPValidationMessageQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new BPValidationMessageRequestHandler());
        }

        #region private Class
        private class BPValidationMessageRequestHandler : BigDataRequestHandler<BPValidationMessageQuery, BPValidationMessage, BPValidationMessageDetail>
        {
            public override BPValidationMessageDetail EntityDetailMapper(BPValidationMessage entity)
            {
                BPValidationMessageManager manager = new BPValidationMessageManager();
                return manager.BPValidationMessageDetailMapper(entity);
            }

            public override IEnumerable<BPValidationMessage> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<BPValidationMessageQuery> input)
            {
                IBPValidationMessageDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPValidationMessageDataManager>();
                return dataManager.GetFilteredBPValidationMessage(input.Query);
            }

            protected override BigResult<BPValidationMessageDetail> AllRecordsToBigResult(DataRetrievalInput<BPValidationMessageQuery> input, IEnumerable<BPValidationMessage> allRecords)
            {
                var defaultBigResult = base.AllRecordsToBigResult(input, allRecords);
                BPValidationMessageBigResult bpValidationMessageBigResult = new BPValidationMessageBigResult()
                {
                    ResultKey = defaultBigResult.ResultKey,
                    Data = defaultBigResult.Data,
                    TotalCount = defaultBigResult.TotalCount
                };
                if (allRecords.Count() > 0)
                    bpValidationMessageBigResult.HasWarningMessages = allRecords.Any(x => x.Severity == ActionSeverity.Warning);
                return bpValidationMessageBigResult;
            }

            protected override ResultProcessingHandler<BPValidationMessageDetail> GetResultProcessingHandler(DataRetrievalInput<BPValidationMessageQuery> input, BigResult<BPValidationMessageDetail> bigResult)
            {
                return new ResultProcessingHandler<BPValidationMessageDetail>
                {
                    ExportExcelHandler = new VRBPValidationMessageExcelExportHandler(input.Query)
                };
            }
        }

        private class VRBPValidationMessageExcelExportHandler : ExcelExportHandler<BPValidationMessageDetail>
        {
            BPValidationMessageQuery _query;
            public VRBPValidationMessageExcelExportHandler(BPValidationMessageQuery query)
            {
                if (query == null)
                    throw new ArgumentNullException("query");
                _query = query;
            }
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<BPValidationMessageDetail> context)
            {
                ExportExcelSheet sheet = new ExportExcelSheet()
                {
                    SheetName = "Messages Validations",
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() }
                };

                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Message", Width = 200 });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Severity" });

                sheet.Rows = new List<ExportExcelRow>();
                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    foreach (var record in context.BigResult.Data)
                    {
                        if (record.Entity != null)
                        {
                            var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                            sheet.Rows.Add(row);
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.Message });
                            row.Cells.Add(new ExportExcelCell { Value = Common.Utilities.GetEnumDescription(record.Entity.Severity) });
                        }
                    }
                }
                context.MainSheet = sheet;
            }
        }
        #endregion

        #region private Method
        private BPValidationMessageDetail BPValidationMessageDetailMapper(BPValidationMessage bpValidationMessage)
        {
            if (bpValidationMessage == null)
                return null;
            return new BPValidationMessageDetail()
            {
                Entity = bpValidationMessage
            };
        }
        #endregion
    }
}
