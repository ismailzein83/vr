﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.AccountBalance.Business;
using Vanrise.AccountBalance.Entities;
using Vanrise.Web.Base;

namespace Vanrise.AccountBalance.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "BillingTransactionType")]
    public class BillingTransactionTypeController : BaseAPIController
    {
        [HttpGet]
        [Route("GetBillingTransactionTypesInfo")]
        public IEnumerable<BillingTransactionTypeInfo> GetBillingTransactionTypesInfo(string filter = null)
        {
            BillingTransactionTypeInfoFilter deserializedFilter = (filter != null) ? Vanrise.Common.Serializer.Deserialize<BillingTransactionTypeInfoFilter>(filter) : null;
            BillingTransactionTypeManager manager = new BillingTransactionTypeManager();
            return manager.GetBillingTransactionTypesInfo(deserializedFilter);
        }
    }
}