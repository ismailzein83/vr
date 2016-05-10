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
        public static string GetCarrierName(string id, string carrierAs)
        {
            string name = "multiple " + carrierAs;
            CarrierAccountManager _bemanager = new CarrierAccountManager();
            if (id != null)
            {
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
                    int carrierId = Convert.ToInt32(id);
                    name = _bemanager.GetCarrierAccountName(carrierId);
                }   
            }
            else
            {
                name = "All " + carrierAs;
            }
            return name;
        }

    }
}
