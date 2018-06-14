using System;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.SupplierPriceList.Entities;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.SupplierPriceList.BP.Arguments
{
	public class SupplierPriceListProcessInput : BaseProcessInputArgument
	{
		public int SupplierAccountId { get; set; }

		public long FileId { get; set; }

		public int CurrencyId { get; set; }

		public DateTime PriceListDate { get; set; }

		public int SupplierPriceListTemplateId { get; set; }

		public SupplierPriceListType SupplierPriceListType { get; set; }

		public bool IsAutoImport { get; set; }

		public int ReceivedPricelistRecordId { get; set; }

		public override string EntityId
		{
			get
			{
				return SupplierAccountId.ToString();
			}
		}
		public override string GetTitle()
		{
			ICarrierAccountManager carrierAccountManager = BEManagerFactory.GetManager<ICarrierAccountManager>();
			string supplierName = carrierAccountManager.GetCarrierAccountName(SupplierAccountId);
			return String.Format("#BPDefinitionTitle# Process Started for Supplier: {0}", supplierName);
		}
	}
}
