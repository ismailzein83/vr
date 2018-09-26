using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.BusinessEntity.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "StateBackup")]
    public class StateBackupController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredStateBackups")]
        public object GetFilteredStateBackups(Vanrise.Entities.DataRetrievalInput<StateBackupQuery> input)
        {
            StateBackupManager manager = new StateBackupManager();
            return GetWebResponse(input, manager.GetFilteredStateBackups(input), "Checkpoints");
        }


        [HttpGet]
        [Route("RestoreData")]
        public UpdateOperationOutput<StateBackup> RestoreData(long stateBackupId)
        {
            StateBackupManager manager = new StateBackupManager();
            return  manager.RestoreData(stateBackupId);
        }


        [HttpGet]
        [Route("GetStateBackupTypes")]
        public IEnumerable<StateBackupTypeConfig> GetStateBackupTypes()
        {
            StateBackupManager manager = new StateBackupManager();
            return manager.GetStateBackupTypes();
        }
       
    }
}