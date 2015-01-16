using System;
using System.Text;

namespace TABS.SpecialSystemParameters
{
    public class InvoiceSerialGenerator
    {
        static log4net.ILog log = log4net.LogManager.GetLogger(typeof(InvoiceSerialGenerator));

        protected static int GetCount(string sql, params object[] parameters)
        {
            try
            {
                object value = DataHelper.ExecuteScalar(sql, parameters);
                return int.Parse(value.ToString()) + 1;
            }
            catch (Exception ex)
            {
                log.Error("Error while generating serial invoice", ex);
                return 1;
            }
        }

        public static readonly string SerialHelpString = @"
            <u>Dates</u>: {0} = Invoice Issue Date, {1} = Invoice Begin Date, {2} = Invoice Till Date
            <br/>
            <u>Identifiers</u>: {3} = Carrier Account ID, {4} = Profile ID
            <br/> 
            <u>Overall Counters</u>: {5} = Account Invoices Count, {6} = Profile Incvoices Count, {7} = All Invoices Count (+ Startup Counter Value)
            <br/>
            <u>Yearly Counters</u>: {8} = Account's Yearly Invoice Count, {9} = Profile Yearly Incvoice Count, {10} = Yearly Invoices Count (+ Yearly Startup Counter Value)
            <br />
            <u>Examples</u>: 
                <ul>
                <li><u>T.One-{1:yyyy}-{10:0000}</u> Becomes <u>T.One-2008-0004</u> (2008 is the year, 0004 is the number of invoices generated for all customers in 2008)
                <li><u>T.One-{3}-{1:yyyy}-{8:0000}</u> Becomes <u>T.One-C019-2008-0002</u> (2008 is the year, C019 is the Carrier Account ID, 2 is the number of invoices generated for carrier account C019 in 2008)
                </ul>
            Date can be formatted in standard .NET formats for date, Invoice counts are formatted according to integer number format rules.";


        public static string GenerateSerial_New(Billing_Invoice invoice)
        {
            // var allInvoices = TABS.ObjectAssembler.GetList<TABS.Billing_Invoice>();

            if (invoice.Customer.CustomerMask != TABS.CarrierAccount.SystemAccountID)
                return GenerateMaskSerial(invoice);

            TABS.CarrierAccount account = invoice.Customer;

            var IsProfileInvoice = account.CarrierProfile.IsProfileInvoice;

            bool isMafiaAccount = account.InvoiceSerialPattern == null ? false : !string.IsNullOrEmpty(account.InvoiceSerialPattern.Trim());

            string Serialformat = !isMafiaAccount ? TABS.SystemParameter.InvoiceSerialNumberFormat.TextValue : account.InvoiceSerialPattern;

            string extraCondition = isMafiaAccount ?
                " AND I.CustomerID IN (SELECT C.CarrierAccountID FROM CarrierAccount C WITH(NOLOCK) WHERE C.InvoiceSerialPattern <> '' AND C.InvoiceSerialPattern IS NOT NULL) "
                : string.Empty; //" AND I.CustomerID IN (SELECT C.CarrierAccountID FROM CarrierAccount C WHERE C.InvoiceSerialPattern IS NULL OR C.InvoiceSerialPattern = '' )";

            StringBuilder sb = new StringBuilder();

            int year = invoice.BeginDate.Year;
            //DateTime FromDateLimit = new DateTime(year, 1, 1);
            //DateTime ToDateLimit = new DateTime(year, 12, 31);

            string yearLimit = " AND I.BeginDate BETWEEN '" + year + "-01-01' AND '" + year + "-12-31'";

            int accountCount = GetCount("WITH Temp AS ( SELECT Count(*) cnt FROM Billing_Invoice I WITH(NOLOCK) WHERE I.CustomerID = @P1" + extraCondition + " Group by I.SerialNumber) select count(*) from Temp", account.CarrierAccountID);
            int profileCount = GetCount("WITH Temp AS ( SELECT Count(*) cnt FROM Billing_Invoice I WITH(NOLOCK) WHERE I.CustomerID IN (SELECT A.CarrierAccountID FROM CarrierAccount A WITH(NOLOCK) WHERE A.ProfileID = @P1) " + extraCondition + " Group by I.SerialNumber) select count(*) from Temp", account.CarrierProfile.ProfileID);
            int allCount = GetCount("WITH Temp AS ( SELECT Count(*) cnt FROM Billing_Invoice I  WITH(NOLOCK) WHERE I.CustomerID <> 'SYS' and I.CustomerMask = 'SYS'" + extraCondition + " Group by I.SerialNumber) select count(*) from Temp")
                +
                (int)SystemParameter.Invoices_Overall_Startup_Counter.NumericValue
                ;

            //            int Allcount = GetCount(@"WITH Temp AS (
            //                 SELECT  cast(Cp.ProfileID AS VARCHAR(5)) + ISNULL(cp.InvoiceByProfile,'N') AS [key] , COUNT(*) cnt
            //                 FROM   Billing_Invoice I WITH(NOLOCK)
            //                 LEFT JOIN CarrierAccount ca WITH(NOLOCK) ON ca.CarrierAccountID = I.CustomerID
            //                 LEFT JOIN CarrierProfile cp WITH(NOLOCK) ON cp.ProfileID = ca.ProfileID
            //                 WHERE  I.CustomerID <> 'SYS'
            //                 GROUP BY
            //                        cast(Cp.ProfileID AS VARCHAR(5)) + ISNULL(cp.InvoiceByProfile,'N')
            //             ) 
            //SELECT count(*)
            //FROM   Temp");

            int yearAccountCount = GetCount("WITH Temp AS ( SELECT Count(*) cnt FROM Billing_Invoice I WITH(NOLOCK) WHERE I.CustomerID = @P1" + yearLimit + extraCondition + " Group by I.SerialNumber) select count(*) from Temp", account.CarrierAccountID);
            int yearProfileCount = GetCount("WITH Temp AS ( SELECT Count(*) cnt FROM Billing_Invoice I WITH(NOLOCK) WHERE I.CustomerID IN (SELECT A.CarrierAccountID FROM CarrierAccount A WITH(NOLOCK) WHERE A.ProfileID = @P1)" + yearLimit + extraCondition + " Group by I.SerialNumber) select count(*) from Temp", account.CarrierProfile.ProfileID);
            int yearAllCount = GetCount("WITH Temp AS ( SELECT Count(*) cnt FROM Billing_Invoice I WITH(NOLOCK) WHERE I.CustomerID <> 'SYS' and I.CustomerMask = 'SYS'" + extraCondition + yearLimit + " Group by I.SerialNumber) select count(*) from Temp")
                +
                (int)SystemParameter.Invoices_Yearly_Startup_Counter.NumericValue
                ;

            sb.AppendFormat(Serialformat,

                    // Dates
                    invoice.IssueDate,
                    invoice.BeginDate,
                    invoice.EndDate,

                    // Carrier Account ID and Profile ID
                    account.CarrierAccountID,
                    account.CarrierProfile.ProfileID,

                    // All counts
                    accountCount,
                    profileCount,
                    allCount,//allCount,

                    // Yearly Counts
                    yearAccountCount,
                    yearProfileCount,
                    yearAllCount
                    );

            return sb.ToString();
        }


