using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TABS.Addons.Runnables
{
    [NamedAddon("T.One - Mail CDR Problems", "Mails CDR problems to registered users.")]
    class CDRProblemsRunner : RunnableBase
    {
        static log4net.ILog log = log4net.LogManager.GetLogger("TABS.Addons.Runnables.CDRProblemsRunner");
        public override void Run()
        {
            SendEmailToUsers(GetMessageBody(true), true);
            SendEmailToUsers(GetMessageBody(false), false);
        }

        private static string GetMessageBody(bool grouped)
        {
            DateTime from = DateTime.Today.AddDays(-1);
            DateTime until = DateTime.Today.AddMilliseconds(-3);
            string missMappedTable, missMappedCDRTable, missMappedOurZoneTable, missMappedSupplierTable,
                missMappedCDPNTable, missMappedSaleTable, missMappedCostTable;
            List<ITableRow>
                rows1 = MissMappedHelper.Format_MismappedData
                    (MissMappedHelper.GetMismappedDataSource(from, until, 50, 0)),
                rows2 = MissMappedHelper.Format_MismappedCdrData
                    (MissMappedHelper.GetMismappedCdrDataSource(from, until, 50, 0), grouped),
                rows3 = MissMappedHelper.Format_MismappedOurZoneData
                    (MissMappedHelper.GetMismappedOurZoneDataSource(from, until, 50, 0, null, null), grouped),
                rows4 = MissMappedHelper.Format_MismappedSupplierZoneData
                    (MissMappedHelper.GetMismappedSupplierZoneDataSource(from, until, 50, 0, null, null), grouped),
                rows5 = MissMappedHelper.Format_MismappedCDPNData
                    (MissMappedHelper.GetMismappedCDPNDataSource(from, until, 50, 0), grouped),
                rows6 = MissMappedHelper.Format_MismappedSaleData
                    (MissMappedHelper.GetMismappedSaleDataSource(from, until, 50,
                        TABS.SystemConfiguration.KnownParameters[TABS.KnownSystemParameter.sys_CDR_Pricing_CDRID].Value, null, null), grouped),
                rows7 = MissMappedHelper.Format_MismappedCostData
                    (MissMappedHelper.GetMismappedCostDataSource(from, until, 50,
                        TABS.SystemConfiguration.KnownParameters[TABS.KnownSystemParameter.sys_CDR_Pricing_CDRID].Value, null, null), grouped);
            if (rows1.Count + rows2.Count + rows3.Count + rows4.Count + rows5.Count + rows6.Count + rows7.Count > 0)
            {
                missMappedTable =
                    MissMappedHelper.DisplayAsTable(rows1,
                    "<tr><th>Carrier</th><th>Type</th></tr>",
                    "Missing Carrier Mapping");

                missMappedCDRTable = MissMappedHelper.DisplayAsTable(rows2,
                        "<tr><th>Day/Attempt</th><th>Carrier In</th><th>Carrier Out</th><th>CDPN</th><th>Country</th></tr>",
                        "Missing Carrier Mapping Info");

                missMappedOurZoneTable =
                    MissMappedHelper.DisplayAsTable(rows3,
                        "<tr><th>Day/Attempt</th><th>Customer</th><th>Supplier</th><th>Sale Zone</th><th>Cost Zone</th></tr>",
                        "Missing Sale Code or Zone Info"
                    );

                missMappedSupplierTable =
                    MissMappedHelper.DisplayAsTable(rows4,
                        "<tr><th>Day/Attempt</th><th>Customer</th><th>Supplier</th><th>Sale Zone</th><th>Cost Zone</th></tr>",
                        "Missing Cost Code or Zone Info"
                    );

                missMappedCDPNTable =
                    MissMappedHelper.DisplayAsTable(rows5,
                        "<tr><th>Day/Attempt</th><th>Carrier In</th><th>Carrier Out</th><th>CDPN</th><th>Country</th></tr>",
                        "Missing CDPN Info"
                    );

                missMappedSaleTable =
                    MissMappedHelper.DisplayAsTable(rows6,
                        "<tr><th>Day/Attempt</th><th>Customer</th><th>Supplier</th><th>Sale Zone</th><th>Cost Zone</th></tr>",
                        "Missing Sale Rate Info"
                    );

                missMappedCostTable =
                    MissMappedHelper.DisplayAsTable(rows7,
                     "<tr><th>Day/Attempt</th><th>Customer</th><th>Supplier</th><th>Sale Zone</th><th>Cost Zone</th></tr>",
                     "Missing Cost Rate (Purchase) Info"
                    );
                return string.Format("{0}<br />{1}<br />{2}<br />{3}<br />{4}<br />{5}<br />{6}",
                    missMappedTable, missMappedCDRTable, missMappedOurZoneTable, missMappedSupplierTable,
                    missMappedCDPNTable, missMappedSaleTable, missMappedCostTable);
            }
            return "<span style='color: green'><b>No error message</b></span>";
            
        }
        public override string Status
        {
            get { return string.Empty; }
        }
        private string GetAllExpandedRecepients()
        {
            //Get the list of users that will be sent the email
            //note that MailCdrProblemsResults is used through an enumeration elsewhere
            IList<SecurityEssentials.User> users;
            using (NHibernate.ISession session = TABS.DataConfiguration.OpenSession())
            {
                users =
                    session
                    .CreateQuery(@"select u from User u 
                                    join u.UserOptions uo1
                                    join u.UserOptions uo2 
                                   Where 
                                    uo1._Option = 'MailCdrProblemsResults' and uo1.Value = 'true'
                                    and
                                    uo2._Option = 'SendCdrProblemsExpanded' and uo2.Value = 'true'
                                ")
                    .List<SecurityEssentials.User>();
                session.Clear();
            }
            return users.Count > 0 ? users.Select(u => u.Login).Aggregate((agg, l) => agg + ", " + l) : string.Empty;
        }
        private string GetAllGroupedRecepients()
        {
            //Get the list of users that will be sent the email
            //note that MailCdrProblemsResults is used through an enumeration elsewhere
            IList<SecurityEssentials.User> users;
            using (NHibernate.ISession session = TABS.DataConfiguration.OpenSession())
            {
                users =
                    session
                    .CreateSQLQuery(@"select {u.*} from [User] u
inner join UserOption uo1 on (u.ID = uo1.[User] and uo1.[Option] = 'MailCdrProblemsResults' and uo1.Value = 'true')
left join UserOption uo2 on (u.ID = uo2.[User] and uo2.[Option] = 'SendCdrProblemsExpanded')
Where uo2.ID is null or uo2.Value = 'false'")
                    .AddEntity("u", typeof(SecurityEssentials.User))
                    //.AddJoin("u.UserOptions",)
                    .List<SecurityEssentials.User>();
                session.Clear();
            }
            return users.Count > 0 ? users.Select(u => u.Login).Aggregate((agg, l) => agg + ", " + l) : string.Empty;
        }
        private void SendEmailToUsers(string body, bool grouped)
        {
            string mailRecepients = grouped ? GetAllGroupedRecepients() : GetAllExpandedRecepients();
            if (mailRecepients == string.Empty) return;
            try
            {
                System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
                message.To.Add(mailRecepients);
                message.Subject = "T.One - CDR Problems";
                message.IsBodyHtml = true;
                StringBuilder sb = new StringBuilder();
                sb.Append("<html>");
                sb.Append("<head>");
                sb.Append("</head>");
                sb.Append("<body>");
                sb.Append(body);
                sb.Append("</body>");
                sb.Append("</html>");
                message.Body = sb.ToString();

                Exception ex;
                TABS.Components.EmailSender.Send(message, out ex);
                if (ex != null)
                {
                    log.Error(string.Format("Error Sending CDR Problems Email to one of the following {0}.", mailRecepients), ex);
                }
            }
            catch (Exception ex)
            {
                log.Error(string.Format("Error Creating Mail Message for {0} ", mailRecepients), ex);
            }
        }
    }
}
