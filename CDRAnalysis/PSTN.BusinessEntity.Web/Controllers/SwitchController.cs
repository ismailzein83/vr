using PSTN.BusinessEntity.Business;
using PSTN.BusinessEntity.Entities;
using System.Collections.Generic;
using System.Web.Http;

namespace PSTN.BusinessEntity.Web.Controllers
{
    public class SwitchController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpGet]
        public List<SwitchType> GetSwitchTypes()
        {
            SwitchManager manager = new SwitchManager();
            return manager.GetSwitchTypes();
        }

        [HttpPost]
        public object GetFilteredSwitches(Vanrise.Entities.DataRetrievalInput<SwitchQuery> input)
        {
            SwitchManager manager = new SwitchManager();
            return GetWebResponse(input, manager.GetFilteredSwitches(input));
        }
    }
}