        public static string GenerateSerial(Billing_Invoice invoice)
        {
            if (invoice.Customer.CustomerMask != TABS.CarrierAccount.SystemAccountID)
                return GenerateMaskSerial(invoice);

            TABS.CarrierAccount account = invoice.Customer;

            bool isMafiaAccount = account.InvoiceSerialPattern == null ? false : !string.IsNullOrEmpty(account.InvoiceSerialPattern.Trim());

            string Serialformat = !isMafiaAccount ? TABS.SystemParameter.InvoiceSerialNumberFormat.TextValue : account.InvoiceSerialPattern;

            string extraCondition = isMafiaAccount ?
                " AND I.CustomerID IN (SELECT C.CarrierAccountID FROM CarrierAccount C WITH(NOLOCK) WHERE C.InvoiceSerialPattern <> '' AND C.InvoiceSerialPattern IS NOT NULL) "
                : string.Empty; //" AND I.CustomerID IN (SELECT C.CarrierAccountID FROM CarrierAccount C WHERE C.InvoiceSerialPattern IS NULL OR C.InvoiceSerialPattern = '' )";

            StringBuilder sb = new StringBuilder();

            int year = invoice.BeginDate.Year;
            string yearLimit = " AND I.BeginDate BETWEEN '" + year + "-01-01' AND '" + year + "-12-31'";

            int accountCount = GetCount("WITH Temp AS ( SELECT Count(*) cnt FROM Billing_Invoice I WITH(NOLOCK) WHERE I.CustomerID = @P1" + extraCondition, account.CarrierAccountID);
            int profileCount = GetCount("WITH Temp AS ( SELECT Count(*) cnt FROM Billing_Invoice I WITH(NOLOCK) WHERE I.CustomerID IN (SELECT A.CarrierAccountID FROM CarrierAccount A WITH(NOLOCK) WHERE A.ProfileID = @P1)" + extraCondition + " Group by I.SerialNumber) select count(*) from Temp", account.CarrierProfile.ProfileID);
            int allCount = GetCount("WITH Temp AS ( SELECT Count(*) cnt FROM Billing_Invoice I  WITH(NOLOCK) WHERE I.CustomerID <> 'SYS'" + extraCondition + " Group by I.SerialNumber) select count(*) from Temp")
                +
                (int)SystemParameter.Invoices_Overall_Startup_Counter.NumericValue
                ;

            int yearAccountCount = GetCount("WITH Temp AS ( SELECT Count(*) cnt FROM Billing_Invoice I WITH(NOLOCK) WHERE I.CustomerID = @P1" + yearLimit + extraCondition + " Group by I.SerialNumber) select count(*) from Temp", account.CarrierAccountID);
            int yearProfileCount = GetCount("WITH Temp AS ( SELECT Count(*) cnt FROM Billing_Invoice I WITH(NOLOCK) WHERE I.CustomerID IN (SELECT A.CarrierAccountID FROM CarrierAccount A WITH(NOLOCK) WHERE A.ProfileID = @P1)" + yearLimit + extraCondition + " Group by I.SerialNumber) select count(*) from Temp", account.CarrierProfile.ProfileID);
            int yearAllCount = GetCount("WITH Temp AS ( SELECT Count(*) cnt FROM Billing_Invoice I WITH(NOLOCK) WHERE I.CustomerID <> 'SYS'" + extraCondition + yearLimit + " Group by I.SerialNumber) select count(*) from Temp");

            sb.AppendFormat(Serialformat,

                    // Dates
                    invoice.IssueDate,
                    invoice.BeginDate,
                    invoice.EndDate,

                    // Carrier Account ID and Profile ID
                    account.CarrierAccountID,
                    account.CarrierProfile.ProfileID,

                    // All counts
                    accountCount,
                    profileCount,
                    allCount,

                    // Yearly Counts
                    yearAccountCount,
                    yearProfileCount,
                    yearAllCount
                    );

            return sb.ToString();
        }
        public static readonly string MaskSerialhelpString = @"
            <u>Dates</u>: {0} = Invoice Issue Date, {1} = Invoice Begin Date, {2} = Invoice Till Date
            <br/>
            <u>Overall Counters</u>: {3} = Account Invoices Count, {4} = All Invoices Count (+ Startup Counter Value)
            <br/>
            <u>Yearly Counters</u>: {5} = Account's Yearly Invoice Count, {6} = Yearly Invoices Count  (+ Yearly Startup Counter Value)
            ";


