using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;

namespace TOne.WhS.Analytics.Business.BillingReports
{
    public static class ReportHelpers
    {
        public static string GetCarrierName(int id, string carrierAs)
        {
            string name = "multiple " + carrierAs;
            CarrierAccountManager _bemanager = new CarrierAccountManager();
            //if (id.Split(',').Length > 1)
            //{
            //    name = "Multiple " + carrierAs;
            //}
            //else if (id == "")
            //{
            //    name = "All " + carrierAs;
            //}
            //else
            //{
            //    name = _bemanager.GetCarrierAccountName(id);
            //}
            name = _bemanager.GetCarrierAccountName(id);
            return name;
        }
    }
}
