﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TOne.BusinessEntity.Business;
using TOne.BusinessEntity.Entities;

namespace TOne.BusinessEntity.Web.Controllers
{
    public class AccountManagerController : ApiController
    {
        [HttpGet]
        public List<AccountManagerCarrier> GetCarriers(int from, int to)
        {
            AccountManagerManager manager = new AccountManagerManager();
            return manager.GetCarriers(from, to);
        }

        [HttpGet]
        public List<AssignedAccountManagerCarrier> GetAssignedCarriers(int userId)
        {
            AccountManagerManager manager = new AccountManagerManager();
            return manager.GetAssignedCarriers(userId);
        }

        [HttpPost]
        public void AssignCarriers(UpdatedAccountManagerCarrier[] updatedCarriers)
        {
            AccountManagerManager manager = new AccountManagerManager();
            manager.AssignCarriers(updatedCarriers);
        }
    }
}
