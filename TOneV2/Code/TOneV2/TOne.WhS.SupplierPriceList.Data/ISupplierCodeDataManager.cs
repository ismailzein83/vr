using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.SupplierPriceList.Entities;

namespace TOne.WhS.SupplierPriceList.Data
{
	public interface ISupplierCodeDataManager : IDataManager
	{
		bool InsertSupplierCode(SupplierCode supplierZone);
		void UpdateSupplierZone(int supplierZoneId, SupplierCode supplierZone);
		
	}
}
