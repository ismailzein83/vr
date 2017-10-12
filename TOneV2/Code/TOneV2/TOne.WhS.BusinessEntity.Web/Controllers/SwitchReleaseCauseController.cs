using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Net.Http;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Web.Base;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "SwitchReleaseCause")]

    public class SwitchReleaseCauseController:BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredSwitchReleaseCauses")]
        public object GetFilteredSwitchReleaseCauses(Vanrise.Entities.DataRetrievalInput<SwitchReleaseCauseQuery> input)
        {
          
            SwitchReleaseCauseManager manager = new SwitchReleaseCauseManager();
            return GetWebResponse(input, manager.GetFilteredSwitchReleaseCauses(input));
        }
        [HttpPost]
        [Route("AddSwitchReleaseCause")]
        public InsertOperationOutput<SwitchReleaseCauseDetail> AddSwitchReleaseCause(SwitchReleaseCause switchReleaseCause)
        {
            SwitchReleaseCauseManager manager = new SwitchReleaseCauseManager();
            return manager.AddSwitchReleaseCause(switchReleaseCause);

        }
         [HttpGet]
        [Route("GetSwitchReleaseCause")]
        public SwitchReleaseCause GetSwitchReleaseCause(int switchReleaseCauseId)
        {
            SwitchReleaseCauseManager manager = new SwitchReleaseCauseManager();
            return manager.GetSwitchReleaseCause(switchReleaseCauseId);



        }
         [HttpPost]
        [Route("UpdateSwitchReleaseCause")]
         public UpdateOperationOutput<SwitchReleaseCauseDetail> UpdateSwitchReleaseCause(SwitchReleaseCause switchReleaseCause)
         {
             SwitchReleaseCauseManager manager = new SwitchReleaseCauseManager();
             return manager.UpdateSwitchReleaseCause(switchReleaseCause);
         }
    }
}