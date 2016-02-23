using CP.SupplierPricelist.Entities;
using System.Collections.Generic;

namespace CP.SupplierPricelist.Data
{
    public interface ICustomerSupplierMappingDataManager : IDataManager
    {
        List<CustomerSupplierMapping> GetAllCustomerSupplierMappings();
        bool Insert(CustomerSupplierMapping customerSupplierMapping);
        bool AreCustomerSupplierMappingsUpdated(ref object updateHandle);
    }
}
