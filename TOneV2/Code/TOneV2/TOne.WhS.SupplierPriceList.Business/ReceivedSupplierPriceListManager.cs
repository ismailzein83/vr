using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.SupplierPriceList.Data;
using TOne.WhS.SupplierPriceList.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

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
