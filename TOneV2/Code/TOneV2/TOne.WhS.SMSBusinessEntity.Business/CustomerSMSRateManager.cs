using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.SMSBusinessEntity.Data;
using TOne.WhS.SMSBusinessEntity.Data.RDB;
using TOne.WhS.SMSBusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Security.Business;

namespace TOne.WhS.SMSBusinessEntity.Business
{
    public class CustomerSMSRateManager
    {
        List<CustomerSMSRate> defaultList = new List<CustomerSMSRate>()
                {
                    new CustomerSMSRate() { MobileNetworkID = 10   },
                    new CustomerSMSRate() { MobileNetworkID = 20   },
                    new CustomerSMSRate() { MobileNetworkID = 30   },
                    new CustomerSMSRate() { MobileNetworkID = 40   },
                    new CustomerSMSRate() { MobileNetworkID = 50   },
                    new CustomerSMSRate() {MobileNetworkID = 60    },
                    new CustomerSMSRate() {MobileNetworkID = 70    },
                    new CustomerSMSRate() {MobileNetworkID = 80    },
                    new CustomerSMSRate() {MobileNetworkID = 90    },
                    new CustomerSMSRate() {MobileNetworkID = 101 },
                    new CustomerSMSRate() {MobileNetworkID = 110   },
                    new CustomerSMSRate() {MobileNetworkID = 120 } ,
                    new CustomerSMSRate() {MobileNetworkID = 130   },
                    new CustomerSMSRate() {MobileNetworkID = 140 },
                    new CustomerSMSRate() {MobileNetworkID = 150   },
                    new CustomerSMSRate() {MobileNetworkID = 160 },
                    new CustomerSMSRate() {MobileNetworkID = 170   },
                    new CustomerSMSRate() {MobileNetworkID = 180   },
                    new CustomerSMSRate() {MobileNetworkID = 190 },
                    new CustomerSMSRate() { MobileNetworkID =201   },
                    new CustomerSMSRate() {MobileNetworkID = 211   },
                    new CustomerSMSRate() { MobileNetworkID = 220  },
                    new CustomerSMSRate() {MobileNetworkID = 230 } ,
                    new CustomerSMSRate() { MobileNetworkID = 240  },
                    new CustomerSMSRate() {MobileNetworkID = 250 },
                    new CustomerSMSRate() {MobileNetworkID = 261 },
                    new CustomerSMSRate() {MobileNetworkID = 271 },
                    new CustomerSMSRate() { MobileNetworkID = 281  },
                    new CustomerSMSRate() {MobileNetworkID = 291 },
                    new CustomerSMSRate() {MobileNetworkID = 301   },
                    new CustomerSMSRate() {MobileNetworkID = 311 },
                    new CustomerSMSRate() { MobileNetworkID = 320  }
                };

        #region Public Methods
        public Vanrise.Entities.IDataRetrievalResult<CustomerSMSRateDetail> GetFilteredCustomerSMSRate(DataRetrievalInput<CustomerSMSRateQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new CustomerSMSRateRequestHandler());
        }

        public List<CustomerSMSRate> GetMobileNetworkRates(int customerID)
        {
            return GetCustomerSMSRates(customerID);
        }

        //public List<CustomerSMSRateChange> GetMobileNetworkRates(CustomerSMSRateChangesQuery customerSMSRateChangesQuery)
        //{
        //    defaultList = defaultList.Where(item => FilterCustomerSMSRateChanges(item, customerSMSRateChangesQuery)).ToList();

        //    AddDataToMobileNetworks(defaultList, customerSMSRateChangesQuery.CustomerID);

        //    return CustomerSMSRateChangesMapper(defaultList);
        //}

        public List<char> GetMobileCountryLetters()
        {
            return defaultList.Select(x => char.ToUpper(GetMobileCountryName(x.MobileNetworkID)[0])).Distinct().OrderBy(x => x).ToList();
        }

        #endregion

        #region Private Methods 

        private List<CustomerSMSRate> GetCustomerSMSRates(int customerID)
        {
            ICustomerSMSRateDataManager customerSMSRateDataManager = CustomerSMSRateDataManagerFactory.GetDataManager<ICustomerSMSRateDataManager>();

            return customerSMSRateDataManager.GetCustomerSMSRates(customerID);
        }

        private bool FilterCustomerSMSRateChanges(CustomerSMSRate customerSMSRate, CustomerSMSRateChangesQuery customerSMSRateChangesQuery)
        {
            if (!customerSMSRateChangesQuery.CountryChar.HasValue)
                return true;

            string countryCharToString = customerSMSRateChangesQuery.CountryChar.Value.ToString().ToUpper();
            countryCharToString = countryCharToString.Replace("\0", string.Empty);

            if (!string.IsNullOrEmpty(countryCharToString) && !GetMobileCountryName(customerSMSRate.MobileNetworkID).ToUpper().StartsWith(countryCharToString))
                return false;

            return true;
        }

