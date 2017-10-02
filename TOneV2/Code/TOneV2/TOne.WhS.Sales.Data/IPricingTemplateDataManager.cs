using System.Collections.Generic;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.Data
{
    public interface IPricingTemplateDataManager : IDataManager
    {
        List<PricingTemplate> GetPricingTemplates();

        bool Insert(PricingTemplate pricingTemplate, out int pricingTemplateId);

        bool Update(PricingTemplateToEdit pricingTemplateToEdit); 

        bool ArePricingTemplatesUpdated(ref object updateHandle);
    }
}
