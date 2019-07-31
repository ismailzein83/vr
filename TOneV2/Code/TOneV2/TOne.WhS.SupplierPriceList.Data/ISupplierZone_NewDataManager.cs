using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.SupplierPriceList.Entities;

namespace TOne.WhS.SupplierPriceList.Data
{
	public interface ISupplierZone_NewDataManager : IDataManager
	{
		bool InsertSupplierZone_New(SupplierZone supplierZone);
		void UpdateSupplierZone_New(int supplierZoneId, SupplierZone supplierZone);
		
	}
}