        private void AddDataToMobileNetworks(List<CustomerSMSRate> defaultList, int customerID)
        {
            List<CustomerSMSRate> customerSMSRates = GetCustomerSMSRates(customerID);

            if (customerSMSRates == null || customerSMSRates.Count == 0)
                return;

            foreach (var defaultCustomerSMS in defaultList)
            {
                CustomerSMSRate customerSMSPreviousEED = new CustomerSMSRate();

                CustomerSMSRate customerSMSNextBED = new CustomerSMSRate();

                SetCustomerSMSPreviousEEDNextBED(customerSMSRates, customerSMSPreviousEED, customerSMSNextBED, defaultCustomerSMS.MobileNetworkID);

                defaultCustomerSMS.FutureRate = customerSMSNextBED.FutureRate;
                //defaultCustomerSMS.HasFutureRate = customerSMSNextBED.BED ? true : false;
                //defaultCustomerSMS.Rate = customerSMSPreviousEED.BED.HasValue ? customerSMSPreviousEED.Rate : null;
            }
        }

        private void SetCustomerSMSPreviousEEDNextBED(List<CustomerSMSRate> customerSMSRates, CustomerSMSRate customerSMSPreviousEED, CustomerSMSRate customerSMSNextBED, int mobileNetworkID)
        {
            DateTime dateTimeNow = DateTime.Now;
            //foreach (var customerSMSRate in customerSMSRates)
            //{
                //if (customerSMSRate.MobileNetworkID == mobileNetworkID)
                //{
                //    if ((!customerSMSRate.EED.HasValue || customerSMSRate.EED.Value > dateTimeNow) && !customerSMSPreviousEED.BED)
                //    {
                //        customerSMSPreviousEED.Rate = customerSMSRate.Rate;
                //        customerSMSPreviousEED.BED = customerSMSRate.BED;
                //    }

                //    if (customerSMSRate.BED > dateTimeNow && (!customerSMSNextBED.Rate.HasValue || customerSMSRate.BED<(customerSMSNextBED.BED)))
                //    {
                //        customerSMSNextBED.BED = customerSMSRate.BED;
                //        customerSMSNextBED.FutureRate = new SMSFutureRate() { Rate = customerSMSRate.Rate, BED = customerSMSRate.BED };
                //    }
                //}
            //}
        }

        private List<CustomerSMSRateChange> CustomerSMSRateChangesMapper(List<CustomerSMSRate> customerSMSRates)
        {
            return customerSMSRates.Select(item => new CustomerSMSRateChange()
            {
                HasFutureRate = item.HasFutureRate,
                MobileNetworkID = item.MobileNetworkID,
                CurrentRate = item.Rate,
                FutureRate = item.FutureRate

            }).ToList();
        }

        private CustomerSMSRateDetail CustomerSMSRateDetailMapper(CustomerSMSRate customerSMSRate)
        {
            if (customerSMSRate == null)
                return null;

            return new CustomerSMSRateDetail()
            {
                MobileCountryName = GetMobileCountryName(customerSMSRate.MobileNetworkID),
                MobileNetworkName = GetMobileNetworkName(customerSMSRate.MobileNetworkID),
                Rate = customerSMSRate.Rate,
                MobileNetworkID = customerSMSRate.MobileNetworkID,
                BED = customerSMSRate.BED,
                EED = customerSMSRate.EED,
                HasFutureRate = customerSMSRate.HasFutureRate,
                FutureRate = customerSMSRate.FutureRate
            };
        }

        private string GetMobileCountryName(int id)
        {
            return id.ToString() + "_CountryName";
        }

        private string GetMobileNetworkName(int id)
        {
            return id.ToString() + "_NetworkName";
        }

        #endregion

        #region Private Classes

