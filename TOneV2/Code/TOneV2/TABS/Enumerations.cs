using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace TABS
{
    /// <summary>
    /// Supplier Rate policy defined in rate planning
    /// </summary>
    /// 
    public enum LicenseType : byte
    {
        SLA,
        Volume
    }

    public enum AutoGenerateInvoice : byte
    {
        Weekly = 1,
        BiWeekly,
        SemiMonthly,
        Monthly,
        BiMonthly,
        Period,
        SpecificDay
    }

    public enum PeriodType : byte
    {
        Hourly,
        Daily,
        Weekly,
        Monthly
    }

    /// <summary>
    /// password strenght
    /// </summary>
    public enum PasswordStrength : byte
    {
        Weak,
        Medium,
        Strong
    }

    /// <summary>
    /// Supplier Rate policy defined in rate planning
    /// </summary>
    public enum SupplierRatePolicy : byte
    {
        None,
        Highest_Rate,
        Lowest_Rate
    }
    /// <summary>
    /// Margin Status for InValid Initail Status
    /// </summary>
    public enum MarginStatus : int
    {
        None=0,
        Valid=1,
        InValid=2,
        Approved=3,
        ApprovedIncreased=4,
        Pended=5,
        Disapproved=6
    }
    /// <summary>
    /// Pricing Template Types
    /// </summary>
    public enum TemplateType : byte
    {
        Pricing,
        TargetAnalysis,
        Margin_Control
    }


    /// <summary>
    /// object type in manual intervention page
    /// </summary>
    public enum ObjectType : byte
    {
        Zone,
        Code,
        Rate
    }
    /// <summary>
    /// Code change 
    /// </summary>
    public enum CodeChangeType : byte
    {
        Unchanged = 0,
        Removed = 1,
        New = 2
    }

    /// <summary>
    /// data type for the worksheet 
    /// </summary>
    public enum XlsDataType : byte
    {
        Data = 0,
        Image = 1
    }

    /// <summary>
    /// rate Sheet options
    /// </summary>
    public enum RateSheetCount : byte
    {
        One_Sheet = 0,
        Two_Sheets = 1
    }

    /// <summary>
    /// rate Sheet options
    /// </summary>
    public enum CodeView : byte
    {
        Comma_Seperated = 0,
        Row_for_each_Code = 1
    }

    /// <summary>
    /// Rate Separator
    /// </summary>
    public enum RateSeparator : byte
    {
        Comma = 0,
        Period = 1
    }


    /// <summary>
    /// Mail Template Type
    /// </summary>
    public enum MailTemplateType : byte
    {
        Pricelist,
        Invoice,
        InvoiceSupplier,
        InvoiceCustomerExceededDueDate,
        Alert,
        TroubleTicketIN,
        PricelistImport,
        InvoiceGeneration,
        PrepaidCustomerAction,
        PrepaidCustomerBlockAction,
        PostpaidCustomerAction,
        PostpaidCustomerBlockAction,
        TroubleTicketOUT,
        TroubleTicketCloseOUT,
        TroubleTicketInUpdate,
        TroubleTicketOutUpdate,
        EATroubleTicketIN,
        EATroubleTicketOUT,
        EATroubleTicketInUpdate,
        EATroubleTicketOutUpdate,
        PrepaidSupplierAction,
        PostpaidSupplierAction,
        PostpaidSupplierBlockAction,
        BillReminder,
        PostpaidSMSSupplier,
        PostpaidSMSCustomer,
        PrepaidSMSCustomer,

        SMSCustomerPayment,
        MailCustomerPayment,
        SMSSupplierPayment,
        MailSupplierPayment,

        UserCreationNotification
    }

    /// <summary>
    /// Export Format
    /// </summary>
    public enum ExportFormat : byte
    {
        CSV = 0,
        EXCEL = 1,
        //PDF = 2,
        WORD = 3

    }
    /// <summary>
    /// Ticket Fault Staus
    /// </summary>
    public enum TicketStatus : byte
    {
        Open = 0,
        Pending = 1,
        Closed = 2

    }

    /// <summary>
    /// Ticket type Enumation
    /// </summary>
    public enum TicketType : byte
    {
        IN = 0,
        OUT = 1
    }

    /// <summary>
    /// the rate reference of applied pricing template 
    /// </summary>
    public enum PricingReferenceRate : byte
    {
        LCR1 = 0,
        LCR2 = 1,
        LCR3 = 2,
        AVG = 3,
        CR = 4,
        Supplier = 5
    }
    /// <summary>
    /// Rate type  
    /// </summary>
    public enum RateType : byte
    {
        Normal = 0,
        OffPeak = 1,
        Weekend = 2
    }
    /// <summary>
    /// Payment Terms for Carrier profiles.
    /// </summary>
    public enum PaymentTerms : byte
    {
        Unspecified = 0,
        Weekly = 7,
        Every_10_Days = 10,
        Every_2_Weeks = 15,
        Monthly = 30
    }

    /// <summary>
    /// Activation status for a carrier account
    /// </summary>
    public enum ActivationStatus : byte
    {
        Inactive = 0,
        Testing = 1,
        Active = 2
    }

    /// <summary>
    /// Route Status
    /// </summary>
    public enum RoutingStatus : byte
    {
        Blocked = 0,
        BlockedInbound = 1,
        BlockedOutbound = 2,
        Enabled = 3
    }

    /// <summary>
    /// The account type based on traffic direction
    /// </summary>
    public enum AccountType : byte
    {
        Client = 0,     // Client only
        Exchange = 1,   // Client and Supply
        Termination = 2 // Supply Only
    }

    /// <summary>
    /// Payment Type for a certain carrier account
    /// </summary>
    public enum PaymentType : byte
    {
        Postpaid = 0,
        Prepaid = 1,
        Undefined = 100,
        Defined_By_Profile = 200
    }

    /// <summary>
    /// Prepaid Amount Type
    /// </summary>
    public enum AmountType : byte
    {
        /// <summary>
        /// Payment Type (Just for the record)
        /// </summary>
        Payment = 0,
        /// <summary>
        /// Billing Type, calculated by the system from Billing Stats
        /// </summary>
        Billing = 1,
        /// <summary>
        /// The Opening Balance of a Customer / Supplier
        /// </summary>
        OpeningBalance = 2,
        /// <summary>
        /// Adjustment Balance of a Customer / Supplier
        /// </summary>
        Adjustment = 3,
        /// <summary>
        /// Sum of old amount of a Customer / Supplier
        /// </summary>
        PreviousBalance = 4
    }

    /// <summary>
    /// The Carrier Account Connection type (TDM or VOIP)
    /// </summary>
    public enum ConnectionType : byte
    {
        TDM = 0,
        VoIP = 1
    }

    /// <summary>
    /// Route Block Type
    /// </summary>
    public enum RouteBlockType : byte
    {
        Permanent,
        Temporary
    }

    /// <summary>
    /// The rate type for Time of Day considerations (ToD)
    /// </summary>
    public enum ToDRateType : byte
    {
        Normal = 0,
        OffPeak = 1,
        Weekend = 2,
        Holiday = 4
    }

    /// <summary>
    /// Deal type 
    /// </summary>
    public enum DealType : byte
    {
        partial = 0,
        total = 1
    }

    /// <summary>
    /// The Status for a route option 
    /// </summary>
    public enum RouteOptionState : byte
    {
        Blocked = 0,
        Enabled = 1
    }

    /// <summary>
    /// Indicate the status of a route
    /// </summary>
    public enum RouteState : byte
    {
        Blocked = 0,
        Enabled = 1
    }

    /// <summary>
    /// The Lock level for a route (or other entity)
    /// </summary>
    public enum LockLevel : byte
    {
        None,
        System,
        User
    }

    /// <summary>
    /// The type of Special Request
    /// </summary>
    public enum SpecialRequestType : byte
    {
        HighestPriorityRoute,
        ForcedRoute
    }

    public enum ItemChangeType
    {
        ItemAdded,
        ItemRemoved
    }

    /// <summary>
    /// Gisad connection type PSTN or Voip 
    /// </summary>

    public enum ConnectionCallType : byte
    {
        PSTN = 0,
        Voip = 1
    }


    /// <summary>
    /// 
    /// </summary>
    public enum PriceListType
    {
        /// <summary>
        /// A Pricelist with full listing of zones and codes.
        /// </summary>
        Full_Pricelist = 0,
        /// <summary>
        /// A full country pricelist. This means that zones and codes not listed are deleted
        /// </summary>
        Country_Pricelist,
        /// <summary>
        /// A Pricelist with changed rates only
        /// </summary>
        Rate_Change_Pricelist,
        /// <summary>
        /// A Pricelist with full full country, except for the supplier's own zones, which are rate changes.
        /// </summary>
        Mixed_Full_Country_And_Changes
    }

    /// <summary>
    /// The Import Parameter type
    /// </summary>
    public enum ImportParameterType : byte
    {
        /// <summary>
        /// A Text (string) valued parameter
        /// </summary>
        Text,
        /// <summary>
        /// An integer parameter
        /// </summary>
        Integer,
        /// <summary>
        /// A boolean (Checkbox?)
        /// </summary>
        Boolean,
        /// <summary>
        /// A Decimal parameter (allow decimal point)
        /// </summary>
        Decimal,
        /// <summary>
        /// A selection from text values. The "DefaultValue" property should hold a
        /// object[] array (string[]?). The first value will be selected by default.
        /// </summary>
        Selection,
        /// <summary>
        /// A Currency Selection Parameter
        /// </summary>
        Currency
    }

    /// <summary>
    /// The System Parameter type
    /// </summary>
    public enum SystemParameterType : byte
    {
        Text = 0,
        Numeric = 1,
        DateTime = 2,
        TimeSpan = 3,
        Boolean = 4,
        LongText = 5
    }

    /// <summary>
    /// Known System Parameters
    /// </summary>
    public enum KnownSystemParameter : byte
    {
        /// <summary>
        /// Session tine out
        /// </summary>
        SessionTimeOut,
        /// <summary>
        /// Enables/Disables windows_event logs
        /// </summary>
        WindowsEventLogger,
        /// <summary>
        /// Task Resolution time (time to wait before checking if a task is worth running)
        /// </summary>
        sys_TaskResolutionTime,
        /// <summary>
        /// The last priced CDR (in Billing Main)
        /// </summary>
        sys_CDR_Pricing_CDRID,
        /// <summary>
        /// The number of CDRs to "Batch" in pricing
        /// </summary>
        sys_CDR_Pricing_Batch_Size,
        /// <summary>
        /// The sample size from which to generate Traffic Stats
        /// </summary>
        sys_TrafficMonitor_Sample_Size,
        /// <summary>
        /// The Number of Days after which New Rates are effective (System Default)
        /// </summary>
        sys_BeginEffectiveRateDays,
        /// <summary>
        /// The Number of Days after which New Rates (Decrease) are effective (System Default)
        /// </summary>
        sys_BeginEffectiveDecreaseRateDays,
        /// <summary>
        /// The last time routes were rebuilt
        /// </summary>
        sys_LastRouteBuild,
        /// <summary>
        /// The last route update (by affectors like ToD, Special Requests and Route Blocks)
        /// </summary>
        sys_LastRouteUpdate,
        /// <summary>
        /// The last route synch to switches
        /// </summary>
        sys_LastRouteSynch,
        /// <summary>
        /// The default SMTP Information (xml) the system uses to send emails
        /// </summary>
        sys_SMTP_Info,
        /// <summary>
        ///  Threeshold Definition For MinimumAttempts 
        /// </summary>
        MinimumAttempts,
        /// <summary>
        ///  Threeshold Definition For MinimumASR
        /// </summary>
        MinimumASR,
        /// <summary>
        ///  Threeshold Definition For MinimumACD
        /// </summary>
        MinimumACD,
        /// <summary>
        /// Save state backup on each pricelist generation
        /// </summary>
        /// 
        AllowExternalUserToViewToneLogo,

        ExternalUserLogo,

        CashedTypesToBeCleared,

        SaveStateBackupOnPricelist,
        /// <summary>
        /// Enable or disable Logging events on pages 
        /// </summary>
        LogAllEvents,
        /// <summary>
        /// The timespan for Traffic Stats cache
        /// </summary>
        TrafficStatsCacheTime,
        /// <summary>
        /// used to enforce check on customer payment terms 
        /// </summary>
        EnforcePaymentTermsCheck,
        /// <summary>
        /// Banking Details 
        /// </summary>
        BankingDetails,
        /// <summary>
        /// Mail Details 
        /// </summary>
        MailDetails,
        /// <summary>
        /// Invoice Serial Number Format
        /// </summary>
        InvoiceSerialNumberFormat,
        /// <summary>
        /// Mask Invoice Serial Number Format
        /// </summary>
        MaskInvoiceSerialNumberFormat,
        /// <summary>
        /// Mail Expression mapping
        /// </summary>
        MailExpressions,
        /// <summary>
        /// rate sheet options
        /// </summary>
        RateSheetCount,
        /// <summary>
        /// Codes  
        /// </summary>
        ZoneCodesView,
        /// <summary>
        /// Signal if a Route Rebuild is required
        /// </summary>
        IsRouteBuildRequired,
        /// <summary>
        /// Configured CDR Stores
        /// </summary>
        ConfiguredCDRStores,
        /// <summary>
        /// to enable sending a confirmation mail when generate an invoice 
        /// </summary>
        SendConfiramtionMail_Invoice,
        /// <summary>
        /// to enable sending a confirmation mail when importing a pricelist   
        /// </summary>
        SendConfiramtionMail_Pricelist,
        /// <summary>
        /// hide/unhide system alerts  
        /// </summary>
        ShowSystemAlerts,
        /// <summary>
        /// hide/unhide Name Suffix of carrier account 
        /// </summary>
        ShowNameSuffix,
        /// hide/unhide Name Suffix of carrier account for pricelist and invoice
        /// </summary>
        ShowNameSuffixForPriceListAndInvoice,
        /// <summary>
        /// hide/unhide Carriers Customers and Suppliers special for GISAD
        /// </summary>
        ShowCarriers,
        /// set the period of refrashable alerts 
        /// </summary>
        RefreshableAlertsPeriod,
        /// <summary>
        /// Holds Post Paid Customer Options
        /// </summary>
        PostPaidCarrierOptions,
        /// <summary>
        /// Holds  Prepaid Paid Customer Options
        /// </summary>
        PrepaidCarrierOptions,
        /// <summary>
        /// Minimum Same Action Email interval (Not to send emails recurrently)
        /// </summary>
        MinimumActionEmailInterval,
        /// <summary>
        /// return the pricelist file name format 
        /// </summary>
        PricelistNameFormat,
        /// <summary>
        /// The Number Traffic Stats Samples per Hour
        /// </summary>
        TrafficStatsSamplesPerHour,
        /// <summary>
        /// The overall invoices startup counter. This number will be added to the total number of invoices
        /// in the database for issuing invoices to customer when using the "All Invoices Count" in the format.
        /// </summary>
        Invoices_Overall_Startup_Counter,
        Invoices_Yearly_Startup_Counter,
        /// <summary>
        /// The overall invoices startup counter. This number will be added to the total number of invoices
        /// in the database for issuing invoices to customer when using the "All Invoices Count" in the format.
        /// </summary>
        Mask_Invoices_Overall_Startup_Counter,
        /// <summary>
        /// The Maximum time to wait before a CDR import from a switch is considered timed-out.
        /// </summary>
        CDR_Import_TimeOut,
        /// <summary>
        /// a list of customers (carrieraccountID) A-Z code changes
        /// </summary>
        FullCodeChangesCustomers,
        /// <summary>
        /// a list of customers (carrieraccountID) Country code changes
        /// </summary>
        CountryCodeChangesCustomers,
        /// <summary>
        /// The Number of Parallel Threads to Run when doing the pricing operation
        /// </summary>
        PricingThreadCount,
        /// <summary>
        /// The number of digits rates have after the zero for display, export, etc
        /// </summary>
        RatesDigitsAfterDot,
        /// <summary>
        /// set of customer -> excluded zones 
        /// </summary>
        CustomerExcludedZones,
        /// <summary>
        /// Own Reference In the Fault Ticket
        /// </summary>
        FaultTicketReferenceFormat,
        /// <summary>
        /// Max Number of Suppliers per Route
        /// </summary>
        MaxSuppliersPerRoute,
        /// <summary>
        /// formatted rate sheet 
        /// </summary>
        FormattedRateSheet,
        /// <summary>
        ///  Custom code for ratesheet generator
        /// </summary>
        CustomRateSheetCode,
        /// <summary>
        ///  Custom code for ratesheet generator
        /// </summary>
        CustomCodeSupplierTargetAnalysis,
        /// <summary>
        /// The max Id for Billing_CDR_Invalid table
        /// </summary>
        Billing_CDR_Invalid_MaxId,
        /// <summary>
        /// The string containing the list of jobs for switches, used by 
        /// </summary>
        Switch_Job_Queue_String,
        /// <summary>
        /// Whether or not to include Blocked Supplier Zones to the Zone Rates Table
        /// </summary>
        Include_Blocked_Zones_In_ZoneRates,
        /// <summary>
        /// Put the forced route as first in routing 
        /// </summary>
        SetForcedRoute_First_In_Routing,
        /// <summary>
        /// Daily Stats Monitor Email To
        /// </summary>
        Daily_Stats_Monitor_Email_To,
        /// <summary>
        /// Daily Stats Monitor Percentage of Difference
        /// </summary>
        Daily_Stats_Monitor_PercentageDiff,
        /// <summary>
        /// Pricelist Import Mail - POP Info: server and credentials
        /// </summary>
        sys_PriceList_Import_POP_Info,
        /// <summary>
        /// Enable/Disable Supplier Pricelist Import Currency Change by user
        /// </summary>
        Allow_Pricelist_Import_Currency_Change,
        /// <summary>
        /// Enable/Disable Rate Plan Currency Change by user
        /// </summary>
        Allow_Rate_Plan_Currency_Change,        
        /// <summary>
        /// Check for unpriced CDRs when issuing an invoice
        /// </summary>
        Check_Unpriced_CDRs_When_Issuing_Invoice,
        /// <summary>
        /// rate plan save policy
        /// </summary>
        Pricelist_Save_Only_Changes,
        /// <summary>
        /// Smtp timeout
        /// </summary>
        SMTP_Timeout,
        /// <summary>
        /// SMTP IIS directory method
        /// </summary>
        SMTP_GetDirectoryFromIIS,

        /// <summary>
        /// user's password strength
        /// </summary>
        Password_Strength,
        /// <summary>
        /// user's password strength
        /// </summary>
        EnforceCheckForUnpricedCDRs,
        /// <summary>
        /// Include registration number in invoice 
        /// </summary>
        IncludeCustomerRegistrationNumberInInvoice,
        /// <summary>
        /// Comma seperated codes without range
        /// </summary>
        IncludeCodeRangesInCustomPricelist,
        /// <summary>
        /// Options for code comparison
        /// </summary>
        CodeComparisonOptions,
        /// <summary>
        /// Enable/disable in routeoverride 
        /// </summary>
        CheckForUnsoldedZonesAndBlocks,
        /// <summary>
        /// chhose to compress or uncompress sheets in code preparation
        /// </summary>
        CompressSheetsInCodePreperation,
        /// <summary>
        /// The SQL partition where routing tables should be on
        /// </summary>
        Routing_Table_FileGroup,
        /// <summary>
        /// The SQL partition where routing indexes should be on
        /// </summary>
        Routing_Indexes_FileGroup,
        /// <summary>
        /// ON/OFF whether the routing indexes should be built using Temp DB
        /// </summary>
        Routing_Sort_Indexes_In_Temp,
        /// <summary>
        /// Hide Inactive carrier account in prepaid postpaid
        /// </summary>
        HidePrepaidPostpaidInactiveCarrierAccounts,
        /// <summary>
        /// rate loss parameters (xml) inxluded customers, suppliers and sale zones
        /// </summary>
        RateLossRunnableParameters,
        /// <summary>
        /// rate loss parameters (xml) inxluded customers, suppliers and sale zones
        /// </summary>
        
        /// </summary>
        /// If should add reason/notes on Route Override,
        /// </summary>
        ForceSaveNotesOnRouteChange,

        TOneLicenseCheckerParameter,

        CodeChangesSheetCustomCode,

        ZoneSheetCustomCode,

        MaskCarrierAccount,

        sys_RateChangeLastDate,

        sys_RatePoolLastClear,

        sys_CustomerServialGeneratorInfo,

        EnForcePercentageInRouting,
        /// <summary>
        /// Billing invoice custom code 
        /// </summary>
        sys_ExactMatchProcessing,
        BillingInvoiceCustomCode,
        SupplierBillingInvoiceCustomCode,
        BillingInvoiceCustomCodeGrouped,
        BillingInvoiceInfoCustomCode,
        BillingInvoiceDetailsCustomCode,
        BillingInvoiceDetailsGroupedCustomCode,
        AllowCostZoneCalculationFromCDPNOut,
        sys_SMS_Info,
        //CommandtimeOut,
        sys_MaxRate_SaleCost,
        sys_Rate_Separator,

        // Target Analysis
        MobilePatterns,
        NameSeperator,
        
        Mct_PositiveInfinity,
        Mct_MinimumMargin,
        Mct_MaximumMargin,
        Mct_DefaultMarginPercentage,
        Mct_ApplyTo,
        Mct_ApplyToSupplier,

        RatePlan_AllowPricelistInvalidRates,
        RatePlan_SendNotificationInvalidRates,
        RatePlan_AllowPartialPricelist,

        ApprovalRequest_NotifyDirectRequest,
        ApprovalRequest_NoitifyInDirectRequest,
        ApprovalRequest_NotifyRequestResponse,
        ApprovalRequest_TimeOut,
        ApprovalRequest_TimeOutAction,
        ApprovalRequest_TimeOutActionHours,
        ApprovalRequest_SendTimeOutNoification

    }

    /// <summary>
    /// Types of Runnable Tasks
    /// </summary>
    public enum RunnableTaskSchedule : byte
    {
        None,
        Startup,
        Scheduled_Once,
        Scheduled_Hourly,
        Scheduled_Daily,
        Scheduled_Weekly,
        Scheduled_Monthly,
        Scheduled_Custom
    }

    /// <summary>
    /// The type of runnable Task for persisted runnable tasks
    /// </summary>
    public enum RunnableTaskType : byte
    {
        /// <summary>
        /// A Database Command (Stored Procedure or SQL Text) 
        /// </summary>
        DatabaseCommand,
        /// <summary>
        /// A Runnable class (Implements IRunnable), the full Class Name (with assembly) should be provided
        /// </summary>
        KnownIRunnableClass,
        /// <summary>
        /// A Runnable Task with custom "Code" defined in its configuration
        /// </summary>
        CustomCodeRunnableTask
    }

    /// <summary>
    /// The types of Route updates 
    /// </summary>
    public enum RouteSynchType : byte
    {
        Full,
        Differential//,
        //Route_Updates_ResetAll = 15
    }

    /// <summary>
    /// The State Backup Type
    /// </summary>
    public enum StateBackupType : byte
    {
        /// <summary>
        /// A full backup for all customers and suppliers
        /// </summary>
        Full,
        /// <summary>
        /// A Partial backup, for a given Customer
        /// </summary>
        Customer,
        /// <summary>
        /// A Partial backup, for a given Supplier
        /// </summary>
        Supplier
    }

    /// <summary>
    /// The Alert (Sensitivity / Priority) Level
    /// </summary>
    public enum AlertLevel : byte
    {
        Low,
        Medium,
        High,
        Urgent,
        Critical
    }

    /// <summary>
    /// The progress of this alert with respect to previous alert(s)
    /// </summary>
    public enum AlertProgress : short
    {
        None = 0,
        Positive = 1,
        Negative = -1
    }

    /// <summary>
    /// Rate Change Flag
    /// </summary>
    public enum Change : short
    {
        None = 0,
        Increase = 1,
        Decrease = -1,
        New = 2
    }
    /// <summary>
    /// Zonecodechanges flag
    /// </summary>
    public enum ChangeFlag : short
    {
        IsAdded = 0,
        IsDeleted = 1
    }

    /// <summary>
    /// System Events that require automated emails 
    /// </summary>
    public enum EventType : byte
    {
        PricelistImport,
        InvoiceGeneration
    }

    /// <summary>
    /// Post-Paid Client Actions (Bitwise Ored)
    /// </summary>
    [Flags]
    public enum EventActions : short
    {
        None = 0,
        Alert = 1,
        Email = 2,
        Block = 4,
        BlockAsCustomer = 8,
        SMS = 16
    }

    /// <summary>
    /// To Specify the type of the port Port_IN or Port_OUT
    /// </summary>
    public enum IO_Direction : byte
    {
        IN,
        OUT
    }

    /// <summary>
    /// Supplier Price list parser
    /// </summary>
    public enum SupplierPriceListParser : byte
    {
        System_Default,
        Ole_DB,
        XlsGen,
        //Spire,
        Tab_Delimited
    }

    /// <summary>
    /// Account manager settings action type
    /// </summary>
    public enum ActionType : byte
    {
        Approval = 1,
        Validation = 2
    }

    public enum Module : byte
    {
        AccountManagerSettings = 1,
        RatePlanSettings = 2,
        BilateralDealsSettings = 3
    }

    /// <summary>
    /// Windows Service Status
    /// </summary>
    public enum ServiceStatus : byte
    {
        Stopped = 0,
        Running = 1
    }

    /// <summary>
    /// The enumerations available for the current assembly encapsulator.
    /// It helps to save these enumerations in the database for checking the data 
    /// (So that data makes more sense and values becomes human readable).
    /// </summary>
    public class Enumerations
    {
        static log4net.ILog log = log4net.LogManager.GetLogger(typeof(Enumerations));


        protected int _ID;

        string _Enumeration, _Name;
        int _Value;

        public virtual int ID
        {
            get { return _ID; }
        }

        public virtual string Enumeration
        {
            get { return _Enumeration; }
            set { _Enumeration = value; }
        }

        public virtual string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        public virtual string DisplayName
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                bool sep = false;
                foreach (char c in _Name.Replace('_', ' ').ToCharArray())
                {
                    if (!sep && sb.Length > 0)
                    {
                        sep = char.IsUpper(c);
                        if (sep) sb.Append(' ');
                    }
                    else
                        sep = char.IsUpper(c);
                    sb.Append(c);
                }
                return sb.ToString();
            }
        }

        public virtual int Value
        {
            get { return _Value; }
            set { _Value = value; }
        }

        public static IList<Enumerations> Get(Type type)
        {
            IList<Enumerations> enumerations = new List<Enumerations>();
            if (type.IsSubclassOf(typeof(Enum)))
            {
                string underlyingType = Enum.GetUnderlyingType(type).Name.ToLower();
                foreach (object value in Enum.GetValues(type))
                {
                    Enumerations enumeration = new Enumerations();
                    enumeration.Enumeration = type.FullName;
                    enumeration.Name = Enum.GetName(type, value);
                    switch (underlyingType)
                    {
                        case "byte": enumeration.Value = (byte)value; break;
                        case "char": enumeration.Value = (char)value; break;
                        case "int16": enumeration.Value = (short)value; break;
                        case "int32": enumeration.Value = (int)value; break;
                        case "int64": enumeration.Value = (int)((long)value); break;
                    }
                    enumerations.Add(enumeration);
                }
            }
            return enumerations;
        }

        public static IList<Enumerations> Get<T>(IEnumerable<T> enumerable)
        {
            List<T> list = new List<T>(enumerable);
            IList<Enumerations> enumerations = new List<Enumerations>();
            if (list.Count > 0 && list[0].GetType().IsSubclassOf(typeof(Enum)))
            {
                Type type = typeof(T);
                string underlyingType = Enum.GetUnderlyingType(type).Name.ToLower();
                foreach (object value in enumerable)
                {
                    Enumerations enumeration = new Enumerations();
                    enumeration.Enumeration = type.FullName;
                    enumeration.Name = Enum.GetName(type, value);
                    switch (underlyingType)
                    {
                        case "byte": enumeration.Value = (byte)value; break;
                        case "char": enumeration.Value = (char)value; break;
                        case "int16": enumeration.Value = (short)value; break;
                        case "int32": enumeration.Value = (int)value; break;
                        case "int64": enumeration.Value = (int)((long)value); break;
                    }
                    enumerations.Add(enumeration);
                }
            }
            return enumerations;
        }

        /// <summary>
        /// Updates the table from the Current Assembly
        /// </summary>
        public static void BuildTableFromAssembly()
        {
            System.Data.IDbConnection connection = new System.Data.SqlClient.SqlConnection(DataConfiguration.Default.Properties["connection.connection_string"].ToString());//hibernate.connection.connection_string
            connection.Open();
            System.Data.IDbCommand command = connection.CreateCommand();
            // First empty the table 
            //System.Data.IDbCommand command = DataConfiguration.Default.SessionFactory.Settings.ConnectionProvider.GetConnection().CreateCommand();
            command.CommandText = "TRUNCATE TABLE Enumerations";
            command.ExecuteNonQuery();
            command.Connection.Close();
            command.Connection.Dispose();

            // Loop through Enumerations in this assembly and save to the database...
            Assembly thisAssembly = Assembly.GetExecutingAssembly();
            using (var session = DataConfiguration.OpenSession())
            {
                try
                {
                    var transaction = session.BeginTransaction();
                    foreach (Type type in thisAssembly.GetTypes())
                    {
                        if (type.BaseType == typeof(Enum))
                        {
                            string Enumeration = type.FullName;
                            if (Enumeration.StartsWith("TABS"))
                            {
                                IList<Enumerations> enums = Enumerations.Get(type);
                                if (enums.Count > 0)
                                    foreach (var value in enums)
                                        session.SaveOrUpdate(value);
                            }
                        }
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    log.Error("Could not Update Enumerations to Database", ex);
                }
            }
        }

        public override bool Equals(object obj)
        {
            Enumerations other = obj as Enumerations;
            if (other != null)
                return (other._Enumeration == this._Enumeration) && (other._Name == this._Name);
            else return false;
        }

        public override int GetHashCode()
        {
            return Enumeration.GetHashCode() ^ Name.GetHashCode();
        }

        public override string ToString()
        {
            return Enumeration + "." + Name;
        }
    }
}
