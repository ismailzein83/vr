﻿using System;
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
    [RoutePrefix(Constants.ROUTE_PREFIX + "StateBackup")]
    public class WhSBE_StateBackupController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredStateBackups")]
        public object GetFilteredStateBackups(Vanrise.Entities.DataRetrievalInput<StateBackupQuery> input)
        {
            StateBackupManager manager = new StateBackupManager();
            return GetWebResponse(input, manager.GetFilteredStateBackups(input));
        }


        [HttpGet]
        [Route("RestoreData")]
        public TOne.Entities.UpdateOperationOutput<StateBackup> RestoreData(long stateBackupId)
        {
            StateBackupManager manager = new StateBackupManager();
            return  manager.RestoreData(stateBackupId);
        }

       
    }
}