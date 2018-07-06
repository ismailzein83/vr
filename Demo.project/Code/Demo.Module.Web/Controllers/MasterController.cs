using Demo.Module.Business;
using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;
using Vanrise.Entities;

namespace Demo.Module.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "Master")]
    [JSONWithTypeAttribute]
    public class Demo_Module_MasterController : BaseAPIController
    {
        MasterManager masterManager = new MasterManager();
        [HttpPost]
        [Route("GetFilteredMasters")]
        public object GetFilteredMasters(DataRetrievalInput<MasterQuery> input)
        {
            return GetWebResponse(input, masterManager.GetFilteredMasters(input));
        }

        [HttpGet]
        [Route("GetMasterById")]
        public Master GetMasterById(long masterId)
        {
            return masterManager.GetMasterById(masterId);
        }

        [HttpPost]
        [Route("UpdateMaster")]
        public UpdateOperationOutput<MasterDetails> UpdateMaster(Master master)
        {
            return masterManager.UpdateMaster(master);
        }

        [HttpPost]
        [Route("AddMaster")]
        public InsertOperationOutput<MasterDetails> AddMaster(Master master)
        {
            return masterManager.AddMaster(master);
        }
        
        
    }
}
