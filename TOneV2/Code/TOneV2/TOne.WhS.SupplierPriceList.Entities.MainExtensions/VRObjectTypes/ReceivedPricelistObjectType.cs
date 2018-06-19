using System;
using Vanrise.Entities;
using TOne.WhS.SupplierPriceList.Business;
using TOne.WhS.SupplierPriceList.Entities;

namespace TOne.WhS.SupplierPriceList.MainExtensions.VRObjectTypes
{
	public class ReceivedPricelistObjectType : VRObjectType
	{
		public override Guid ConfigId
		{
			get { return new Guid("0A5F1A1F-7A68-44EF-A71D-7BFD81D68F91"); }
		}

		public override object CreateObject(IVRObjectTypeCreateObjectContext context)
		{
			ReceivedSupplierPricelistManager receivedSupplierPricelistManager = new ReceivedSupplierPricelistManager();
			ReceivedPricelist receivedPricelist = receivedSupplierPricelistManager.GetReceivedPricelistById(Convert.ToInt32(context.ObjectId));

			if (receivedPricelist == null)
				throw new DataIntegrityValidationException(string.Format("Received pricelist not found for ID: '{0}'", context.ObjectId));

			return receivedPricelist;
		}
	}
}
