using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Analytics.Data;
using TOne.WhS.Analytics.Entities;
using TOne.WhS.Analytics.Entities.BillingReport;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Analytics.Business.BillingReports
{
    public class ZoneSummaryReportGenerator : IReportGenerator
    {
        private readonly IGenericBillingDataManager _gdatamanager;

        public ZoneSummaryReportGenerator()
        {
            _gdatamanager = AnalyticsDataManagerFactory.GetDataManager<IGenericBillingDataManager>();
        }

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
            
            double service = 0;

            TOne.WhS.Analytics.Entities.BillingReportQuery query = new BillingReportQuery();
            query.FromDate = parameters.FromTime;
            query.ToDate = parameters.ToTime;
            query.CustomerIds = parameters.CustomersId;
            query.SupplierIds = parameters.SuppliersId;
            query.CurrencyId = parameters.CurrencyId;


            var listBillingReportRecords = _gdatamanager.GetFilteredBillingReportRecords(query); 
            
            List<ZoneSummary> listzoneSummaries = new List<ZoneSummary>();
            foreach (var record in listBillingReportRecords)
            {
                ZoneSummary zoneSummary = new ZoneSummary();
                zoneSummary.Zone = record.Zone;
                zoneSummary.Calls = record.Calls;
                zoneSummary.CommissionValue = record.CommissionValue;
                zoneSummary.DurationInSeconds = record.DurationInSeconds;
                zoneSummary.DurationNet = record.DurationNet;
                listzoneSummaries.Add(zoneSummary);
            }
            List<ZoneSummaryFormatted> zoneSummaries = manager.FormatZoneSummaries(listzoneSummaries);

            //List<ZoneSummaryFormatted> zoneSummaries =
            //    manager.GetZoneSummary(parameters.FromTime, parameters.ToTime, parameters.CustomersId, parameters.SuppliersId, parameters.IsCost,
            //    parameters.CurrencyId, parameters.SupplierGroup, parameters.CustomerGroup, lstCustomers, lstSuppliers, parameters.GroupBySupplier, out service);
            

            decimal services = 0;
            if (parameters.IsCost)
                if (zoneSummaries.Count != 0)
                    services = (decimal)service;

            parameters.ServicesForCustomer = services;
            parameters.NormalDuration = zoneSummaries.Where(y => y.RateTypeFormatted == "Normal").Sum(x => Math.Round(x.DurationInSeconds, 2));

            parameters.OffPeakDuration = Math.Ceiling(zoneSummaries.Where(y => y.RateTypeFormatted == "OffPeak").Sum(x => Math.Round(x.DurationInSeconds, 2)));

            parameters.NormalNet = zoneSummaries.Where(y => y.RateTypeFormatted == "Normal").Sum(x => x.Net).Value;

            parameters.OffPeakNet = Math.Round(zoneSummaries.Where(y => y.RateTypeFormatted == "OffPeak").Sum(x => x.Net).Value, 0);

            parameters.TotalAmount = parameters.OffPeakNet + parameters.NormalNet;

            Dictionary<string, System.Collections.IEnumerable> dataSources = new Dictionary<string, System.Collections.IEnumerable>();
            dataSources.Add("ZoneSummaries", zoneSummaries);
            return dataSources;
        }

        public Dictionary<string, RdlcParameter> GetRdlcReportParameters(ReportParameters parameters)
        {
            Dictionary<string, RdlcParameter> list = new Dictionary<string, RdlcParameter>();

            list.Add("FromDate", new RdlcParameter { Value = parameters.FromTime.ToString(), IsVisible = true });
            list.Add("ToDate", new RdlcParameter { Value = parameters.ToTime.ToString(), IsVisible = true });
            list.Add("Title", new RdlcParameter { Value = String.Format("Zone Summary ({0})", parameters.IsCost == true ? "Buy" : "Sale"), IsVisible = true });
            list.Add("Currency", new RdlcParameter { Value = parameters.CurrencyDescription, IsVisible = true });
            list.Add("LogoPath", new RdlcParameter { Value = "logo", IsVisible = true });
            list.Add("Customer", new RdlcParameter { Value = ReportHelpers.GetCarrierName(parameters.CustomersId, "Customers"), IsVisible = true });
            list.Add("Supplier", new RdlcParameter { Value = ReportHelpers.GetCarrierName(parameters.SuppliersId, "Suppliers"), IsVisible = true });
            list.Add("DigitRate", new RdlcParameter { Value = "4", IsVisible = true });
            list.Add("NormalNet", new RdlcParameter { Value = parameters.NormalNet.ToString(), IsVisible = true });
            list.Add("NormalDuration", new RdlcParameter { Value = parameters.NormalDuration.ToString(), IsVisible = true });
            list.Add("OffPeakNet", new RdlcParameter { Value = parameters.OffPeakNet.ToString(), IsVisible = true });
            list.Add("OffPeakDuration", new RdlcParameter { Value = parameters.OffPeakDuration.ToString(), IsVisible = true });
            list.Add("IsService", new RdlcParameter { Value = parameters.IsService.ToString(), IsVisible = true });
            list.Add("IsCommission", new RdlcParameter { Value = parameters.IsCommission.ToString(), IsVisible = true });
            list.Add("TotalAmount", new RdlcParameter { Value = parameters.TotalAmount.ToString(), IsVisible = true });
            list.Add("ServicesForCustomer", new RdlcParameter { Value = parameters.ServicesForCustomer.ToString(), IsVisible = true });
            list.Add("IsCost", new RdlcParameter { Value = parameters.IsCost.ToString(), IsVisible = true });
            list.Add("GroupeBySupplier", new RdlcParameter { Value = parameters.GroupBySupplier.ToString(), IsVisible = true });

            return list;
        }
    }
}
