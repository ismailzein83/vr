using System;
using System.Collections.Generic;
using System.Linq;

namespace TABS
{
    public sealed class SystemConfiguration : Interfaces.ICachedCollectionContainer
    {
        /// <summary>
        /// Needed for marker interface to be called by reflection
        /// </summary>
        public static void ClearCachedCollections()
        {
            _Parameters = null;
            _KnownParameters = null;
            TABS.Components.CacheProvider.Clear(typeof(SystemParameter).FullName);
        }

        private SystemConfiguration() { }

        internal static Dictionary<KnownSystemParameter, SystemParameter> _KnownParameters;
        internal static Dictionary<string, SystemParameter> _Parameters;

        public static Dictionary<KnownSystemParameter, SystemParameter> KnownParameters
        {
            get
            {
                lock (typeof(SystemConfiguration))
                {
                    if (_KnownParameters == null || (_KnownParameters != null && _KnownParameters.Count == 0))
                    {

                        _KnownParameters = new Dictionary<KnownSystemParameter, SystemParameter>();
                        foreach (SystemParameter parameter in Parameters.Values)
                        {
                            try
                            {
                                KnownSystemParameter knownParam = (KnownSystemParameter)Enum.Parse(typeof(KnownSystemParameter), parameter.Name);
                                _KnownParameters.Add(knownParam, parameter);
                            }
                            catch
                            {
                            }
                        }
                        // Check for missing Parameters and add them to the database
                        CheckMissingKnownParameters();
                    }
                }
                return _KnownParameters;
            }
        }

        public static Dictionary<string, SystemParameter> Parameters
        {
            get
            {
                lock (typeof(SystemConfiguration))
                {
                    if (_Parameters == null)
                    {
                        _Parameters = new Dictionary<string, SystemParameter>();
                        IList<SystemParameter> parameters = ObjectAssembler.GetList<SystemParameter>();
                        foreach (SystemParameter parameter in parameters)
                        {
                            _Parameters.Add(parameter.Name, parameter);
                        }
                    }
                }
                return _Parameters;
            }
        }

        delegate void KnownParameterAdder(KnownSystemParameter knowParam, SystemParameterType type, object value);

