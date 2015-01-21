using System;

namespace TABS
{
    public class SystemParameter : Components.BaseEntity
    {
        public override string Identifier { get { return "SystemParameter:" + Name; } }

        private string _Name;
        private string _Description;
        private SystemParameterType _Type;
        private string _TextValue;
        private bool? _BooleanValue;
        private decimal? _NumericValue;
        private DateTime? _DateTimeValue;
        private string _TimeSpanValue;
        private string _LongTextValue;

        public virtual string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        public virtual string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        public virtual SystemParameterType Type
        {
            get { return _Type; }
            set { _Type = value; }
        }

        public virtual bool? BooleanValue
        {
            get { return _BooleanValue; }
            set { _BooleanValue = value; }
        }

        public virtual decimal? NumericValue
        {
            get { return _NumericValue; }
            set { _NumericValue = value; }
        }

        public virtual DateTime? DateTimeValue
        {
            get { return _DateTimeValue; }
            set { _DateTimeValue = value; }
        }

        public virtual TimeSpan? TimeSpanValue
        {
            get { return _TimeSpanValue == null ? default(TimeSpan?) : TimeSpan.Parse(_TimeSpanValue); }
            set { _TimeSpanValue = value == null ? null : value.ToString(); }
        }

        public virtual string TextValue
        {
            get { return _TextValue; }
            set { _TextValue = value; }
        }

        public virtual string LongTextValue
        {
            get { return _LongTextValue; }
            set { _LongTextValue = value; }
        }

        public virtual object Value
        {
            get
            {
                switch (Type)
                {
                    case SystemParameterType.Boolean: return BooleanValue;
                    case SystemParameterType.LongText: return LongTextValue;
                    case SystemParameterType.Numeric: return NumericValue;
                    case SystemParameterType.TimeSpan: return TimeSpanValue;
                    case SystemParameterType.DateTime: return DateTimeValue;
                    default: return TextValue;
                }
            }
            set
            {
                switch (Type)
                {
                    case SystemParameterType.Boolean: BooleanValue = value == null ? (bool?)null : bool.Parse(value.ToString()); break;
                    case SystemParameterType.LongText: LongTextValue = value == null ? null : value.ToString(); break;
                    case SystemParameterType.Numeric: NumericValue = value == null ? (decimal?)null : decimal.Parse(value.ToString()); break;
                    case SystemParameterType.TimeSpan: TimeSpanValue = value == null ? (TimeSpan?)null : TimeSpan.Parse(value.ToString()); break;
                    case SystemParameterType.DateTime: DateTimeValue = value == null ? (DateTime?)null : DateTime.Parse(value.ToString()); break;
                    default: TextValue = value == null ? "" : value.ToString(); break;
                }
            }
        }

        /// <summary>
        /// Returns the value of the Parameter as a string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Value == null ? null : Value.ToString();
        }

        public static SystemParameter operator <=(SystemParameter parameter, object value)
        {
            parameter.Value = value;
            return parameter;
        }

        public static SystemParameter operator >=(SystemParameter parameter, object value)
        {
            parameter.Value = value;
            return parameter;
        }

        public static bool operator ==(SystemParameter parameter, object value)
        {
            if (value is SystemParameter) return parameter.Equals(value);
            if (parameter.Equals(null) || parameter.Value == null) return value == null;
            else return parameter.Value.Equals(value);
        }

