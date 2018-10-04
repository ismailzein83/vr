﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.SupplierPriceList.Data;
using TOne.WhS.SupplierPriceList.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Common.Excel;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class ReplyMapping
    {
        public ReceivedPricelistStatus Status { get; set; }
        public bool Send { get; set; }
        public Guid MailTemplateId { get; set; }
        public bool AttachFile { get; set; }
    }

    public class ReceivedSupplierPricelistManager
    {

        #region Public Methods
        public IDataRetrievalResult<ReceivedPricelistDetail> GetFilteredReceivedPricelists(DataRetrievalInput<ReceivedPricelistQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new ReceivedPricelistRequestHandler());
        }

        public ReceivedPricelist GetReceivedPricelistById(int id)
        {
            IReceivedPricelistManager receivedPricelistDataManager = SupPLDataManagerFactory.GetDataManager<IReceivedPricelistManager>();
            return receivedPricelistDataManager.GetReceivedPricelistById(id);
        }

        public UpdateOperationOutput<ReceivedPricelistDetail> SetReceivedPricelistAsCompleted(ReceivedPricelistDetail receivedPricelistDetail)
        {
            UpdateOperationOutput<ReceivedPricelistDetail> updateOperationOutput = new UpdateOperationOutput<ReceivedPricelistDetail>();
            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IReceivedPricelistManager receivedPricelistDataManager = SupPLDataManagerFactory.GetDataManager<IReceivedPricelistManager>();
            bool updateActionSucc = receivedPricelistDataManager.SetReceivedPricelistAsCompletedManualy(receivedPricelistDetail.ReceivedPricelist.Id, ReceivedPricelistStatus.ImportedManually);
            if (updateActionSucc)
            {
                SendMailToSupplier(receivedPricelistDetail.ReceivedPricelist.Id, AutoImportEmailTypeEnum.Succeeded);
                SendMailToInternal(receivedPricelistDetail.ReceivedPricelist.Id, AutoImportEmailTypeEnum.Succeeded);
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = ReceivedPriceListDetailMapper(this.GetReceivedPricelistById(receivedPricelistDetail.ReceivedPricelist.Id));
            }
            return updateOperationOutput;
        }


        public void SendMailToSupplier(int receivedPricelistRecordId, AutoImportEmailTypeEnum emailType)
        {
            VRFileManager fileManager = new VRFileManager();
            VRFile receivedPricelistFile = null;
            var receivedPricelist = GetReceivedPricelistById(receivedPricelistRecordId);
            if (receivedPricelist == null)
                throw new VRBusinessException(string.Format("There is no received pricelist with Id '{0}'.", receivedPricelistRecordId));

            var supplierMailAttachements = new List<VRMailAttachement>();

            SupplierAutoImportTemplate supplierAutoImportTemplate = new BusinessEntity.Business.ConfigManager().GetSupplierAutoImportTemplateByType(emailType);
            supplierAutoImportTemplate.ThrowIfNull(string.Format("There is no mail template selected for type: '{0}'", Vanrise.Common.Utilities.GetEnumDescription(emailType)));
            if (supplierAutoImportTemplate.AttachPricelist && receivedPricelist.FileId.HasValue)
            {
                receivedPricelistFile = fileManager.GetFile(receivedPricelist.FileId.Value);
                receivedPricelistFile.ThrowIfNull(string.Format("There is no file with Id '{0}'.", receivedPricelist.FileId.Value));
                supplierMailAttachements.Add(new VRMailAttachmentExcel { Name = receivedPricelistFile.Name, Content = receivedPricelistFile.Content });
            }

            var objects = new Dictionary<string, dynamic>
            {
                {"Received Pricelist", receivedPricelist}
            };

            if ((int)receivedPricelist.Status > 65 && receivedPricelist.MessageDetails != null && receivedPricelist.MessageDetails.Count > 0)
            {
                var excelFile = new VRExcelFile();
                var errorSheet = CreateExcelErrorSheet(receivedPricelist);
                if (errorSheet != null)
                    excelFile.AddSheet(errorSheet);
                supplierMailAttachements.Add(new VRMailAttachmentExcel { Name = "Errors.xls", Content = excelFile.GenerateExcelFile() });
            }

            VRMailManager vrMailManager = new VRMailManager();
            var supplierEvaluatedObj = vrMailManager.EvaluateMailTemplate(supplierAutoImportTemplate.EmailTemplateId, objects);
            vrMailManager.SendMail(supplierEvaluatedObj.From, supplierEvaluatedObj.To, supplierEvaluatedObj.CC, supplierEvaluatedObj.BCC, supplierEvaluatedObj.Subject, supplierEvaluatedObj.Body, supplierMailAttachements, false);
            UpdateSentToSupplierStatus(receivedPricelistRecordId, true);
        }

        public void SendMailToInternal(int receivedPricelistRecordId, AutoImportEmailTypeEnum emailType)
        {
            VRFileManager fileManager = new VRFileManager();
            VRFile receivedPricelistFile = null;
            var receivedPricelist = GetReceivedPricelistById(receivedPricelistRecordId);
            receivedPricelist.ThrowIfNull(string.Format("There is no received pricelist with Id '{0}'.", receivedPricelistRecordId));

            var internalMailAttachements = new List<VRMailAttachement>();

            InternalAutoImportTemplate internalAutoImportTemplate = new BusinessEntity.Business.ConfigManager().GetInternalAutoImportTemplateByType(emailType);
            if (internalAutoImportTemplate == null || internalAutoImportTemplate.EmailTemplateId == null || internalAutoImportTemplate.EmailTemplateId == Guid.Empty)
            {
                if (emailType == AutoImportEmailTypeEnum.WaitingConfirmation)
                    throw new VRBusinessException(string.Format("There is no mail template selected for type: '{0}'.", receivedPricelistRecordId));
                else
                    return;
            }
            if (internalAutoImportTemplate.AttachPricelist && receivedPricelist.FileId.HasValue)
            {
                receivedPricelistFile = fileManager.GetFile(receivedPricelist.FileId.Value);
                receivedPricelistFile.ThrowIfNull(string.Format("There is no file with Id '{0}'.", receivedPricelist.FileId.Value));
                internalMailAttachements.Add(new VRMailAttachmentExcel { Name = receivedPricelistFile.Name, Content = receivedPricelistFile.Content });
            }

            var objects = new Dictionary<string, dynamic>
            {
                {"Received Pricelist", receivedPricelist}
            };

            if ((int)receivedPricelist.Status > 65 && receivedPricelist.MessageDetails != null && receivedPricelist.MessageDetails.Count > 0)
            {
                var excelFile = new VRExcelFile();
                var errorSheet = CreateExcelErrorSheet(receivedPricelist);
                if (errorSheet != null)
                    excelFile.AddSheet(errorSheet);
                internalMailAttachements.Add(new VRMailAttachmentExcel { Name = "Errors.xls", Content = excelFile.GenerateExcelFile() });
            }

            VRMailManager vrMailManager = new VRMailManager();
            var internalEvaluatedObj = vrMailManager.EvaluateMailTemplate(internalAutoImportTemplate.EmailTemplateId.Value, objects);
            vrMailManager.SendMail(internalEvaluatedObj.From, internalEvaluatedObj.To, internalEvaluatedObj.CC, internalEvaluatedObj.BCC, internalEvaluatedObj.Subject, internalEvaluatedObj.Body, internalMailAttachements, false);
        }

        #endregion

        #region Private Classes

        private class ReceivedPricelistExcelExportHandler : ExcelExportHandler<ReceivedPricelistDetail>
        {
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<ReceivedPricelistDetail> context)
            {
                ExportExcelSheet sheet = new ExportExcelSheet
                {
                    SheetName = "Received Supplier Pricelist",
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() },
                    AutoFitColumns = true
                };
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Supplier" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Pricelist Type" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Status" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Received Date" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Processing Date" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Error" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Sent To supplier" });

                sheet.Rows = new List<ExportExcelRow>();

                string formatedDateString = Utilities.GetDateTimeFormat(DateTimeType.LongDateTime);
                if (context.BigResult != null && context.BigResult.Data != null && context.BigResult.Data.Any())
                {
                    foreach (var record in context.BigResult.Data)
                    {
                        var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                        row.Cells.Add(new ExportExcelCell { Value = record.SupplierName });
                        row.Cells.Add(new ExportExcelCell { Value = record.PriceListTypeDescription });
                        row.Cells.Add(new ExportExcelCell { Value = record.StatusDescription });
                        row.Cells.Add(new ExportExcelCell { Value = record.ReceivedPricelist.ReceivedDateTime.ToString(formatedDateString) });

                        string startProcessingDateFormated = null;
                        if (record.ReceivedPricelist.StartProcessingDateTime.HasValue)
                            startProcessingDateFormated = record.ReceivedPricelist.StartProcessingDateTime.Value.ToString(formatedDateString);

                        row.Cells.Add(new ExportExcelCell { Value = startProcessingDateFormated });

                        StringBuilder messageStringBuilder = new StringBuilder();
                        if (record.ReceivedPricelist.MessageDetails != null)
                        {
                            foreach (var message in record.ReceivedPricelist.MessageDetails)
                            {
                                if (string.IsNullOrEmpty(message.Message))
                                    continue;

                                if (messageStringBuilder.Length > 32000)
                                    break;

                                messageStringBuilder.AppendFormat(message.Message);
                            }

                        }
                        row.Cells.Add(new ExportExcelCell { Value = messageStringBuilder.ToString() });
                        row.Cells.Add(new ExportExcelCell { Value = record.SentToSupplier });

                        sheet.Rows.Add(row);

                    }
                }
                context.MainSheet = sheet;
            }
        }
        private class ReceivedPricelistRequestHandler : BigDataRequestHandler<ReceivedPricelistQuery, ReceivedPricelist, ReceivedPricelistDetail>
        {
            public override ReceivedPricelistDetail EntityDetailMapper(ReceivedPricelist entity)
            {
                return ReceivedPriceListDetailMapper(entity);
            }

            public override IEnumerable<ReceivedPricelist> RetrieveAllData(DataRetrievalInput<ReceivedPricelistQuery> input)
            {
                IReceivedPricelistManager receivedPricelistDataManager = SupPLDataManagerFactory.GetDataManager<IReceivedPricelistManager>();
                return receivedPricelistDataManager.GetFilteredReceivedPricelists(input.Query);
            }

            protected override ResultProcessingHandler<ReceivedPricelistDetail> GetResultProcessingHandler(DataRetrievalInput<ReceivedPricelistQuery> input, BigResult<ReceivedPricelistDetail> bigResult)
            {
                var resultProcessingHandler = new ResultProcessingHandler<ReceivedPricelistDetail>
                {
                    ExportExcelHandler = new ReceivedPricelistExcelExportHandler()
                };
                return resultProcessingHandler;
            }
        }

        #endregion


        #region Private Methods
        private void UpdateSentToSupplierStatus(int receivedPricelistRecordId, bool status)
        {
            IReceivedPricelistManager receivedPricelistDataManager = SupPLDataManagerFactory.GetDataManager<IReceivedPricelistManager>();
            receivedPricelistDataManager.UpdateSentToSupplierStatus(receivedPricelistRecordId, status);
        }

        private VRExcelSheet CreateExcelErrorSheet(ReceivedPricelist receivedPricelist)
        {
            var errorSheet = new VRExcelSheet();
            VRExcelColumnConfig errorColumnConfig = new VRExcelColumnConfig() { ColumnIndex = 0, ColumnWidth = 100 };
            errorSheet.AddCell(new VRExcelCell() { Value = "Errors", RowIndex = 0, ColumnIndex = 0 });
            errorSheet.SetColumnConfig(errorColumnConfig);

            for (int i = 0; i < receivedPricelist.MessageDetails.Count; i++)
            {
                errorSheet.AddCell(new VRExcelCell() { Value = receivedPricelist.MessageDetails[i].Message, RowIndex = i + 1, ColumnIndex = 0 });
            }
            return errorSheet;
        }
        private static ReceivedPricelistDetail ReceivedPriceListDetailMapper(ReceivedPricelist receivedPricelist)
        {
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            return new ReceivedPricelistDetail()
            {
                ReceivedPricelist = receivedPricelist,
                Id = receivedPricelist.Id,
                SentToSupplier = receivedPricelist.SentToSupplier,
                PriceListTypeDescription = receivedPricelist.PricelistType.HasValue ? Vanrise.Common.Utilities.GetEnumDescription(receivedPricelist.PricelistType.Value) : null,
                StatusDescription = Vanrise.Common.Utilities.GetEnumDescription(receivedPricelist.Status),
                SupplierName = new CarrierAccountManager().GetCarrierAccountName(receivedPricelist.SupplierId),
                CurrencyId = carrierAccountManager.GetCarrierAccountCurrencyId(receivedPricelist.SupplierId)
            };
        }
        #endregion
    }
}
