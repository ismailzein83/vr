﻿using System;
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
            IBPValidationMessageDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPValidationMessageDataManager>();
            Vanrise.Entities.BigResult<BPValidationMessageDetail> bigResult = dataManager.GetFilteredBPValidationMessage(input);

            BPValidationMessageBigResult bpValidationMessageBigResult = new BPValidationMessageBigResult()
            {
                ResultKey = bigResult.ResultKey,
                Data = bigResult.Data,
                TotalCount = bigResult.TotalCount
            };
            ResultProcessingHandler<BPValidationMessageDetail> handler = new ResultProcessingHandler<BPValidationMessageDetail>()
            {
                ExportExcelHandler = new BPValidationMessageDetailExportExcelHandler()
            };

            if (bigResult.Data != null && bigResult.Data.Count() > 0)
                bpValidationMessageBigResult.HasWarningMessages = bigResult.Data.Any(x => x.Entity.Severity == ActionSeverity.Warning);

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, bpValidationMessageBigResult, handler);
        }

        private BPValidationMessageDetail BPValidationMessageDetailMapper(BPValidationMessage bpValidationMessage)
        {
            if (bpValidationMessage == null)
                return null;
            return new BPValidationMessageDetail()
            {
                Entity = bpValidationMessage
            };
        }

        private class BPValidationMessageDetailExportExcelHandler : ExcelExportHandler<BPValidationMessageDetail>
        {
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<BPValidationMessageDetail> context)
            {
                ExportExcelSheet sheet = new ExportExcelSheet()
                {
                    SheetName = "Messages Validations",
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() }
                };

                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Message",Width=200 });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Severity"});

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
    }
}
