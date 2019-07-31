using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.SupplierPriceList.Entities;

namespace TOne.WhS.SupplierPriceList.Data
{
	public interface ISupplierRateDataManager : IDataManager
	{
		bool InsertSupplierRate(SupplierRate supplierRate);
		void UpdateSupplierRate(int supplierRateId, SupplierRate supplierRate);
		
	}
}