        /// <summary>
        /// Check for (and create, if necessary) missing Known Parameters.
        /// </summary>
        static void CheckMissingKnownParameters()
        {
            SystemParameter parameter;
            List<SystemParameter> addedParameters = new List<SystemParameter>();

            KnownParameterAdder AddKnowParam = delegate(KnownSystemParameter knowParam, SystemParameterType type, object value)
            {
                parameter = new SystemParameter();
                parameter.Type = type;
                parameter.Name = knowParam.ToString();
                parameter.Value = value;
                KnownParameters.Add(knowParam, parameter);
                Parameters.Add(parameter.Name, parameter);
                addedParameters.Add(parameter);
            };

            foreach (KnownSystemParameter knownParam in Enum.GetValues(typeof(KnownSystemParameter)))
            {
                if (!KnownParameters.ContainsKey(knownParam))
                {
                    switch (knownParam)
                    {
                        // the seeion time out
                        case KnownSystemParameter.SessionTimeOut:
                            AddKnowParam(knownParam, SystemParameterType.Numeric, 0m);
                            break;
                        // The time that the tread of a runnable task waits before checking for the next run
                        case KnownSystemParameter.sys_TaskResolutionTime:
                            AddKnowParam(knownParam, SystemParameterType.TimeSpan, TimeSpan.FromMilliseconds(1109));
                            break;
                        // The Last CDR That was priced, so that pricing will continue afterwards
                        case KnownSystemParameter.sys_CDR_Pricing_CDRID:
                            AddKnowParam(knownParam, SystemParameterType.Numeric, 0m);
                            break;
                        // The Batch Size for CDR Pricing 
                        case KnownSystemParameter.sys_CDR_Pricing_Batch_Size:
                            AddKnowParam(knownParam, SystemParameterType.Numeric, 20000m);
                            break;
                        // The Batch Size for Traffic Monitor Sampling 
                        case KnownSystemParameter.sys_TrafficMonitor_Sample_Size:
                            AddKnowParam(knownParam, SystemParameterType.Numeric, 20000m);
                            break;
                        // The Number of Days after which New Rates are effective (System Default)
                        case KnownSystemParameter.sys_BeginEffectiveRateDays:
                            AddKnowParam(knownParam, SystemParameterType.Numeric, 7m);
                            break;
                        // The Number of Days after which New Rates are effective (System Default)
                        case KnownSystemParameter.sys_BeginEffectiveDecreaseRateDays:
                            AddKnowParam(knownParam, SystemParameterType.Numeric, 0m);
                            break;
                        // The last time the routes were rebuilt
                        case KnownSystemParameter.sys_LastRouteBuild:
                            AddKnowParam(knownParam, SystemParameterType.DateTime, DateTime.Now);
                            break;
                        // The last time the routes were re-synched to switches
                        case KnownSystemParameter.ExternalUserLogo:
                            AddKnowParam(knownParam, SystemParameterType.LongText, null);
                            break;
                        case KnownSystemParameter.CashedTypesToBeCleared:
                            AddKnowParam(knownParam, SystemParameterType.LongText, null);
                            break;
                        case KnownSystemParameter.sys_LastRouteSynch:
                            AddKnowParam(knownParam, SystemParameterType.DateTime, null);
                            break;
                        // The last time the routes were updated by affectors
                        case KnownSystemParameter.sys_LastRouteUpdate:
                            AddKnowParam(knownParam, SystemParameterType.DateTime, null);
                            break;
                        // Traffic Monitor Defaults
                        case KnownSystemParameter.MinimumACD:
                            AddKnowParam(knownParam, SystemParameterType.Numeric, 2m);
                            break;
                        // Traffic Monitor Defaults
                        case KnownSystemParameter.MinimumASR:
                            AddKnowParam(knownParam, SystemParameterType.Numeric, 20m);
                            break;
                        // Traffic Monitor Defaults
                        case KnownSystemParameter.MinimumAttempts:
                            AddKnowParam(knownParam, SystemParameterType.Numeric, 50m);
                            break;
                        // Traffic Stats Samples Per Hour, 4 (Every 15 min)
                        case KnownSystemParameter.TrafficStatsSamplesPerHour:
                            AddKnowParam(knownParam, SystemParameterType.Numeric, 4m);
                            break;
                        // Default SMTP Server
                        case KnownSystemParameter.sys_SMTP_Info:
                            AddKnowParam(knownParam, SystemParameterType.LongText, SystemParameter.DefaultXml);
                            break;
                        //Logging All Events in a page 
                        case KnownSystemParameter.LogAllEvents:
                            AddKnowParam(knownParam, SystemParameterType.Boolean, false);
                            break;
                        //Enable/Disable windows_events logs
                        case KnownSystemParameter.WindowsEventLogger:
                            AddKnowParam(knownParam, SystemParameterType.Boolean, true);
                            break;
                        // Save State backup on Pricelist?
                        case KnownSystemParameter.SaveStateBackupOnPricelist:
                            AddKnowParam(knownParam, SystemParameterType.Boolean, true);
                            break;
                        case KnownSystemParameter.AllowExternalUserToViewToneLogo:
                            AddKnowParam(knownParam, SystemParameterType.Boolean, false);
                            break;
                        // enable /Disable payment terms check 
                        case KnownSystemParameter.EnforcePaymentTermsCheck:
                            AddKnowParam(knownParam, SystemParameterType.Boolean, false);
                            break;
                        // The timespan for Traffic Stats cache
                        case KnownSystemParameter.TrafficStatsCacheTime:
                            AddKnowParam(knownParam, SystemParameterType.TimeSpan, new TimeSpan(3, 0, 0));
                            break;
                        // The banking details for the system account  
                        case KnownSystemParameter.BankingDetails:
                            AddKnowParam(knownParam, SystemParameterType.LongText, SystemParameter.DefaultXml);
                            break;
                        // The Configured CDR Stores
                        case KnownSystemParameter.ConfiguredCDRStores:
                            AddKnowParam(knownParam, SystemParameterType.LongText, TABS.SpecialSystemParameters.CDRStoreDetails.DefaultXml);
                            break;
                        // the mail details (predefined mail structures)  
                        case KnownSystemParameter.MailDetails:
                            AddKnowParam(knownParam, SystemParameterType.LongText, TABS.SpecialSystemParameters.EmailDetails.DefaultXml);
                            break;
                        // invoice serial number format definition 
                        case KnownSystemParameter.InvoiceSerialNumberFormat:
                            AddKnowParam(knownParam, SystemParameterType.Text, "TABS-{1:yyyy}-{10:0000}");
                            break;
                        // Mask invoice serial number format definition 
                        case KnownSystemParameter.MaskInvoiceSerialNumberFormat:
                            AddKnowParam(knownParam, SystemParameterType.Text, "");
                            break;

                        // Email Expression mapping 
                        case KnownSystemParameter.MailExpressions:
                            AddKnowParam(knownParam, SystemParameterType.LongText, SystemParameter.DefaultXml);
                            break;
                        // rate sheet count
                        case KnownSystemParameter.RateSheetCount:
                            AddKnowParam(knownParam, SystemParameterType.Numeric, (decimal)(byte)TABS.RateSheetCount.One_Sheet);
                            break;
                        // rate sheet Formatting
                        case KnownSystemParameter.FormattedRateSheet:
                            AddKnowParam(knownParam, SystemParameterType.Boolean, true);
                            break;
                        // zone codes view  
                        case KnownSystemParameter.ZoneCodesView:
                            AddKnowParam(knownParam, SystemParameterType.Numeric, (decimal)(byte)TABS.CodeView.Row_for_each_Code);
                            break;
                        // Is Route Rebuild Required 
                        case KnownSystemParameter.IsRouteBuildRequired:
                            AddKnowParam(knownParam, SystemParameterType.Boolean, false);
                            break;
                        // The Store  "Send Confiramtion Mail Invoice" option 
                        case KnownSystemParameter.SendConfiramtionMail_Invoice:
                            AddKnowParam(knownParam, SystemParameterType.Boolean, false);
                            break;
                        // The Store  "Send Confiramtion Mail Pricelist" option 
                        case KnownSystemParameter.SendConfiramtionMail_Pricelist:
                            AddKnowParam(knownParam, SystemParameterType.Boolean, false);
                            break;
                        // The Store  "show System Alerts" option
                        case KnownSystemParameter.ShowSystemAlerts:
                            AddKnowParam(knownParam, SystemParameterType.Boolean, false);
                            break;
                        // Show Name Suffix of carrier account
                        case KnownSystemParameter.ShowNameSuffix:
                            AddKnowParam(knownParam, SystemParameterType.Boolean, false);
                            break;
                        // Show Name Suffix of carrier account for pricelist and invoice
                        case KnownSystemParameter.ShowNameSuffixForPriceListAndInvoice:
                            AddKnowParam(knownParam, SystemParameterType.Boolean, false);
                            break;
                        // refresh system  alerts 
                        case KnownSystemParameter.RefreshableAlertsPeriod:
                            AddKnowParam(knownParam, SystemParameterType.TimeSpan, TimeSpan.FromSeconds(60));
                            break;
                        // Post-Paid Customer Options
                        case KnownSystemParameter.PostPaidCarrierOptions:
                            AddKnowParam(knownParam, SystemParameterType.LongText, TABS.SpecialSystemParameters.PostPaidCarrierOptions.DefaultXml);
                            break;
                        // Pre-Paid Customer Options
                        case KnownSystemParameter.PrepaidCarrierOptions:
                            AddKnowParam(knownParam, SystemParameterType.LongText, TABS.SpecialSystemParameters.PrepaidCarrierOptions.DefaultXml);
                            break;
                        // Minimum Same Action Email interval (Not to send emails recurrently)
                        case KnownSystemParameter.MinimumActionEmailInterval:
                            AddKnowParam(knownParam, SystemParameterType.TimeSpan, TimeSpan.FromDays(1));
                            break;
                        // pricelist file name format 
                        case KnownSystemParameter.PricelistNameFormat:
                            AddKnowParam(knownParam, SystemParameterType.Text, "TABS-{0}-{1:ddMMyy}");
                            break;
                        // Overall Invoices Startup Counter 
                        case KnownSystemParameter.Invoices_Overall_Startup_Counter:
                            AddKnowParam(knownParam, SystemParameterType.Numeric, 0.0m);
                            break;
                        // Overall Mask Invoices Startup Counter 
                        case KnownSystemParameter.Mask_Invoices_Overall_Startup_Counter:
                            AddKnowParam(knownParam, SystemParameterType.Numeric, 0.0m);
                            break;
                        // The Maximum time to wait before a CDR import from a switch is considered timed-out.
                        case KnownSystemParameter.CDR_Import_TimeOut:
                            AddKnowParam(knownParam, SystemParameterType.TimeSpan, TimeSpan.FromMinutes(5));
                            break;
                        // Customers A-Z Code Changes
                        case KnownSystemParameter.FullCodeChangesCustomers:
                            AddKnowParam(knownParam, SystemParameterType.Text, TABS.CarrierAccount.Customers
                                                                                   .Select(c => c.CarrierAccountID)
                                                                                   .Aggregate((c1, c2) => c1 + ',' + c2));
                            break;
                        // Customers Country Code Changes
                        case KnownSystemParameter.CountryCodeChangesCustomers:
                            AddKnowParam(knownParam, SystemParameterType.Text, TABS.CarrierAccount.Customers
                                                                                   .Select(c => c.CarrierAccountID)
                                                                                   .Aggregate((c1, c2) => c1 + ',' + c2));
                            break;
                        // Pricing Thread Count
                        case KnownSystemParameter.PricingThreadCount:
                            AddKnowParam(knownParam, SystemParameterType.Numeric, (decimal)2);
                            break;

                        // Rates Digits After Zero
                        case KnownSystemParameter.RatesDigitsAfterDot:
                            AddKnowParam(knownParam, SystemParameterType.Numeric, (decimal)5);
                            break;
                        // customer Exclusion zones 
                        case KnownSystemParameter.CustomerExcludedZones:
                            AddKnowParam(knownParam, SystemParameterType.LongText, SpecialSystemParameters.ExclusionZone.DefaultXml);
                            break;

                        case KnownSystemParameter.FaultTicketReferenceFormat:
                            AddKnowParam(knownParam, SystemParameterType.Text, "VAN");
                            break;

                        case KnownSystemParameter.MaxSuppliersPerRoute:
                            AddKnowParam(knownParam, SystemParameterType.Numeric, 10);
                            break;

                        case KnownSystemParameter.sys_MaxRate_SaleCost:
                            AddKnowParam(knownParam, SystemParameterType.Numeric, 10);
                            break;

                        case KnownSystemParameter.CustomRateSheetCode:
                            AddKnowParam(knownParam, SystemParameterType.LongText,
                                        @"public byte[] GetPricelistWorkbook(TABS.PriceList pricelist) 
                                            {
                                                TABS.Addons.PriceListExport.ExcelPricelistGenerator exporter = new TABS.Addons.PriceListExport.ExcelPricelistGenerator(); 
                                                return exporter.GetPricelistWorkbook(pricelist);
                                            }");

                            break;
                        case KnownSystemParameter.CustomCodeSupplierTargetAnalysis:
                            AddKnowParam(knownParam, SystemParameterType.LongText,
                                        @"public byte[] GetSupplierTargetAnalysisWorkbook(DataTable DataSource, DateTime fromDate, DateTime toDate, bool WithAvreges)
                                            {
                                                TABS.Addons.TargetAnalysisExport.ExcelTargetAnalysisGenerator exporter = new TABS.Addons.TargetAnalysisExport.ExcelTargetAnalysisGenerator(); 
                                                return exporter.GetWorkbook(DataSource, fromDate, toDate, WithAvreges);
                                            }");
                            break;

                        //billing Invoice Custom Code
                        case KnownSystemParameter.BillingInvoiceCustomCode:
                        case KnownSystemParameter.SupplierBillingInvoiceCustomCode:
                            AddKnowParam(knownParam, SystemParameterType.LongText,
                                        @"public Telerik.Reporting.IReportDocument GetBillingInvoice(TABS.Billing_Invoice Invoice) 
                                            {
                                                       TABS.Reports.RptInvoiceV2 report;
                                                   
                                                       report =new TABS.Reports.RptInvoiceV2(Invoice);
                                                    
                                                    report.InvoiceDetails.ReportSource.DataSource = Invoice.Billing_Invoice_Details;
                                                    ((TABS.Reports.RptInVoiceDetail)report.InvoiceDetails.ReportSource).BillingInvoice = Invoice;
                                                    return (IReportDocument)report;
                                            }");

                            break;
                        case KnownSystemParameter.BillingInvoiceCustomCodeGrouped:
                            AddKnowParam(knownParam, SystemParameterType.LongText,
                                        @"public Telerik.Reporting.IReportDocument GetBillingInvoice(TABS.Billing_Invoice Invoice) 
                                            {
                                                  TABS.Reports.RptGroupedInvoice report;
                                                   
                                                       report =new TABS.Reports.RptGroupedInvoice(Invoice);
                                                    report.InvoiceDetails.ReportSource.DataSource = Invoice.Billing_Invoice_Details;
                                                     ((TABS.Reports.RptGroupedInVoiceDetail)report.InvoiceDetails.ReportSource).BillingInvoice = Invoice;
                                                    return (IReportDocument)report;
                                            }");

                            break;

                        // Billing Invoice Info
                        case KnownSystemParameter.BillingInvoiceInfoCustomCode:
                            AddKnowParam(knownParam, SystemParameterType.LongText,
                                        @"public Telerik.Reporting.IReportDocument GetBillingInvoice(TABS.Billing_Invoice Invoice) 
                                            {
                                                       TABS.Reports.RptInvoiceInfo report;
                                                   
                                                       report = new TABS.Reports.RptInvoiceInfo(Invoice);
                                                    
                                                    //report.InvoiceDetails.ReportSource.DataSource = Invoice.Billing_Invoice_Details;
                                                    //((TABS.Reports.RptInVoiceDetail)report.InvoiceDetails.ReportSource).BillingInvoice = Invoice;
                                                    return (IReportDocument)report;
                                            }");
                            break;
                        // Billing Invoice Details
                        case KnownSystemParameter.BillingInvoiceDetailsCustomCode:
                            AddKnowParam(knownParam, SystemParameterType.LongText,
                                      @"public Telerik.Reporting.IReportDocument GetBillingInvoice(TABS.Billing_Invoice Invoice) 
                                            {
                                                       TABS.Reports.RptInvoiceDetails report; 
                                                   
                                                       report = new TABS.Reports.RptInvoiceDetails();
                                                    
                                                    //report.InvoiceDetails.ReportSource.DataSource = Invoice.Billing_Invoice_Details;
                                                    //((TABS.Reports.RptInVoiceDetail)report.InvoiceDetails.ReportSource).BillingInvoice = Invoice;
                                                    
                                                    report.DataSource = Invoice.Billing_Invoice_Details;
                                                    report.BillingInvoice = Invoice;                    

                                                    return (IReportDocument)report;
                                                                        
                                            }");
                            break;
                        // Billing Invoice Details Grouped
                        case KnownSystemParameter.BillingInvoiceDetailsGroupedCustomCode:
                            AddKnowParam(knownParam, SystemParameterType.LongText,
                                        @"public Telerik.Reporting.IReportDocument GetBillingInvoice(TABS.Billing_Invoice Invoice) 
                                            {
                                                    TABS.Reports.RptInvoiceDetailsGrouped report; 
                                                   
                                                       report = new TABS.Reports.RptInvoiceDetailsGrouped();
                                                    
                                                    //report.InvoiceDetails.ReportSource.DataSource = Invoice.Billing_Invoice_Details;
                                                    //((TABS.Reports.RptInVoiceDetail)report.InvoiceDetails.ReportSource).BillingInvoice = Invoice;
                                                    
                                                    report.DataSource = Invoice.Billing_Invoice_Details;
                                                    report.BillingInvoice = Invoice;                    

                                                    return (IReportDocument)report;
                                                                        
                                            }");
                            break;

                        case KnownSystemParameter.CodeChangesSheetCustomCode:
                            AddKnowParam(knownParam, SystemParameterType.LongText,
                                        @"public byte[] GetCodeSheetWorkbook(IEnumerable<ZoneCodeNotes> data, CarrierAccount Customer, DateTime EffectiveDate, SecurityEssentials.User CusrrentUser) 
                                            {
                                                TABS.Addons.PriceListExport.ExcelCodeSheetGenerator exporter = new TABS.Addons.PriceListExport.ExcelCodeSheetGenerator(); 
                                                return exporter.GetCodeSheetWorkbook(data, Customer, EffectiveDate, CusrrentUser);
                                            }");

                            break;
                        //case KnownSystemParameter.CommandtimeOut:
                        //    AddKnowParam(knownParam, SystemParameterType.Text,(3*60).ToString());

                        //    break;
                        case KnownSystemParameter.ZoneSheetCustomCode:
                            AddKnowParam(knownParam, SystemParameterType.LongText,
                                        @"public byte[] GetZoneSheet(List<Zone> zones, SecurityEssentials.User CusrrentUser)
                                            {
                                                TABS.Addons.PriceListExport.ExcelZoneSheetGenerator exporter = new TABS.Addons.PriceListExport.ExcelZoneSheetGenerator(); 
                                                return exporter.GetZoneSheet(zones, CusrrentUser);
                                            }");
                            break;
                        case KnownSystemParameter.Billing_CDR_Invalid_MaxId:
                            lock (typeof(Billing_CDR_Invalid))
                            {
                                decimal invalidMaxId = decimal.Parse(DataHelper.ExecuteScalar("SELECT ISNULL(MAX(ID),0) FROM Billing_CDR_Invalid (NOLOCK)").ToString());
                                AddKnowParam(knownParam, SystemParameterType.Numeric, invalidMaxId);
                            }
                            break;

                        case KnownSystemParameter.Switch_Job_Queue_String:
                            AddKnowParam(knownParam, SystemParameterType.Text, "");
                            break;

                        case KnownSystemParameter.Include_Blocked_Zones_In_ZoneRates:
                            AddKnowParam(knownParam, SystemParameterType.Boolean, false);
                            break;

                        case KnownSystemParameter.SetForcedRoute_First_In_Routing:
                            AddKnowParam(knownParam, SystemParameterType.Boolean, false);
                            break;

                        case KnownSystemParameter.Daily_Stats_Monitor_Email_To:
                            AddKnowParam(knownParam, SystemParameterType.Text, "");
                            break;

                        case KnownSystemParameter.ShowCarriers:
                            AddKnowParam(knownParam, SystemParameterType.Boolean, true);
                            break;

                        case KnownSystemParameter.Daily_Stats_Monitor_PercentageDiff:
                            AddKnowParam(knownParam, SystemParameterType.Numeric, 0);
                            break;

                        // Default PriceList Import POP Server
                        case KnownSystemParameter.sys_PriceList_Import_POP_Info:
                            AddKnowParam(knownParam, SystemParameterType.LongText, SystemParameter.DefaultXml);
                            break;

                        // Enable/Disable Supplier Pricelist Import Currency Change by user
                        case KnownSystemParameter.Allow_Pricelist_Import_Currency_Change:
                            AddKnowParam(knownParam, SystemParameterType.Boolean, true);
                            break;

                        // Enable/Disable Rate Plan Currency Change by user
                        case KnownSystemParameter.Allow_Rate_Plan_Currency_Change:
                            AddKnowParam(knownParam, SystemParameterType.Boolean, true);
                            break;

                        // Customer pricelist save policy
                        case KnownSystemParameter.Pricelist_Save_Only_Changes:
                            AddKnowParam(knownParam, SystemParameterType.Boolean, true);
                            break;
                        // SMTP client timeout in milliseconds
                        case KnownSystemParameter.SMTP_Timeout:
                            AddKnowParam(knownParam, SystemParameterType.Numeric, 5);
                            break;
                        // SMTP IIS method
                        case KnownSystemParameter.SMTP_GetDirectoryFromIIS:
                            AddKnowParam(knownParam, SystemParameterType.Boolean, false);
                            break;
                        case KnownSystemParameter.Password_Strength:
                            AddKnowParam(knownParam, SystemParameterType.Numeric, 0);
                            break;
                        case KnownSystemParameter.EnforceCheckForUnpricedCDRs:
                            AddKnowParam(knownParam, SystemParameterType.Boolean, true);
                            break;
                        case KnownSystemParameter.IncludeCustomerRegistrationNumberInInvoice:
                            AddKnowParam(knownParam, SystemParameterType.Boolean, false);
                            break;
                        case KnownSystemParameter.IncludeCodeRangesInCustomPricelist:
                            AddKnowParam(knownParam, SystemParameterType.Boolean, true);
                            break;
                        case KnownSystemParameter.CodeComparisonOptions:
                            AddKnowParam(knownParam, SystemParameterType.LongText, string.Empty);
                            break;
                        case KnownSystemParameter.CheckForUnsoldedZonesAndBlocks:
                            AddKnowParam(knownParam, SystemParameterType.Boolean, false);
                            break;
                        case KnownSystemParameter.CompressSheetsInCodePreperation:
                            AddKnowParam(knownParam, SystemParameterType.Boolean, true);
                            break;
                        case KnownSystemParameter.Routing_Table_FileGroup:
                            AddKnowParam(knownParam, SystemParameterType.Text, "PRIMARY");
                            break;
                        case KnownSystemParameter.Routing_Indexes_FileGroup:
                            AddKnowParam(knownParam, SystemParameterType.Text, "PRIMARY");
                            break;
                        case KnownSystemParameter.Routing_Sort_Indexes_In_Temp:
                            AddKnowParam(knownParam, SystemParameterType.Boolean, true);
                            break;
                        case KnownSystemParameter.HidePrepaidPostpaidInactiveCarrierAccounts:
                            AddKnowParam(knownParam, SystemParameterType.Boolean, false);
                            break;
                        case KnownSystemParameter.RateLossRunnableParameters:
                            //AddKnowParam(knownParam, SystemParameterType.LongText, SystemParameter.DefaultXml);
                            AddKnowParam(knownParam, SystemParameterType.LongText, TABS.SpecialSystemParameters.RateLossRunnableParameters.DefaultXml);
                            break;
                        case KnownSystemParameter.TOneLicenseCheckerParameter:
                            AddKnowParam(knownParam, SystemParameterType.LongText, TABS.SpecialSystemParameters.TOneLicenseChecker.defaultXml);
                            break;
                        case KnownSystemParameter.MaskCarrierAccount:
                            AddKnowParam(knownParam, SystemParameterType.LongText, TABS.SpecialSystemParameters.MaskAccount.DefaultXml);
                            break;
                        case KnownSystemParameter.sys_RateChangeLastDate:
                            AddKnowParam(knownParam, SystemParameterType.DateTime, DateTime.Now);
                            break;
                        case KnownSystemParameter.sys_RatePoolLastClear:
                            AddKnowParam(knownParam, SystemParameterType.DateTime, DateTime.Now);
                            break;
                        case KnownSystemParameter.sys_CustomerServialGeneratorInfo:
                            AddKnowParam(knownParam, SystemParameterType.LongText, TABS.SpecialSystemParameters.CustomerInvoiceSerialGenerator.DefaultXml);
                            break;
                        case KnownSystemParameter.Invoices_Yearly_Startup_Counter:
                            AddKnowParam(knownParam, SystemParameterType.Numeric, 0);
                            break;
                        case KnownSystemParameter.EnForcePercentageInRouting:
                            AddKnowParam(knownParam, SystemParameterType.Boolean, false);
                            break;
                        case KnownSystemParameter.AllowCostZoneCalculationFromCDPNOut:
                            AddKnowParam(knownParam, SystemParameterType.Boolean, false);
                            break;
                        case KnownSystemParameter.sys_ExactMatchProcessing:
                            AddKnowParam(knownParam, SystemParameterType.Boolean, false);
                            break;
                        case KnownSystemParameter.ForceSaveNotesOnRouteChange:
                            AddKnowParam(knownParam, SystemParameterType.Boolean, false);
                            break;
                        case KnownSystemParameter.sys_Rate_Separator:
                            AddKnowParam(knownParam, SystemParameterType.Text, ".");
                            break;
                        case KnownSystemParameter.MobilePatterns:
                            AddKnowParam(knownParam, SystemParameterType.Text, "mob;mobil;mobile");
                            break;
                        case KnownSystemParameter.NameSeperator:
                            AddKnowParam(knownParam, SystemParameterType.Text, ",|;|_|-");
                            break;
                        case KnownSystemParameter.Mct_ApplyTo:
                            AddKnowParam(knownParam, SystemParameterType.Numeric, 3);
                            break;
                        case KnownSystemParameter.Mct_ApplyToSupplier:
                            AddKnowParam(knownParam, SystemParameterType.Text, "");
                            break;
                        case KnownSystemParameter.Mct_DefaultMarginPercentage:
                            AddKnowParam(knownParam, SystemParameterType.Boolean, true);
                            break;
                        case KnownSystemParameter.Mct_MaximumMargin:
                            AddKnowParam(knownParam, SystemParameterType.Numeric, 10);
                            break;
                        case KnownSystemParameter.Mct_MinimumMargin:
                            AddKnowParam(knownParam, SystemParameterType.Numeric, 1);
                            break;
                        case KnownSystemParameter.Mct_PositiveInfinity:
                            AddKnowParam(knownParam, SystemParameterType.Numeric, 100);
                            break;
                        case KnownSystemParameter.RatePlan_AllowPartialPricelist:
                            AddKnowParam(knownParam, SystemParameterType.Boolean, true);
                            break;
                        case KnownSystemParameter.RatePlan_AllowPricelistInvalidRates:
                            AddKnowParam(knownParam, SystemParameterType.Boolean, false);
                            break;
                        case KnownSystemParameter.RatePlan_SendNotificationInvalidRates:
                            AddKnowParam(knownParam, SystemParameterType.Boolean, true);
                            break;
                        case KnownSystemParameter.ApprovalRequest_NoitifyInDirectRequest:
                            AddKnowParam(knownParam, SystemParameterType.Boolean, true);
                            break;
                        case KnownSystemParameter.ApprovalRequest_NotifyDirectRequest:
                            AddKnowParam(knownParam, SystemParameterType.Boolean, true);
                            break;
                        case KnownSystemParameter.ApprovalRequest_NotifyRequestResponse:
                            AddKnowParam(knownParam, SystemParameterType.Boolean, true);
                            break;
                        case KnownSystemParameter.ApprovalRequest_SendTimeOutNoification:
                            AddKnowParam(knownParam, SystemParameterType.Boolean, true);
                            break;
                        case KnownSystemParameter.ApprovalRequest_TimeOut:
                            AddKnowParam(knownParam, SystemParameterType.TimeSpan, new TimeSpan(0,0,0,0));
                            break;
                        case KnownSystemParameter.ApprovalRequest_TimeOutAction:
                            AddKnowParam(knownParam, SystemParameterType.Numeric, 1);
                            break;
                        case KnownSystemParameter.ApprovalRequest_TimeOutActionHours:
                            AddKnowParam(knownParam, SystemParameterType.Numeric, 0);
                            break;
               

                    }
                }
            }
            if (addedParameters.Count > 0)
            {
                Exception ex;
                using (NHibernate.ISession session = DataConfiguration.OpenSession())
                {
                    ObjectAssembler.SaveXorUpdate(session, addedParameters, out ex, true); // Use "Save" since this is an insert.
                }
            }
        }

        public static string GetRateFormat()
        {
            int zeros = (int)SystemParameter.RatesDigitsAfterDot.NumericValue.Value;
            return "0".PadRight(zeros, '0');
        }
    }
}