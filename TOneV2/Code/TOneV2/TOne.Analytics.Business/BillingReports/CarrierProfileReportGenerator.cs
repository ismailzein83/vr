using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Entities;
using TOne.Entities;    

namespace TOne.Analytics.Business.BillingReports
{
    public class CarrierProfileReportGenerator : TOne.Entities.IReportGenerator
    {
        public Dictionary<string, System.Collections.IEnumerable> GenerateDataSources(TOne.Entities.ReportParameters parameters)
        {

            BillingStatisticManager manager = new BillingStatisticManager();
            List<CarrierProfileReport> carrierProfile =
                manager.GetCarrierProfileMTDAndMTA(parameters.FromTime, parameters.ToTime,parameters.CustomerId,true);

            Dictionary<string, System.Collections.IEnumerable> dataSources = new Dictionary<string, System.Collections.IEnumerable>();
            dataSources.Add("CarrierProfile", carrierProfile);
            return dataSources;
        }

        public Dictionary<string, RdlcParameter> GetRdlcReportParameters(TOne.Entities.ReportParameters parameters)
        {
            Dictionary<string, RdlcParameter> list = new Dictionary<string, RdlcParameter>();

            list.Add("FromDate", new RdlcParameter { Value = parameters.FromTime.ToString(), IsVisible = true });
            list.Add("ToDate", new RdlcParameter { Value = parameters.ToTime.ToString(), IsVisible = true });
            list.Add("Title", new RdlcParameter { Value = "Carrier Profile", IsVisible = true });
            list.Add("Currency", new RdlcParameter { Value = "[USD] United States Dollars", IsVisible = true });
            list.Add("LogoPath", new RdlcParameter { Value = "logo", IsVisible = true });
            list.Add("DigitRate", new RdlcParameter { Value = "4", IsVisible = true });

            return list;
        }
    }
}
