using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Analytics.Entities.BillingReport;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Analytics.Business.BillingReports
{
    public class ZoneProfitReportGenerator : IReportGenerator
    {
        public Dictionary<string, System.Collections.IEnumerable> GenerateDataSources(ReportParameters parameters)
        {
            AccountManagerManager am = new AccountManagerManager();
            IEnumerable<AssignedCarrier> suppliersIds = am.GetAssignedCarriers();
            IEnumerable<AssignedCarrier> customersIds = am.GetAssignedCarriers();
            List<string> lstSuppliers = new List<string>();
            foreach (var a in suppliersIds)
            {
                lstSuppliers.Add(a.CarrierAccountId.ToString());
            }
            List<string> lstCustomers = new List<string>();
            foreach (var a in customersIds)
            {
                lstCustomers.Add(a.CarrierAccountId.ToString());
            }
            BillingStatisticManager manager = new BillingStatisticManager();
            List<ZoneProfitFormatted> zoneProfit = manager.GetZoneProfit(parameters.FromTime, parameters.ToTime, parameters.CustomersId, parameters.SuppliersId, parameters.GroupByCustomer, lstSuppliers, lstCustomers, parameters.CurrencyId);

            //List<CarrierSummaryDailyFormatted> zs = manager.GetDailyCarrierSummary(DateTime.Parse("2012-05-01 00:00:00"), DateTime.Parse("2015-05-01 00:00:00"), null, null, false, true, null , null );
            Dictionary<string, System.Collections.IEnumerable> dataSources = new Dictionary<string, System.Collections.IEnumerable>();
            dataSources.Add("ZoneProfit", zoneProfit);
            return dataSources;
        }

        public Dictionary<string, RdlcParameter> GetRdlcReportParameters(ReportParameters parameters)
        {
            Dictionary<string, RdlcParameter> list = new Dictionary<string, RdlcParameter>();
            list.Add("GroupByCustomer", new RdlcParameter() { Value = parameters.GroupByCustomer.ToString(), IsVisible = false });
            list.Add("FromDate", new RdlcParameter { Value = parameters.FromTime.ToString(), IsVisible = true });
            list.Add("ToDate", new RdlcParameter { Value = parameters.ToTime.ToString(), IsVisible = true });
            list.Add("Title", new RdlcParameter { Value = "Zone Profit", IsVisible = true });
            list.Add("Currency", new RdlcParameter { Value = parameters.CurrencyDescription, IsVisible = true });
            list.Add("LogoPath", new RdlcParameter { Value = "logo", IsVisible = true });
            list.Add("Customer", new RdlcParameter { Value = ReportHelpers.GetCarrierName(parameters.CustomersId, "Customers"), IsVisible = true });
            list.Add("Supplier", new RdlcParameter { Value = ReportHelpers.GetCarrierName(parameters.SuppliersId, "Suppliers"), IsVisible = true });
            list.Add("DigitRate", new RdlcParameter { Value = "4", IsVisible = true });

            return list;
        }
    }
}