        public static string GenerateMaskSerial(Billing_Invoice invoice)
        {
            TABS.CarrierAccount account = invoice.Customer;
            int year = invoice.BeginDate.Year;
            string yearLimit = " AND bi.BeginDate BETWEEN '" + year + "-01-01' AND '" + year + "-12-31'";

            int accountCount = GetCount("WITH Temp AS ( SELECT Count(*) cnt FROM Billing_Invoice bi WITH(NOLOCK) WHERE bi.CustomerMask = @P1 AND bi.CustomerID = @P2 Group by bi.SerialNumber) select count(*) from Temp", account.CustomerMask, account.CarrierAccountID);
            int yearAccountCount = GetCount("WITH Temp AS ( SELECT Count(*) cnt FROM Billing_Invoice bi WITH(NOLOCK) WHERE bi.CustomerMask = @P1" + yearLimit + " AND bi.CustomerID = @P2 Group by bi.SerialNumber) select count(*) from Temp", account.CustomerMask, account.CarrierAccountID);

            int allCount = GetCount("WITH Temp AS ( SELECT Count(*) cnt FROM Billing_Invoice bi WITH(NOLOCK) WHERE bi.CustomerMask = @P1 Group by bi.SerialNumber) select count(*) from Temp", account.CustomerMask)
              +
                //(int)SystemParameter.Mask_Invoices_Overall_Startup_Counter.NumericValue
              account.CustomerMaskAccount.MaskOverAllCounter
              ;
            int yearAllCount = GetCount("WITH Temp AS ( SELECT Count(*) cnt FROM Billing_Invoice bi WITH(NOLOCK) WHERE bi.CustomerMask = @P1 " + yearLimit + " Group by bi.SerialNumber) select count(*) from Temp", account.CustomerMask)
            +
              account.CustomerMaskAccount.YearlyMaskOverAllCounter
              ;
            StringBuilder sb = new StringBuilder();
            //string Serialformat = TABS.SystemParameter.MaskInvoiceSerialNumberFormat.TextValue;
            string Serialformat = account.CustomerMaskAccount.MaskInvoiceformat;

            sb.AppendFormat(Serialformat,
                // Dates
                                invoice.IssueDate,
                                invoice.BeginDate,
                                invoice.EndDate,

                                // All counts
                                accountCount,
                                allCount,

                                // Yearly Counts
                                yearAccountCount,
                                yearAllCount
                                );

            return sb.ToString();
        }
    }
}
