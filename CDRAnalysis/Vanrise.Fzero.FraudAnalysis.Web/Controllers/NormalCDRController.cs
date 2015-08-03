using System.Web.Http;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Web.Base;

namespace  Vanrise.Fzero.FraudAnalysis.Web.Controllers
{

    public class NormalCDRController : BaseAPIController
    {

        [HttpPost]
        public object GetNormalCDRs(Vanrise.Entities.DataRetrievalInput<NormalCDRResultQuery> input)
        {
            NormalCDRManager manager = new NormalCDRManager();

            return GetWebResponse(input, manager.GetNormalCDRs(input));
        }

    }
}