        public static bool operator !=(SystemParameter parameter, object value)
        {
            return !(parameter == value);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static double GetEffectiveperiod()
        {
            return double.Parse(TABS.SystemConfiguration.KnownParameters[TABS.KnownSystemParameter.sys_BeginEffectiveRateDays].Value.ToString());
        }
        public static SystemParameter WindowsEventLogger
        {
            get
            {
                return SystemConfiguration.KnownParameters[KnownSystemParameter.WindowsEventLogger];
            }
        }
        /// <summary>
        /// The last Route Build
        /// </summary>
        public static SystemParameter LastRouteBuild
        {
            get
            {
                return SystemConfiguration.KnownParameters[KnownSystemParameter.sys_LastRouteBuild];
            }
        }

        /// <summary>
        /// The Last Route Synch to switches
        /// </summary>
        public static SystemParameter LastRouteSynch
        {
            get
            {
                return SystemConfiguration.KnownParameters[KnownSystemParameter.sys_LastRouteSynch];
            }
        }

        public static SystemParameter ExternalUserLogo
        {
            get
            {

                return SystemConfiguration.KnownParameters[KnownSystemParameter.ExternalUserLogo];
            }
        }

        public static SystemParameter CashedTypesToBeCleared
        {

            get
            {
                return SystemConfiguration.KnownParameters[KnownSystemParameter.CashedTypesToBeCleared];
            }
        }


        /// <summary>
        /// Enable Pricelist State Backup
        /// </summary>
        public static SystemParameter SaveStateBackupOnPricelist
        {
            get
            {
                return SystemConfiguration.KnownParameters[KnownSystemParameter.SaveStateBackupOnPricelist];
            }
        }

        public static SystemParameter ForceSaveNotesOnRouteChange
        {
            get
            {
                return SystemConfiguration.KnownParameters[KnownSystemParameter.ForceSaveNotesOnRouteChange];
            }
        }

        public static SystemParameter AllowExternalUserToViewToneLogo
        {

            get
            {

                return SystemConfiguration.KnownParameters[KnownSystemParameter.AllowExternalUserToViewToneLogo];
            }
        }

        /// <summary>
        /// Enable Pricelist State Backup
        /// </summary>
        public static SystemParameter EnforcePaymentTermsCheck
        {
            get
            {
                return SystemConfiguration.KnownParameters[KnownSystemParameter.EnforcePaymentTermsCheck];
            }
        }

        /// <summary>
        /// The Last Route Update (Affectors: ToD, Special Request, Route Blocks)
        /// </summary>
        public static SystemParameter LastRouteUpdate
        {
            get
            {
                return SystemConfiguration.KnownParameters[KnownSystemParameter.sys_LastRouteUpdate];
            }
        }

        /// <summary>
        /// The Default SMTP Server that system uses to send emails
        /// </summary>
        public static SystemParameter SMTP_Info
        {
            get
            {
                return SystemConfiguration.KnownParameters[KnownSystemParameter.sys_SMTP_Info];
            }
        }
        /// <summary>
        /// SMS Info the user uses to send Text Messages(SMS)
        /// </summary>
        public static SystemParameter SMS_Info
        {
            get 
            {
                return SystemConfiguration._KnownParameters[KnownSystemParameter.sys_SMS_Info];
            }
        }

        /// <summary>
        /// The Default PriceList Import POP3 Server that system uses to receive emails
        /// </summary>
        public static SystemParameter PriceList_Import_POP_Info
        {
            get
            {
                return SystemConfiguration.KnownParameters[KnownSystemParameter.sys_PriceList_Import_POP_Info];
            }
        }

        /// <summary>
        /// The TimeSpan for Traffic Stats Cache
        /// </summary>
        public static SystemParameter TrafficStatsCacheTime
        {
            get
            {
                return SystemConfiguration.KnownParameters[KnownSystemParameter.TrafficStatsCacheTime];
            }
        }

        /// <summary>
        /// return the banking details for the system parameter  
        /// </summary>
        public static SystemParameter BankingDetails
        {
            get
            {
                return SystemConfiguration.KnownParameters[KnownSystemParameter.BankingDetails];
            }
        }

        public static readonly string DefaultXml = @"<?xml version=""1.0"" encoding=""utf-8"" ?><SystemParameter></SystemParameter>";

        /// <summary>
        /// return the mail details for the system parameter  
        /// </summary>
        public static SystemParameter MailDetails
        {
            get
            {
                return SystemConfiguration.KnownParameters[KnownSystemParameter.MailDetails];
            }
        }
        /// <summary>
        /// return the format for the invoice serial number 
        /// </summary>
        public static SystemParameter InvoiceSerialNumberFormat
        {
            get
            {
                return SystemConfiguration.KnownParameters[KnownSystemParameter.InvoiceSerialNumberFormat];
            }
        }

        /// <summary>
        /// return the pricelist file name format 
        /// </summary>
        public static SystemParameter PricelistNameFormat
        {
            get
            {
                return SystemConfiguration.KnownParameters[KnownSystemParameter.PricelistNameFormat];
            }
        }
        /// <summary>
        /// return the Expression Mapping parameter
        /// </summary>
        public static SystemParameter MailExpressions
        {
            get
            {
                return SystemConfiguration.KnownParameters[KnownSystemParameter.MailExpressions];
            }
        }

        /// <summary>
        /// return the rate sheet option (one sheet or 2 sheets or ...)
        /// </summary>
        public static SystemParameter RateSheetCount
        {
            get
            {
                return SystemConfiguration.KnownParameters[KnownSystemParameter.RateSheetCount];
            }
        }

        /// <summary>
        /// return the rate sheet formatted option
        /// </summary>
        public static SystemParameter FormattedRateSheet
        {
            get
            {
                return SystemConfiguration.KnownParameters[KnownSystemParameter.FormattedRateSheet];
            }
        }

        /// <summary>
        /// return the zone codes view in rate sheet parameter 
        /// </summary>
        public static SystemParameter CodeView
        {
            get { return SystemConfiguration.KnownParameters[KnownSystemParameter.ZoneCodesView]; }
        }

        public static SystemParameter IsRouteBuildRequired
        {
            get { return SystemConfiguration.KnownParameters[KnownSystemParameter.IsRouteBuildRequired]; }
        }

        /// <summary>
        /// enable sending confirmation  mails when generating an invoice 
        /// </summary>
        public static SystemParameter SendConfiramtionMail_Invoice
        {
            get
            {
                return SystemConfiguration.KnownParameters[KnownSystemParameter.SendConfiramtionMail_Invoice];
            }
        }
        /// <summary>
        /// enbale sending  confiramation mails when importing a supplier pricelist
        /// </summary>
        public static SystemParameter SendConfiramtionMail_Pricelist
        {
            get
            {
                return SystemConfiguration.KnownParameters[KnownSystemParameter.SendConfiramtionMail_Pricelist];
            }
        }
        /// <summary>
        /// enbale/disable System alerts 
        /// </summary>
        public static SystemParameter ShowSystemAlerts
        {
            get
            {
                return SystemConfiguration.KnownParameters[KnownSystemParameter.ShowSystemAlerts];
            }
        }

        /// <summary>
        /// enbale/disable Name Suffix of the carrier account 
        /// </summary>
        public static SystemParameter ShowNameSuffix
        {
            get
            {
                return SystemConfiguration.KnownParameters[KnownSystemParameter.ShowNameSuffix];
            }
        }

        /// <summary>
        /// enbale/disable Name Suffix of the carrier account in the pricelist and invoice
        /// </summary>
        public static SystemParameter ShowNameSuffixForPriceListAndInvoice
        {
            get
            {
                return SystemConfiguration.KnownParameters[KnownSystemParameter.ShowNameSuffixForPriceListAndInvoice];
            }
        }

        /// <summary>
        /// enbale/disable Objects Customers and Suppliers
        /// </summary>
        public static SystemParameter ShowCarriers
        {
            get
            {
                return SystemConfiguration.KnownParameters[KnownSystemParameter.ShowCarriers];
            }
        }


        /// <summary>
        /// refresh alert time 
        /// </summary>
        public static SystemParameter RefreshableAlertsPeriod
        {
            get
            {
                return SystemConfiguration.KnownParameters[KnownSystemParameter.RefreshableAlertsPeriod];
            }
        }

        /// <summary>
        /// Daily Stats Monitor Email To
        /// </summary>
        public static SystemParameter Daily_Stats_Monitor_Email_To { get { return SystemConfiguration.KnownParameters[KnownSystemParameter.Daily_Stats_Monitor_Email_To]; } }

        /// <summary>
        /// Daily Stats Monitor Percentage of Difference
        /// </summary>
        public static SystemParameter Daily_Stats_Monitor_PercentageDiff { get { return SystemConfiguration.KnownParameters[KnownSystemParameter.Daily_Stats_Monitor_PercentageDiff]; } }

        /// <summary>
        /// The Configured CDR Stores
        /// </summary>
        public static SystemParameter ConfiguredCDRStores
        {
            get { return SystemConfiguration.KnownParameters[KnownSystemParameter.ConfiguredCDRStores]; }
        }

        public static SystemParameter PostPaidCarrierOptions
        {
            get { return SystemConfiguration.KnownParameters[KnownSystemParameter.PostPaidCarrierOptions]; }
        }

        public static SystemParameter PrepaidCarrierOptions
        {
            get { return SystemConfiguration.KnownParameters[KnownSystemParameter.PrepaidCarrierOptions]; }
        }

        public static SystemParameter MinimumActionEmailInterval
        {
            get { return SystemConfiguration.KnownParameters[KnownSystemParameter.MinimumActionEmailInterval]; }
        }

        public static SystemParameter CustomRateSheetCode
        {
            get { return SystemConfiguration.KnownParameters[KnownSystemParameter.CustomRateSheetCode]; }
        }

        public static SystemParameter CustomCodeSupplierTargetAnalysis
        {
            get { return SystemConfiguration.KnownParameters[KnownSystemParameter.CustomCodeSupplierTargetAnalysis]; }
        }

        public static SystemParameter TrafficStatsSamplesPerHour { get { return SystemConfiguration.KnownParameters[KnownSystemParameter.TrafficStatsSamplesPerHour]; } }

        public static SystemParameter CDR_Import_TimeOut { get { return SystemConfiguration.KnownParameters[KnownSystemParameter.CDR_Import_TimeOut]; } }

        /// <summary>
        /// Overall Invoices Startup Counter. Will be added to the overall number of invoices when using the
        /// "All Invoices Count" value in the invoice serial format.
        /// </summary>
        public static SystemParameter Invoices_Overall_Startup_Counter { get { return SystemConfiguration.KnownParameters[KnownSystemParameter.Invoices_Overall_Startup_Counter]; } }

        public static SystemParameter Mask_Invoices_Overall_Startup_Counter { get { return SystemConfiguration.KnownParameters[KnownSystemParameter.Mask_Invoices_Overall_Startup_Counter]; } }

        public static SystemParameter FullCodeChangesCustomers { get { return SystemConfiguration.KnownParameters[KnownSystemParameter.FullCodeChangesCustomers]; } }

        public static SystemParameter CountryCodeChangesCustomers { get { return SystemConfiguration.KnownParameters[KnownSystemParameter.CountryCodeChangesCustomers]; } }

        public static SystemParameter PricingThreadCount { get { return SystemConfiguration.KnownParameters[KnownSystemParameter.PricingThreadCount]; } }

        public static SystemParameter RatesDigitsAfterDot { get { return SystemConfiguration.KnownParameters[KnownSystemParameter.RatesDigitsAfterDot]; } }

        public static SystemParameter CustomerExcludedZones { get { return SystemConfiguration.KnownParameters[KnownSystemParameter.CustomerExcludedZones]; } }

        public static SystemParameter FaultTicketReferenceFormat { get { return SystemConfiguration.KnownParameters[KnownSystemParameter.FaultTicketReferenceFormat]; } }

        public static SystemParameter MaxSuppliersPerRoute { get { return SystemConfiguration.KnownParameters[KnownSystemParameter.MaxSuppliersPerRoute]; } }

        public static SystemParameter Billing_CDR_Invalid_MaxId { get { return SystemConfiguration.KnownParameters[KnownSystemParameter.Billing_CDR_Invalid_MaxId]; } }

        public static SystemParameter Include_Blocked_Zones_In_ZoneRates { get { return SystemConfiguration.KnownParameters[KnownSystemParameter.Include_Blocked_Zones_In_ZoneRates]; } }

        public static SystemParameter SetForcedRoute_First_In_Routing { get { return SystemConfiguration.KnownParameters[KnownSystemParameter.SetForcedRoute_First_In_Routing]; } }

        //-------
        public static SystemParameter SessionTimeOut { get { return SystemConfiguration.KnownParameters[KnownSystemParameter.SessionTimeOut]; } }
        //-------

        public static SystemParameter Allow_Pricelist_Import_Currency_Change { get { return SystemConfiguration.KnownParameters[KnownSystemParameter.Allow_Pricelist_Import_Currency_Change]; } }

        public static SystemParameter Allow_Rate_Plan_Currency_Change { get { return SystemConfiguration.KnownParameters[KnownSystemParameter.Allow_Rate_Plan_Currency_Change]; } }

        public static SystemParameter Pricelist_Save_Only_Changes { get { return SystemConfiguration.KnownParameters[KnownSystemParameter.Pricelist_Save_Only_Changes]; } }

        public static SystemParameter HidePrepaidPostpaidInactiveCarrierAccounts { get { return SystemConfiguration.KnownParameters[KnownSystemParameter.HidePrepaidPostpaidInactiveCarrierAccounts]; } }

        public static SystemParameter SMTP_Timeout { get { return SystemConfiguration.KnownParameters[KnownSystemParameter.SMTP_Timeout]; } }

        public static SystemParameter SMTP_GetDirectoryFromIIS { get { return SystemConfiguration.KnownParameters[KnownSystemParameter.SMTP_GetDirectoryFromIIS]; } }

        public static SystemParameter Password_Strength { get { return SystemConfiguration.KnownParameters[KnownSystemParameter.Password_Strength]; } }

        public static SystemParameter EnforceCheckForUnpricedCDRs { get { return SystemConfiguration.KnownParameters[KnownSystemParameter.EnforceCheckForUnpricedCDRs]; } }

        public static SystemParameter IncludeCustomerRegistrationNumberInInvoice { get { return SystemConfiguration.KnownParameters[KnownSystemParameter.IncludeCustomerRegistrationNumberInInvoice]; } }

        public static SystemParameter IncludeCodeRangesInCustomPricelist { get { return SystemConfiguration.KnownParameters[KnownSystemParameter.IncludeCodeRangesInCustomPricelist]; } }

        public static SystemParameter CodeComparisonOptions { get { return SystemConfiguration.KnownParameters[KnownSystemParameter.CodeComparisonOptions]; } }

        public static SystemParameter CheckForUnsoldedZonesAndBlocks { get { return SystemConfiguration.KnownParameters[KnownSystemParameter.CheckForUnsoldedZonesAndBlocks]; } }

        public static SystemParameter Routing_Table_FileGroup { get { return SystemConfiguration.KnownParameters[KnownSystemParameter.Routing_Table_FileGroup]; } }

        public static SystemParameter Routing_Indexes_FileGroup { get { return SystemConfiguration.KnownParameters[KnownSystemParameter.Routing_Indexes_FileGroup]; } }

        public static SystemParameter Routing_Sort_Indexes_In_Temp { get { return SystemConfiguration.KnownParameters[KnownSystemParameter.Routing_Sort_Indexes_In_Temp]; } }

        public static SystemParameter CompressSheetsInCodePreperation { get { return SystemConfiguration.KnownParameters[KnownSystemParameter.CompressSheetsInCodePreperation]; } }

        public static SystemParameter RateLossRunnableParameters { get { return SystemConfiguration.KnownParameters[KnownSystemParameter.RateLossRunnableParameters]; } }

        public static SystemParameter TOneLicenseCheckerParameter { get { return SystemConfiguration.KnownParameters[KnownSystemParameter.TOneLicenseCheckerParameter]; } }

        public static SystemParameter CodeChangesSheetCustomCode { get { return SystemConfiguration.KnownParameters[KnownSystemParameter.CodeChangesSheetCustomCode]; } }

        public static SystemParameter ZoneSheetCustomCode { get { return SystemConfiguration.KnownParameters[KnownSystemParameter.ZoneSheetCustomCode]; } }

        public static SystemParameter MaskCarrierAccount { get { return SystemConfiguration.KnownParameters[KnownSystemParameter.MaskCarrierAccount]; } }

        public static SystemParameter sys_RateChangeLastDate { get { return SystemConfiguration.KnownParameters[KnownSystemParameter.sys_RateChangeLastDate]; } }

        public static SystemParameter sys_RatePoolLastClear { get { return SystemConfiguration.KnownParameters[KnownSystemParameter.sys_RatePoolLastClear]; } }

        public static SystemParameter MaskInvoiceSerialNumberFormat { get { return SystemConfiguration.KnownParameters[KnownSystemParameter.MaskInvoiceSerialNumberFormat]; } }

        public static SystemParameter sys_CustomerServialGeneratorInfo { get { return SystemConfiguration.KnownParameters[KnownSystemParameter.sys_CustomerServialGeneratorInfo]; } }

        public static SystemParameter Invoices_Yearly_Startup_Counter { get { return SystemConfiguration.KnownParameters[KnownSystemParameter.Invoices_Yearly_Startup_Counter]; } }

        public static SystemParameter EnForcePercentageInRouting { get { return SystemConfiguration.KnownParameters[KnownSystemParameter.EnForcePercentageInRouting]; } }

        public static SystemParameter AllowCostZoneCalculationFromCDPNOut { get { return SystemConfiguration.KnownParameters[KnownSystemParameter.AllowCostZoneCalculationFromCDPNOut]; } }

        public static SystemParameter MaxRateSaleCost { get { return SystemConfiguration.KnownParameters[KnownSystemParameter.sys_MaxRate_SaleCost]; } }

        public static SystemParameter RateSeparator { get { return SystemConfiguration.KnownParameters[KnownSystemParameter.sys_Rate_Separator]; } }

        public static SystemParameter MobilePatterns { get { return SystemConfiguration.KnownParameters[KnownSystemParameter.MobilePatterns]; } }
        public static SystemParameter NameSeperator { get { return SystemConfiguration.KnownParameters[KnownSystemParameter.NameSeperator]; } }



        public static SystemParameter BillingInvoiceCustomCode { 
            get { return SystemConfiguration.KnownParameters[KnownSystemParameter.BillingInvoiceCustomCode]; }
            set { SystemConfiguration.KnownParameters[KnownSystemParameter.BillingInvoiceCustomCode].Value = value; ; }
        
        }

        public static SystemParameter SupplierBillingInvoiceCustomCode
        {
            get { return SystemConfiguration.KnownParameters[KnownSystemParameter.SupplierBillingInvoiceCustomCode]; }
            set { SystemConfiguration.KnownParameters[KnownSystemParameter.SupplierBillingInvoiceCustomCode].Value = value; ; }

        }

        public static SystemParameter BillingInvoiceCustomCodeGrouped
        {
            get { return SystemConfiguration.KnownParameters[KnownSystemParameter.BillingInvoiceCustomCodeGrouped]; }
            set { SystemConfiguration.KnownParameters[KnownSystemParameter.BillingInvoiceCustomCodeGrouped].Value = value; ; }

        }
        public static SystemParameter ExactMatch_Processing { get { return SystemConfiguration.KnownParameters[KnownSystemParameter.sys_ExactMatchProcessing]; } }


        public static SystemParameter Mct_ApplyTo
        {
            get { return SystemConfiguration.KnownParameters[KnownSystemParameter.Mct_ApplyTo]; }
            set { SystemConfiguration.KnownParameters[KnownSystemParameter.Mct_ApplyTo].Value = value; }
        }

        public static SystemParameter Mct_ApplyToSupplier
        {
            get { return SystemConfiguration.KnownParameters[KnownSystemParameter.Mct_ApplyToSupplier]; }
            set { SystemConfiguration.KnownParameters[KnownSystemParameter.Mct_ApplyToSupplier].Value = value; }
        }

        public static SystemParameter Mct_DefaultMarginPercentage
        {
            get { return SystemConfiguration.KnownParameters[KnownSystemParameter.Mct_DefaultMarginPercentage]; }
            set { SystemConfiguration.KnownParameters[KnownSystemParameter.Mct_DefaultMarginPercentage].Value = value; }
        }

        public static SystemParameter Mct_MaximumMargin
        {
            get { return SystemConfiguration.KnownParameters[KnownSystemParameter.Mct_MaximumMargin]; }
            set { SystemConfiguration.KnownParameters[KnownSystemParameter.Mct_MaximumMargin].Value = value; }
        }

        public static SystemParameter Mct_MinimumMargin
        {
            get { return SystemConfiguration.KnownParameters[KnownSystemParameter.Mct_MinimumMargin]; }
            set { SystemConfiguration.KnownParameters[KnownSystemParameter.Mct_MinimumMargin].Value = value; }
        }

        public static SystemParameter Mct_PositiveInfinity
        {
            get { return SystemConfiguration.KnownParameters[KnownSystemParameter.Mct_PositiveInfinity]; }
            set { SystemConfiguration.KnownParameters[KnownSystemParameter.Mct_PositiveInfinity].Value = value; }
        }

        public static SystemParameter RatePlan_AllowPartialPricelist
        {
            get { return SystemConfiguration.KnownParameters[KnownSystemParameter.RatePlan_AllowPartialPricelist]; }
            set { SystemConfiguration.KnownParameters[KnownSystemParameter.RatePlan_AllowPartialPricelist].Value = value; }
        }

        public static SystemParameter RatePlan_AllowPricelistInvalidRates
        {
            get { return SystemConfiguration.KnownParameters[KnownSystemParameter.RatePlan_AllowPricelistInvalidRates]; }
            set { SystemConfiguration.KnownParameters[KnownSystemParameter.RatePlan_AllowPricelistInvalidRates].Value = value; }
        }

        public static SystemParameter RatePlan_SendNotificationInvalidRates
        {
            get { return SystemConfiguration.KnownParameters[KnownSystemParameter.RatePlan_SendNotificationInvalidRates]; }
            set { SystemConfiguration.KnownParameters[KnownSystemParameter.RatePlan_SendNotificationInvalidRates].Value = value; }
        }

        public static SystemParameter ApprovalRequest_NoitifyInDirectRequest
        {
            get { return SystemConfiguration.KnownParameters[KnownSystemParameter.ApprovalRequest_NoitifyInDirectRequest]; }
            set { SystemConfiguration.KnownParameters[KnownSystemParameter.ApprovalRequest_NoitifyInDirectRequest].Value = value; }
        }

        public static SystemParameter ApprovalRequest_NotifyDirectRequest
        {
            get { return SystemConfiguration.KnownParameters[KnownSystemParameter.ApprovalRequest_NotifyDirectRequest]; }
            set { SystemConfiguration.KnownParameters[KnownSystemParameter.ApprovalRequest_NotifyDirectRequest].Value = value; }
        }

        public static SystemParameter ApprovalRequest_NotifyRequestResponse
        {
            get { return SystemConfiguration.KnownParameters[KnownSystemParameter.ApprovalRequest_NotifyRequestResponse]; }
            set { SystemConfiguration.KnownParameters[KnownSystemParameter.ApprovalRequest_NotifyRequestResponse].Value = value; }
        }

        public static SystemParameter ApprovalRequest_SendTimeOutNoification
        {
            get { return SystemConfiguration.KnownParameters[KnownSystemParameter.ApprovalRequest_SendTimeOutNoification]; }
            set { SystemConfiguration.KnownParameters[KnownSystemParameter.ApprovalRequest_SendTimeOutNoification].Value = value; }
        }

        public static SystemParameter ApprovalRequest_TimeOut
        {
            get { return SystemConfiguration.KnownParameters[KnownSystemParameter.ApprovalRequest_TimeOut]; }
            set { SystemConfiguration.KnownParameters[KnownSystemParameter.ApprovalRequest_TimeOut].Value = value; }
        }

        public static SystemParameter ApprovalRequest_TimeOutAction
        {
            get { return SystemConfiguration.KnownParameters[KnownSystemParameter.ApprovalRequest_TimeOutAction]; }
            set { SystemConfiguration.KnownParameters[KnownSystemParameter.ApprovalRequest_TimeOutAction].Value = value; }
        }

        public static SystemParameter ApprovalRequest_TimeOutActionHours
        {
            get { return SystemConfiguration.KnownParameters[KnownSystemParameter.ApprovalRequest_TimeOutActionHours]; }
            set { SystemConfiguration.KnownParameters[KnownSystemParameter.ApprovalRequest_TimeOutActionHours].Value = value; }
        }


        //public static SystemParameter CommandTimeOut
        //{
        //    get { return SystemConfiguration.KnownParameters[KnownSystemParameter.CommandtimeOut]; }
        //    set { SystemConfiguration.KnownParameters[KnownSystemParameter.CommandtimeOut].Value = value; }

        //}
    }
}
