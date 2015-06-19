﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Entities;
using TOne.Entities;

namespace TOne.Analytics.Business.BillingReports
{
    public class ExchangeCarrierProfitReportGenerator : TOne.Entities.IReportGenerator
    {
        public Dictionary<string, System.Collections.IEnumerable> GenerateDataSources(TOne.Entities.ReportParameters parameters)
        {
            BillingStatisticManager manager = new BillingStatisticManager();
            List<ExchangeCarrierFormatted> exchangeCarrier = manager.GetExchangeCarriers(parameters.FromTime, parameters.ToTime, parameters.SupplierAMUId, parameters.CustomerAMUId, parameters.IsExchange);

            Dictionary<string, System.Collections.IEnumerable> dataSources = new Dictionary<string, System.Collections.IEnumerable>();
            dataSources.Add("ExchangeCarrier", exchangeCarrier);
            dataSources.Add("ExchangeChart", exchangeCarrier.Take(10).ToList());

            return dataSources;
        }

        public Dictionary<string, RdlcParameter> GetRdlcReportParameters(TOne.Entities.ReportParameters parameters)
        {

            Dictionary<string, RdlcParameter> list = new Dictionary<string, RdlcParameter>();
            list.Add("FromDate", new RdlcParameter { Value = parameters.FromTime.ToString(), IsVisible = true });
            list.Add("ToDate", new RdlcParameter { Value = parameters.ToTime.ToString(), IsVisible = true });
            list.Add("Title", new RdlcParameter { Value = "Exchange Carrier Profit Summary", IsVisible = true });
            list.Add("Currency", new RdlcParameter { Value = "[USD] United States Dollars", IsVisible = true });
            list.Add("LogoPath", new RdlcParameter { Value = "logo", IsVisible = true });            
            list.Add("DigitRate", new RdlcParameter { Value = "2", IsVisible = true });

            return list;
        }
    }
}
