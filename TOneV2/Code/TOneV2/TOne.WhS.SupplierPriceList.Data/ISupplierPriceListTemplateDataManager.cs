using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Entities;

namespace TOne.WhS.SupplierPriceList.Data
{
    public interface ISupplierPriceListTemplateDataManager:IDataManager
    {
        bool AreSupplierPriceListTemplatesUpdated(ref object updateHandle);
        List<SupplierPriceListTemplate> GetSupplierPriceListTemplates();
        bool InsertSupplierPriceListTemplate(SupplierPriceListTemplate supplierPriceListTemplate, out int insertedObjectId);
        bool UpdateSupplierPriceListTemplate(SupplierPriceListTemplate supplierPriceListTemplate);
    }
}
