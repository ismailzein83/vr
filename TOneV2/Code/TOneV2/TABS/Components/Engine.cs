using System;

namespace TABS.Components
{
    public partial class Engine:IDisposable
    {
        internal static log4net.ILog logMail = log4net.LogManager.GetLogger(typeof(Engine));
        public static DateTime AssemblyBuildDate
        {
            get
            {
                return WebHelperLibrary.Utility.GetBuildDate(System.Reflection.Assembly.GetExecutingAssembly());
            }
        }

        public static readonly string Version = "T.One v1.9.0 " + AssemblyBuildDate.ToString("yyyy-MM-dd");
        public static readonly DateTime Started = DateTime.Now;

        internal static bool _IsCDRStopRequested = false;
        internal static bool _IsAutoInvoiceStopRequested = false;
        internal static bool _IsCDRImportRunning = false;
         internal static bool _IsAutomaticInvoiceRunning = false;
        internal static bool _IsCDRPricingRunning = false;
        internal static bool _IsRouteOperationStopRequested = false;
        internal static bool _IsRouteOperationRunning = false;
        public static string _RouteOpertationStatus = null;

        public static string RouteOpertationStatus { get { return _RouteOpertationStatus; } }
        public static System.Threading.Thread RepricingThread { get; internal set; }
        public static bool IsRepricingRunning {

            get
            {
                if (RepricingThread != null && RepricingThread.ThreadState != System.Threading.ThreadState.Stopped)
                    return true;
                else
                    if (RepricingThread != null) Clear();
                return false;
               }
        }
        public static CdrRepricingParameters RepricingParameters { get; internal set; }

        public static readonly string RepricingLoggerName = "TABS.Repricing";
        public void Dispose()
        {
            if (RepricingThread != null)
                RepricingThread.Abort();
            RepricingThread = null;
        }
        ~Engine()
        {
            if (RepricingThread != null)
            {
                RepricingThread.Join();
                RepricingThread.Abort();
            }
            RepricingThread = null;
            CDRsImportedAndStored = null;
            CDRsImportedNotSaved = null;
            BillingCDRsGenerated = null;
            PricingComplete = null;
            logMail = null;
        }
        public static void Clear()
        {
            try
            {
                if (RepricingThread != null)
                {
                    RepricingThread.Join();
                    RepricingThread.Abort();
                }
                RepricingThread = null;
                GC.Collect(); GC.Collect();
                GC.Collect(); GC.Collect();

            }
            catch(Exception ex){}
        }
        /// <summary>
        /// Gets whether CDR operation Stop is requested.
        /// </summary>
        public static bool IsCDRStopRequested
        {
            get
            {
                return _IsCDRStopRequested;
            }
        }

        /// <summary>
        /// Gets whether Automatic Invoice operation Stop is requested.
        /// </summary>
        public static bool IsAutoInvoiceStopRequested
        {
            get
            {
                return _IsAutoInvoiceStopRequested;
            }
        }

        public static bool IsCheckForAlertsRunning { get; set; }

        /// <summary>
        /// Gets whether the CDR Import operation is actually running.
        /// </summary>
        public static bool IsCDRImportRunning
        {
            get
            {
                return _IsCDRImportRunning;
            }
        }

        /// <summary>
        /// Gets whether the CDR Import operation is actually running.
        /// </summary>
        public static bool IsAutomaticInvoiceRunning
        {
            get
            {
                return _IsAutomaticInvoiceRunning;
            }
        }

        /// <summary>
        /// Gets whether the CDR Pricing operation is actually running.
        /// </summary>
        public static bool IsCDRPricingRunning
        {
            get
            {
                return _IsCDRPricingRunning;
            }
        }

        /// <summary>
        /// Gets the Last Route Build Date/Time
        /// </summary>
        public static DateTime? LastRouteBuild { get { return SystemParameter.LastRouteBuild.DateTimeValue; } }

        /// <summary>
        /// Gets the Last Route update Date/Time
        /// </summary>
        public static DateTime? LastRouteUpdate { get { return SystemParameter.LastRouteUpdate.DateTimeValue; } }

        /// <summary>
        /// Gets the Last Route Synch Date/Time
        /// </summary>
        public static DateTime? LastRouteSynch { get { return SystemParameter.LastRouteSynch.DateTimeValue; } }

        /// <summary>
        /// Gets whether Route Build operation Stop is requested.
        /// </summary>
        public static bool IsRouteOperationStopRequested
        {
            get
            {
                return _IsRouteOperationStopRequested;
            }
        }

        /// <summary>
        /// Gets whether any route operation is actually running.
        /// </summary>
        public static bool IsRouteOperationRunning
        {
            get
            {
                return _IsRouteOperationRunning;
            }
            set
            {
                _IsRouteOperationRunning = value;
            }
        }

        public static bool DummyObject { get; set; }

        /// <summary>
        /// Request a stop for CDR Import Operations.
        /// </summary>
        public static bool StopCDRImport()
        {
            if (IsCDRImportRunning)
                _IsCDRStopRequested = true;
            return IsCDRStopRequested;
        }

        /// <summary>
        /// Request a stop for Automatic Invoice Operations.
        /// </summary>
        public static bool StopAutomaticInvoice()
        {
            if (IsAutomaticInvoiceRunning)
                _IsAutoInvoiceStopRequested = true;
            return IsAutoInvoiceStopRequested;
        }

        /// <summary>
        /// Request a stop for CDR Pricing Operations.
        /// </summary>
        public static bool StopCDRPricing()
        {
            if (IsCDRPricingRunning)
                _IsCDRStopRequested = true;
            return IsCDRStopRequested;
        }

        /// <summary>
        /// Request a stop for Route Build Operations
        /// </summary>
        public static bool StopRouteBuild()
        {
            if (IsRouteOperationRunning)
                _IsRouteOperationStopRequested = true;
            return IsRouteOperationStopRequested;
        }

        public static string BuildRouteState()
        {
            object routeBuildEnd = DataHelper.ExecuteScalar("SELECT Top 1 [Description] FROM SystemMessage  order by timestamp desc");
            if (routeBuildEnd == null || routeBuildEnd == DBNull.Value)
            {
                return (string)routeBuildEnd;
            }
            return string.Empty;
        }
    }
}