using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Business;

namespace TOne.Analytics.Business.BillingReports
{
    public static class ReportHelpers
    {

        public static string GetCarrierName(string id, string carrierAs)
        {
            string name = "multiple " + carrierAs;
            BusinessEntityInfoManager _bemanager = new BusinessEntityInfoManager();
            if (id.Split(',').Length > 1)
            {
                name = "Multiple " + carrierAs;
            }
            else if (id == "")
            {
                name = "All " + carrierAs;
            }
            else
            {
                name = _bemanager.GetCarrirAccountName(id);
            }

            return name;
        }
    }
}
