using System;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Web.Base;

namespace  Vanrise.Fzero.FraudAnalysis.Web.Controllers
{

    public class NormalCDRController : BaseAPIController
    {

        [HttpGet]
        public IEnumerable<CDR> GetNormalCDRs(int fromRow, int toRow, DateTime fromDate, DateTime toDate, string msisdn)
        {
            NormalCDRManager manager = new NormalCDRManager();

            return manager.GetNormalCDRs(fromRow, toRow, fromDate, toDate, msisdn);
        }


    }
}