using System.Web.Http;
using Vanrise.Fzero.CDRImport.Business;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Fzero.FraudAnalysis.Web.Controllers
{

    [RoutePrefix(Constants.ROUTE_PREFIX + "CDR")]
    public class CDRController : BaseAPIController
    {

        [HttpPost]
        [Route("GetCDRs")]
        public object GetCDRs(Vanrise.Entities.DataRetrievalInput<CDRQuery> input)
        {
            CDRManager manager = new CDRManager();

            return GetWebResponse(input, manager.GetCDRs(input));
        }

    }
}