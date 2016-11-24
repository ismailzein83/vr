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
    [RoutePrefix(Constants.ROUTE_PREFIX + "BalanceAlert")]
    [JSONWithTypeAttribute]
    public class BalanceAlertController : BaseAPIController
    {
        BalanceAlertManager _manager = new BalanceAlertManager();
   
    }
}