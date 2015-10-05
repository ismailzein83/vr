using PSTN.BusinessEntity.Business;
using PSTN.BusinessEntity.Entities;
using System.Web.Http;

namespace PSTN.BusinessEntity.Web.Controllers
{
    public class NormalizationRuleController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpPost]
        public object GetFilteredNormalizationRules(Vanrise.Entities.DataRetrievalInput<NormalizationRuleQuery> input)
        {
            NormalizationRuleManager manager = new NormalizationRuleManager();
            return GetWebResponse(input, manager.GetFilteredNormalizationRules(input));
        }
    }
}