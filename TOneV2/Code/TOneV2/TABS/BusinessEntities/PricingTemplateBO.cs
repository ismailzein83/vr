using System.Collections.Generic;

namespace TABS.BusinessEntities
{
    public class PricingTemplateBO
    {
        public static IList<PricingTemplatePlan> GetPricingTemplatePlan(int pricingtemplateid)
        {
            return TABS.ObjectAssembler.CurrentSession.CreateQuery(
                                                            @"FROM PricingTemplatePlan P 
                                                            WHERE P.PricingTemplate.PricingTemplateId = :id 
                                                            ORDER BY P.Priority DESC")
                                                            .SetParameter("id", pricingtemplateid)
                                                            .List<TABS.PricingTemplatePlan>();
        }
    }
}
