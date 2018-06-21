using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Common.Business;
using Vanrise.Security.Business;
using Vanrise.BEBridge.Entities;
using Vanrise.BusinessProcess.Business;
using Vanrise.BusinessProcess.Entities;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.SupplierPriceList.Data;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.BP.Arguments;

namespace TOne.WhS.SupplierPriceList.Business
{
	public class SupplierPricelistReceivedMailMessageSynchronizer : TargetBESynchronizer
	{
		public override string Name
		{
			get
			{
				return "Supplier Pricelist Received Mail Message Synchronizer";
			}
		}

		public override void InsertBEs(ITargetBESynchronizerInsertBEsContext context)
		{
			IReceivedPricelistManager receivedPricelistDataManager = SupPLDataManagerFactory.GetDataManager<IReceivedPricelistManager>();
			var receivedSupplierPricelistManager = new ReceivedSupplierPricelistManager();

			CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
			BusinessEntity.Business.ConfigManager configManager = new BusinessEntity.Business.ConfigManager();

			foreach (var targetBE in context.TargetBE)
			{
				var objectsTargetBE = targetBE as VRObjectsTargetBE;
				objectsTargetBE.ThrowIfNull("objectsTargetBE is null");
				foreach (var targetObject in objectsTargetBE.TargetObjects.Values)
				{
					VRReceivedMailMessage receivedMailMessage = targetObject as VRReceivedMailMessage;
					receivedMailMessage.ThrowIfNull("receivedMailMessage is null");
					int recordId;

					Dictionary<string, CarrierAccount> supplierAccounts = carrierAccountManager.GetCachedSupplierAccountsByAutoImportEmail();
					var supplierAccount = supplierAccounts.GetRecord(receivedMailMessage.Header.From.ToLower());

					SupplierPricelistType? pricelistType = null;
					var pricelistTypeMappings = configManager.GetPurchasePricelistTypeMappingList();
					pricelistTypeMappings.ThrowIfNull("Pricelist Type Mappings");
					foreach (var pricelistTypeMapping in pricelistTypeMappings)
					{
						if (receivedMailMessage.Header.Subject.ToLower().Contains(pricelistTypeMapping.Subject))
						{
							pricelistType = pricelistTypeMapping.PricelistType;
							break;
						}
					}

					var receivedMessageAttachments = receivedMailMessage.GetAttachments();

					List<VRFile> applicableFiles = new List<VRFile>();

					var files = receivedMessageAttachments.Where(item => item.Extension == "xls" || item.Extension == "xlsx" || item.Extension == "csv");
					if (files != null || files.Any())
						applicableFiles.AddRange(files);

					var zipApplicableFiles = GetApplicableFilesFromZipFiles(receivedMessageAttachments);
					if (zipApplicableFiles != null || zipApplicableFiles.Any())
						applicableFiles.AddRange(zipApplicableFiles);

					var attachmentCode = supplierAccount.SupplierSettings.AutoImportSettings.AttachmentCode;
					if (!string.IsNullOrEmpty(attachmentCode))
						applicableFiles = applicableFiles.FindAllRecords(item => item.Name.ToLower().Contains(attachmentCode.ToLower())).ToList();

					var supplierPriceListTemplateManager = new SupplierPriceListTemplateManager();
					var supplierPriceListTemplate = supplierPriceListTemplateManager.GetSupplierPriceListTemplateBySupplierId(supplierAccount.CarrierAccountId);

					List<SPLImportErrorDetail> errors;
					if (IsValidToImport(supplierAccount, pricelistType, applicableFiles, out errors))
					{
						var receivedPricelistFile = applicableFiles[0];
						VRFileManager fileManager = new VRFileManager();
						receivedPricelistFile.FileId = fileManager.AddFile(receivedPricelistFile);

						if (supplierPriceListTemplate == null)
						{
							errors.Add(new SPLImportErrorDetail() { ErrorMessage = "No pricelist mapping is defined yet for this supplier." });
							receivedPricelistDataManager.InsertReceivedPricelist(supplierAccount.CarrierAccountId, receivedPricelistFile.FileId, receivedMailMessage.Header.MessageSendTime, pricelistType, ReceivedPricelistStatus.FailedDueToConfigurationError, errors, out recordId);
							receivedSupplierPricelistManager.SendMailToInternal(recordId, AutoImportEmailTypeEnum.Failed);
						}
						else
						{
							int loggedInUserId = SecurityContext.Current.GetLoggedInUserId();
							receivedPricelistDataManager.InsertReceivedPricelist(supplierAccount.CarrierAccountId, receivedPricelistFile.FileId, receivedMailMessage.Header.MessageSendTime, pricelistType, ReceivedPricelistStatus.Received, null, out recordId);
							receivedSupplierPricelistManager.SendMailToSupplier(recordId, AutoImportEmailTypeEnum.Received);
							receivedSupplierPricelistManager.SendMailToInternal(recordId, AutoImportEmailTypeEnum.Received);

							var supplierPriceListProcessInput = new SupplierPriceListProcessInput
							{
								SupplierAccountId = supplierAccount.CarrierAccountId,
								FileId = receivedPricelistFile.FileId,
								SupplierPriceListTemplateId = supplierPriceListTemplate.SupplierPriceListTemplateId,
								CurrencyId = carrierAccountManager.GetCarrierAccountCurrencyId(supplierAccount.CarrierAccountId),
								PriceListDate = receivedMailMessage.Header.MessageSendTime,
								SupplierPricelistType = pricelistType.Value,
								IsAutoImport = true,
								ReceivedPricelistRecordId = recordId,
								UserId = loggedInUserId
							};

							BPInstanceManager bpClient = new BPInstanceManager();
							bpClient.CreateNewProcess(new CreateProcessInput { InputArguments = supplierPriceListProcessInput });
						}
					}
					else
					{
						receivedPricelistDataManager.InsertReceivedPricelist(supplierAccount.CarrierAccountId, null, receivedMailMessage.Header.MessageSendTime, null, ReceivedPricelistStatus.FailedDueToReceivedMailError, errors, out recordId);
						receivedSupplierPricelistManager.SendMailToSupplier(recordId, AutoImportEmailTypeEnum.Failed);
						receivedSupplierPricelistManager.SendMailToInternal(recordId, AutoImportEmailTypeEnum.Failed);
					}
				}
			}
		}