        private class CustomerSMSRateExcelExportHandler : ExcelExportHandler<CustomerSMSRateDetail>
        {
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<CustomerSMSRateDetail> context)
            {
                ExportExcelSheet sheet = new ExportExcelSheet()
                {
                    SheetName = "Customer SMS Rates",
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() },
                    AutoFitColumns = true
                };

                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Country" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Mobile Network", Width = 45 });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Rate", Width = 25 });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "BED", Width = 45 });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "EED", Width = 45 });

                sheet.Rows = new List<ExportExcelRow>();
                if (context.BigResult != null && context.BigResult.Data != null && context.BigResult.Data.Count() > 0)
                {

                    foreach (var record in context.BigResult.Data)
                    {
                        var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                        row.Cells.Add(new ExportExcelCell { Value = record.MobileCountryName });
                        row.Cells.Add(new ExportExcelCell { Value = record.MobileNetworkName });
                        row.Cells.Add(new ExportExcelCell { Value = record.Rate.HasValue ? record.Rate.Value.ToString() : string.Empty });
                        row.Cells.Add(new ExportExcelCell { Value = record.BED });
                        row.Cells.Add(new ExportExcelCell { Value = record.EED });

                        sheet.Rows.Add(row);
                    }
                }

                context.MainSheet = sheet;
            }
        }

        private class CustomerSMSRateRequestHandler : BigDataRequestHandler<CustomerSMSRateQuery, CustomerSMSRate, CustomerSMSRateDetail>
        {
            CustomerSMSRateManager _manager = new CustomerSMSRateManager();

            public override CustomerSMSRateDetail EntityDetailMapper(CustomerSMSRate entity)
            {
                return _manager.CustomerSMSRateDetailMapper(entity);
            }

            public override IEnumerable<CustomerSMSRate> RetrieveAllData(DataRetrievalInput<CustomerSMSRateQuery> input)
            {
                if (input != null && input.Query != null)
                {
                    return _manager.GetCustomerSMSRates(input.Query.CustomerID);
                }

                return null;
            }

            //private List<CustomerSMSRate> MergeMobileNetworks(List<CustomerSMSRate> defaultCustomerSMSRates, int customerID, DateTime? effectiveDate)
            //{
            //    List<CustomerSMSRate> customerSMSRates = _manager.GetCustomerSMSRates(customerID);

            //    if (customerSMSRates == null)
            //        return defaultCustomerSMSRates;


            //    if (defaultCustomerSMSRates != null)
            //    {
            //        DateTime dateTime = effectiveDate.HasValue ? effectiveDate.Value : DateTime.Now;

            //        foreach (var defaultCustomerSMSRate in defaultCustomerSMSRates)
            //        {

            //            SetCustomerSMSRateData(customerSMSRates, defaultCustomerSMSRate, dateTime);
            //        }

            //        return defaultCustomerSMSRates;
            //    }

            //    return null;
            //}

            //private void SetCustomerSMSRateData(List<CustomerSMSRate> customerSMSRates, CustomerSMSRate defaultCustomerSMSRate, DateTime dateTime)
            //{
            //    SMSFutureRate customerSMSFutureRate = new SMSFutureRate();

            //    foreach (var customerSMSRate in customerSMSRates)
            //    {
            //        if (customerSMSRate.MobileNetworkID == defaultCustomerSMSRate.MobileNetworkID)
            //        {
            //            customerSMSRate.IsEffective(dateTime);

            //            if (customerSMSRate.BED<=(dateTime) && (!customerSMSRate.EED.HasValue || customerSMSRate.EED.VRGreaterThan(dateTime)))
            //            {
            //                defaultCustomerSMSRate.ID = customerSMSRate.ID;
            //                defaultCustomerSMSRate.Rate = customerSMSRate.Rate;
            //                defaultCustomerSMSRate.BED = customerSMSRate.BED;
            //                defaultCustomerSMSRate.EED = customerSMSRate.EED;
            //                defaultCustomerSMSRate.PriceListID = customerSMSRate.PriceListID;
            //            }
            //            else if (customerSMSRate.BED > dateTime && (!customerSMSFutureRate.BED.HasValue || customerSMSRate.BED<(customerSMSFutureRate.BED)))
            //            {
            //                customerSMSFutureRate.Rate = customerSMSRate.Rate;
            //                customerSMSFutureRate.BED = customerSMSRate.BED;
            //                customerSMSFutureRate.EED = customerSMSRate.EED;
            //            }
            //        }
            //    }

            //    defaultCustomerSMSRate.HasFutureRate = customerSMSFutureRate.BED.HasValue;
            //    defaultCustomerSMSRate.FutureRate = customerSMSFutureRate;
            //}

            private bool FilterCustomerSMSRates(CustomerSMSRate customerSMSRate, CustomerSMSRateQuery customerSMSRateQuery)
            {
                //if (!customerSMSRate.BED.HasValue)
                //    return true;

                if (customerSMSRateQuery.EffectiveDate>=customerSMSRate.BED && (!customerSMSRate.EED.HasValue || customerSMSRateQuery.EffectiveDate<customerSMSRate.EED))
                    return true;

                return false;
            }

            protected override ResultProcessingHandler<CustomerSMSRateDetail> GetResultProcessingHandler(DataRetrievalInput<CustomerSMSRateQuery> input, BigResult<CustomerSMSRateDetail> bigResult)
            {
                var resultProcessingHandler = new ResultProcessingHandler<CustomerSMSRateDetail>() { ExportExcelHandler = new CustomerSMSRateExcelExportHandler() };
                return resultProcessingHandler;
            }
        }
    }

    #endregion
}
