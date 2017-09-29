using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.Data
{
    public interface IPricingTemplateDataManager : IDataManager
    {
        List<PricingTemplate> GetPricingTemplates();
        bool Insert(PricingTemplate pricingTemplate, out int pricingTemplateId);
        bool Update(PricingTemplate pricingTemplate);
        bool ArePricingTemplatesUpdated(ref object updateHandle);
    }
}
