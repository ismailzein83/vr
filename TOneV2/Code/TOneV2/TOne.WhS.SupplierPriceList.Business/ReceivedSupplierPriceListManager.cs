using System;
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

            AutoImportTemplate supplierAutoImportTemplate = new BusinessEntity.Business.ConfigManager().GetSupplierAutoImportTemplateByType(emailType);
            supplierAutoImportTemplate.ThrowIfNull(string.Format("There is no template selected for event {0}", Vanrise.Common.Utilities.GetEnumDescription(emailType)));
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

            if ((int)receivedPricelist.Status > 65 && receivedPricelist.ErrorDetails != null && receivedPricelist.ErrorDetails.Count > 0)
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

            AutoImportTemplate internalAutoImportTemplate = new BusinessEntity.Business.ConfigManager().GetInternalAutoImportTemplateByType(emailType);
            if (internalAutoImportTemplate == null)
                return;

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

            if ((int)receivedPricelist.Status > 65 && receivedPricelist.ErrorDetails != null && receivedPricelist.ErrorDetails.Count > 0)
            {
                var excelFile = new VRExcelFile();
                var errorSheet = CreateExcelErrorSheet(receivedPricelist);
                if(errorSheet != null)
                excelFile.AddSheet(errorSheet);
                internalMailAttachements.Add(new VRMailAttachmentExcel { Name = "Errors.xls", Content = excelFile.GenerateExcelFile() });
            }

            VRMailManager vrMailManager = new VRMailManager();
            var internalEvaluatedObj = vrMailManager.EvaluateMailTemplate(internalAutoImportTemplate.EmailTemplateId, objects);
            vrMailManager.SendMail(internalEvaluatedObj.From, internalEvaluatedObj.To, internalEvaluatedObj.CC, internalEvaluatedObj.BCC, internalEvaluatedObj.Subject, internalEvaluatedObj.Body, internalMailAttachements, false);
        }

        #endregion

        #region Private Classes

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

            for (int i = 0; i < receivedPricelist.ErrorDetails.Count; i++)
            {
                errorSheet.AddCell(new VRExcelCell() { Value = receivedPricelist.ErrorDetails[i].ErrorMessage, RowIndex = i + 1, ColumnIndex = 0 });
            }
            return errorSheet;
        }
        private static ReceivedPricelistDetail ReceivedPriceListDetailMapper(ReceivedPricelist receivedPricelist)
        {
            return new ReceivedPricelistDetail()
            {
                ReceivedPricelist = receivedPricelist,
                Id = receivedPricelist.Id,
                SentToSupplier = receivedPricelist.SentToSupplier,
                PriceListTypeDescription = receivedPricelist.PricelistType.HasValue ? Vanrise.Common.Utilities.GetEnumDescription(receivedPricelist.PricelistType.Value) : null,
                StatusDescription = Vanrise.Common.Utilities.GetEnumDescription(receivedPricelist.Status),
                SupplierName = new CarrierAccountManager().GetCarrierAccountName(receivedPricelist.SupplierId),
            };
        }
        #endregion
    }
}
