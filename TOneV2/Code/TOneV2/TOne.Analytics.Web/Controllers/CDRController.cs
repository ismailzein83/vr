using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TOne.Analytics.Business;
using TOne.Analytics.Entities;
namespace TOne.Analytics.Web.Controllers
{
    public class CDRController : Vanrise.Web.Base.BaseAPIController
    {
        private readonly CDRManager __cdrManager;
        public CDRController()
        {

            __cdrManager = new CDRManager();
        }

        public List<CDR> GetCDRData(DateTime fromDate, DateTime toDate, int nRecords)
        {

            return __cdrManager.GetCDRData(fromDate, toDate, nRecords);
        }
    }
}