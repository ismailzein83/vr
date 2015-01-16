using System;
using System.Collections.Generic;

namespace TABS.Addons.CDRStores
{
    [NamedAddon("Default CDR Store", "Stores cdrs in the system Database")]
    public class DefaultCDRStore : Extensibility.ICDRStore
    {
        static log4net.ILog log = log4net.LogManager.GetLogger(typeof(DefaultCDRStore));

        #region ICDRStore Members

        public string Name { get { return typeof(DefaultCDRStore).FullName; } }
        public string Description { get { return "Stores cdrs in the system Database"; } set { /* does nothing */ } }
        public string HelpHtml { get { return "The Default CDR Store is not configurable"; } }
        public string ConfigString { get { return "<not configurable>"; } set { /* does nothing */ } }
        public string ConfigOptions { get { return "<not configurable>"; } set { /* does nothing */ } }
        public bool IsEnabled { get; set; }

        public void Put(NHibernate.ISession session, IEnumerable<CDR> cdrs)
        {
            foreach (var cdr in cdrs) 
                cdr.ClipFieldsToBounds();

            using (Components.BulkManager bulkManager = new TABS.Components.BulkManager(log))
            {
                bulkManager.Write(cdrs);
            }            
        }

        public IEnumerable<CDR> Get(NHibernate.ISession session, DateTime from, DateTime till)
        {
            return session.CreateQuery("FROM CDR where AttemptDateTime BETWEEN :from AND :till")
                .SetParameter("from", from)
                .SetParameter("till", till)
                .List<CDR>();
        }

        #endregion
    }
}
