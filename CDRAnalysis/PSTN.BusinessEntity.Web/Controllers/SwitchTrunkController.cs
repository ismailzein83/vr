using PSTN.BusinessEntity.Business;
using PSTN.BusinessEntity.Entities;
using System.Web.Http;

namespace PSTN.BusinessEntity.Web.Controllers
{
    public class SwitchTrunkController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpPost]
        public object GetFilteredSwitchTrunks(Vanrise.Entities.DataRetrievalInput<SwitchTrunkDetailQuery> input)
        {
            SwitchTrunkManager manager = new SwitchTrunkManager();
            return GetWebResponse(input, manager.GetFilteredSwitchTrunks(input));
        }
    }
}