using System;
using System.Collections.Generic;
using Vanrise.Entities;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.BusinessEntity.Business;
using System.Linq;
using Vanrise.Common.Business;
using Vanrise.Common;

namespace TOne.WhS.SupplierPriceList.MainExtensions.VRObjectTypes
{
    public enum ReceivedPricelistField { PricelistType = 0, PricelistStatus = 1, PricelistName = 2, ReceivedDateTime = 3, StartProcessingDateTime = 4, SupplierName = 5, SupplierEmail = 6, MessageDetails = 7, SupplierId = 8, ProcessInstanceId = 9, PricelistId = 10, FileId = 11, ReceivedPricelistId = 12 }

	public class ReceivedPricelistPropertyEvaluator : VRObjectPropertyEvaluator
	{
		public override Guid ConfigId
		{
			get { return new Guid("BEC4CF34-A716-4FD9-A38B-FCF1BB56265B"); }
		}

		public ReceivedPricelistField ReceivedPricelistField { get; set; }

		public override dynamic GetPropertyValue(IVRObjectPropertyEvaluatorContext context)
		{
			ReceivedPricelist receivedPricelist = context.Object as ReceivedPricelist;
			CarrierAccountManager CarrierAccountManager = new CarrierAccountManager();

			if (receivedPricelist == null)
				throw new NullReferenceException("Received Pricelist");

			switch (this.ReceivedPricelistField)
			{
				case ReceivedPricelistField.PricelistType:
					return (receivedPricelist.PricelistType.HasValue) ? Utilities.GetEnumDescription(receivedPricelist.PricelistType.Value) : null;

				case ReceivedPricelistField.PricelistStatus:
					return Utilities.GetEnumDescription(receivedPricelist.Status);

				case ReceivedPricelistField.ReceivedDateTime:
					return receivedPricelist.ReceivedDateTime;

				case ReceivedPricelistField.SupplierId:
					return receivedPricelist.SupplierId;

				case ReceivedPricelistField.SupplierName:
					return new CarrierAccountManager().GetCarrierAccountName(receivedPricelist.SupplierId);

				case ReceivedPricelistField.ProcessInstanceId:
					return receivedPricelist.ProcessInstanceId;

				case ReceivedPricelistField.StartProcessingDateTime:
					return receivedPricelist.StartProcessingDateTime;

				case ReceivedPricelistField.PricelistId:
					return receivedPricelist.PricelistId;

				case ReceivedPricelistField.FileId:
					return receivedPricelist.FileId;

				case ReceivedPricelistField.ReceivedPricelistId:
					return receivedPricelist.Id;

				case ReceivedPricelistField.SupplierEmail:
					return new CarrierAccountManager().GetSupplierAutoImportEmail(receivedPricelist.SupplierId);

				case ReceivedPricelistField.MessageDetails:
					return GetMessages(receivedPricelist.MessageDetails);

				case ReceivedPricelistField.PricelistName:
					{
						if (!receivedPricelist.FileId.HasValue)
							return "No pricelist found.";
						var fileInfo = new VRFileManager().GetFileInfo(receivedPricelist.FileId.Value);
						fileInfo.ThrowIfNull(string.Format("No file found for id '{0}'.", receivedPricelist.FileId.Value));
						return fileInfo.Name;
					}
			}

			return null;
		}

		public string GetMessages(IEnumerable<SPLImportErrorDetail> messages)
		{
            if (messages != null && messages.Any())
                return string.Join(Environment.NewLine, messages.Select(item => item.Message));
			return null;
		}
	}
}
