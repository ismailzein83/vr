using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.BusinessEntity.Business;
using TOne.BusinessEntity.Entities;

namespace TOne.BusinessEntity.Web.Controllers
{
    public class CodeController:Vanrise.Web.Base.BaseAPIController
    {
        [HttpGet]
        public List<Code> GetCodes(int zoneID, DateTime effectiveOn)
        {
            CodeManager manager = new CodeManager();
            return manager.GetCodes(zoneID, effectiveOn);
        }
    }
}