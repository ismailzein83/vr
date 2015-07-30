using System;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Fzero.FraudAnalysis.Web.Controllers
{
    public class NumberProfileController : BaseAPIController
    {
        [HttpGet]
        public IEnumerable<NumberProfile> GetNumberProfiles(int fromRow, int toRow, DateTime fromDate, DateTime toDate, string subscriberNumber)
        {
            NumberProfileManager manager = new NumberProfileManager();

            return manager.GetNumberProfiles(fromRow, toRow, fromDate, toDate, subscriberNumber);
        }
    }
}