﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace Vanrise.Common.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "VRBulkActionDraftController")]
    public class VRBulkActionDraftController : BaseAPIController
    {
        [HttpPost]
        [Route("GetVRBulkActionDrafts")]
        public IEnumerable<VRBulkActionDraft> GetVRBulkActionDrafts(BulkActionFinalState finaleBulkActionDraftObject)
        {
            VRBulkActionDraftManager manager = new VRBulkActionDraftManager();
            return manager.GetVRBulkActionDrafts(finaleBulkActionDraftObject);
        }

    }
}