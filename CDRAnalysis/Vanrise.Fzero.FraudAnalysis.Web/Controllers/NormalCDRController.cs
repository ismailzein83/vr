﻿using System.Web.Http;
using Vanrise.Fzero.CDRImport.Business;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Web.Base;

namespace  Vanrise.Fzero.FraudAnalysis.Web.Controllers
{

    public class NormalCDRController : BaseAPIController
    {

        [HttpPost]
        public object GetNormalCDRs(Vanrise.Entities.DataRetrievalInput<NormalCDRQuery> input)
        {
            NormalCDRManager manager = new NormalCDRManager();

            return GetWebResponse(input, manager.GetNormalCDRs(input));
        }

    }
}