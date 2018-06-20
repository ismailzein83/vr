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
		public void SendReceivedPricelistReply(ReceivedPricelist receivedPricelist)
		{
			var vrMailManager = new VRMailManager();
			VRFileManager fileManager = new VRFileManager();

			ReplyMapping x = new ReplyMapping();

			var objects = new Dictionary<string, dynamic>
			{
				{"ReceivedPricelist", receivedPricelist}
			};
			List<VRMailAttachement> vrMailAttachements = null;

			if (receivedPricelist.FileId.HasValue && x.AttachFile)
			{
				var pricelistFile = fileManager.GetFile(receivedPricelist.FileId.Value);
				vrMailAttachements = new List<VRMailAttachement>() { new VRMailAttachmentExcel { Name = pricelistFile.Name, Content = pricelistFile.Content } };
			}
			var evaluatedObject = vrMailManager.EvaluateMailTemplate(x.MailTemplateId, objects);
			vrMailManager.SendMail(evaluatedObject.From, evaluatedObject.To, evaluatedObject.CC, evaluatedObject.BCC, evaluatedObject.Subject, evaluatedObject.Body
						, vrMailAttachements, false);
		}

		public IDataRetrievalResult<ReceivedPricelistDetail> GetFilteredReceivedPricelists(DataRetrievalInput<ReceivedPricelistQuery> input)
		{
			return BigDataManager.Instance.RetrieveData(input, new ReceivedPricelistRequestHandler());
		}

		public ReceivedPricelist GetReceivedPricelistById(int id)
		{
			IReceivedPricelistManager receivedPricelistDataManager = SupPLDataManagerFactory.GetDataManager<IReceivedPricelistManager>();
			return receivedPricelistDataManager.GetReceivedPricelistById(id);
		}

		public void SendMail(int receivedPricelistRecordId, AutoImportEmailTypeEnum emailType)
		{
			VRFileManager fileManager = new VRFileManager();
			VRFile receivedPricelistFile = null;
			var receivedPricelist = GetReceivedPricelistById(receivedPricelistRecordId);
			receivedPricelist.ThrowIfNull(string.Format("There is no received pricelist with Id '{0}'.", receivedPricelistRecordId));

			var supplierMailAttachements = new List<VRMailAttachement>();
			var internalMailAttachements = new List<VRMailAttachement>();

			AutoImportTemplate supplierAutoImportTemplate = new BusinessEntity.Business.ConfigManager().GetSupplierAutoImportTemplateByType(emailType);
			AutoImportTemplate internalAutoImportTemplate = new BusinessEntity.Business.ConfigManager().GetInternalAutoImportTemplateByType(emailType);

			if (supplierAutoImportTemplate.AttachPricelist && receivedPricelist.FileId.HasValue)
			{
				receivedPricelistFile = fileManager.GetFile(receivedPricelist.FileId.Value);
				receivedPricelistFile.ThrowIfNull(string.Format("There is no file with Id '{0}'.", receivedPricelist.FileId.Value));
				supplierMailAttachements.Add(new VRMailAttachmentExcel { Name = receivedPricelistFile.Name, Content = receivedPricelistFile.Content });
			}

			if (internalAutoImportTemplate.AttachPricelist && receivedPricelist.FileId.HasValue)
			{
				receivedPricelistFile = (receivedPricelistFile == null) ? fileManager.GetFile(receivedPricelist.FileId.Value) : receivedPricelistFile;
				receivedPricelistFile.ThrowIfNull(string.Format("There is no file with Id '{0}'.", receivedPricelist.FileId.Value));
				internalMailAttachements.Add(new VRMailAttachmentExcel { Name = receivedPricelistFile.Name, Content = receivedPricelistFile.Content });
			}

			var objects = new Dictionary<string, dynamic>
			{
				{"Received Pricelist", receivedPricelist}
			};

			if (receivedPricelist.ErrorDetails != null && receivedPricelist.ErrorDetails.Count > 0)
			{
				var excelFile = new VRExcelFile();
				var errorSheet = new VRExcelSheet();
				errorSheet.AddCell((new VRExcelCell() { Value = "Errors", RowIndex = 0, ColumnIndex = 0 }));

				for (int i = 0; i < receivedPricelist.ErrorDetails.Count; i++)
				{
					errorSheet.AddCell(new VRExcelCell() { Value = receivedPricelist.ErrorDetails[i].ErrorMessage, RowIndex = i + 1, ColumnIndex = 0 });
				}
				excelFile.AddSheet(errorSheet);
				supplierMailAttachements.Add(new VRMailAttachmentExcel { Name = "Errors.xls", Content = excelFile.GenerateExcelFile() });
				internalMailAttachements.Add(new VRMailAttachmentExcel { Name = "Errors.xls", Content = excelFile.GenerateExcelFile() });
			}

			VRMailManager vrMailManager = new VRMailManager();
			var supplierEvaluatedObj = vrMailManager.EvaluateMailTemplate(supplierAutoImportTemplate.EmailTemplateId, objects);
			vrMailManager.SendMail(supplierEvaluatedObj.From, supplierEvaluatedObj.To, supplierEvaluatedObj.CC, supplierEvaluatedObj.BCC, supplierEvaluatedObj.Subject, supplierEvaluatedObj.Body, supplierMailAttachements, false);

			var internalEvaluatedObj = vrMailManager.EvaluateMailTemplate(internalAutoImportTemplate.EmailTemplateId, objects);
			vrMailManager.SendMail(internalEvaluatedObj.From, internalEvaluatedObj.To, internalEvaluatedObj.CC, internalEvaluatedObj.BCC, internalEvaluatedObj.Subject, internalEvaluatedObj.Body, internalMailAttachements, false);
		}

		#endregion

		#region Private Classes

		private class ReceivedPricelistRequestHandler : BigDataRequestHandler<ReceivedPricelistQuery, ReceivedPricelist, ReceivedPricelistDetail>
		{

			public override ReceivedPricelistDetail EntityDetailMapper(ReceivedPricelist entity)
			{
				return new ReceivedPricelistDetail()
				{
					ReceivedPricelist = entity,
					SupplierName = new CarrierAccountManager().GetCarrierAccountName(entity.SupplierId),
					StatusDescription = Vanrise.Common.Utilities.GetEnumDescription(entity.Status),
					PriceListTypeDescription = entity.PricelistType.HasValue ? Vanrise.Common.Utilities.GetEnumDescription(entity.PricelistType.Value) : null,
				};
			}

			public override IEnumerable<ReceivedPricelist> RetrieveAllData(DataRetrievalInput<ReceivedPricelistQuery> input)
			{
				IReceivedPricelistManager receivedPricelistDataManager = SupPLDataManagerFactory.GetDataManager<IReceivedPricelistManager>();
				return receivedPricelistDataManager.GetFilteredReceivedPricelists(input.Query);
			}
		}

		#endregion
	}
}