		private bool IsValidToImport(CarrierAccount supplierAccount, SupplierPricelistType? pricelistType, List<VRFile> applicableFiles, out List<SPLImportErrorDetail> errors)
		{
			errors = new List<SPLImportErrorDetail>();

			//var attachmentCode = supplierAccount.SupplierSettings.AutoImportSettings.AttachmentCode;

			if (pricelistType == null)
				errors.Add(new SPLImportErrorDetail() { ErrorMessage = "Email subject does not contain pricelist type." });

			if (applicableFiles == null || applicableFiles.Count == 0)
				errors.Add(new SPLImportErrorDetail() { ErrorMessage = "There is no applicable attachment." });

			else if (applicableFiles.Count > 1)
				errors.Add(new SPLImportErrorDetail() { ErrorMessage = "There are more than one applicable attachment." });

			//else if (!string.IsNullOrEmpty(attachmentCode) && applicableFiles[0].Name.ToLower().Contains(attachmentCode.ToLower()))
			//errors.Add(new SPLImportErrorDetail() { ErrorMessage = string.Format("Attachment name does not contain .", attachmentCode) });

			if (errors.Any())
				return false;

			return true;
		}

		private IEnumerable<VRFile> GetApplicableFilesFromZipFiles(IEnumerable<VRFile> receivedMessageAttachments)
		{
			List<VRFile> result = new List<VRFile>();
			var zipFiles = receivedMessageAttachments.Where(item => item.Extension == "zip");

			foreach (var zipFile in zipFiles)
			{
				var files = UnZipFiles(zipFile);
				var applicableFiles = files.Where(item => item.Extension == "xls" || item.Extension == "xlsx" || item.Extension == "csv");
				if (applicableFiles != null && applicableFiles.Any())
					result.AddRange(applicableFiles);
			}
			return result;
		}

		private static IEnumerable<VRFile> UnZipFiles(VRFile zipFile)
		{
			List<VRFile> files = new List<VRFile>();
			if (zipFile != null)
			{
				#region zipFile
				Stream stream = new MemoryStream(zipFile.Content);
				using (var zf = new ZipFile(stream))
				{
					foreach (ZipEntry ze in zf)
					{
						if (ze.IsDirectory)
							continue;

						string[] nameastab = ze.Name.Split('.');

						if (nameastab.Length < 2)
							continue;

						using (Stream s = zf.GetInputStream(ze))
						{
							byte[] buf = new byte[4096];
							byte[] content = null;
							using (MemoryStream ms = new MemoryStream())
							{
								StreamUtils.Copy(s, ms, buf);
								content = ms.ToArray();
							}
							var receivedFile = new VRFile()
							{
								Content = content,
								Name = ze.Name,
								Extension = nameastab[nameastab.Length - 1],
								IsTemp = true,
							};
							files.Add(receivedFile);
						}
					}
				}
				#endregion
			}
			return files;
		}

		public override bool TryGetExistingBE(ITargetBESynchronizerTryGetExistingBEContext context)
		{
			return true;
		}

		public override void UpdateBEs(ITargetBESynchronizerUpdateBEsContext context)
		{

		}
	}
}
