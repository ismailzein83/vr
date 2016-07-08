using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Entities;

namespace TOne.WhS.SupplierPriceList.Data
{
    public interface IPriceListTemplateDataManager:IDataManager
    {
        bool ArePriceListTemplatesUpdated(ref object updateHandle);
        List<PriceListTemplate> GetPriceListTemplates();
        bool InsertPriceListTemplate(PriceListTemplate priceListTemplate, out int insertedObjectId);
        bool UpdatePriceListTemplate(PriceListTemplate priceListTemplate);
    }
}
