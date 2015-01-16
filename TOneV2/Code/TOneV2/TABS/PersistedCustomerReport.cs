using System;
using System.Collections.Generic;
using System.Linq;

namespace TABS
{
    /// <summary>
    /// A Persisted Custom Report (View TABS.Reports)
    /// </summary>
    public class PersistedCustomReport : Components.BaseEntity, TABS.Interfaces.ICachedCollectionContainer
    {
        internal static Dictionary<int, PersistedCustomReport> _All;

        public static Dictionary<int, PersistedCustomReport> All
        {
            get
            {
                lock (ObjectAssembler.SyncRoot)
                {
                    if (_All == null)
                    {
                        _All = ObjectAssembler.GetList<PersistedCustomReport>().ToDictionary(pcr => pcr.ID);
                    }
                }
                return _All;
            }
        }

        public static void ClearCachedCollections()
        {
            _All = null;
            TABS.Components.CacheProvider.Clear(typeof(PersistedCustomReport).FullName);
        }

        public PersistedCustomReport()
        {
            this.Created = DateTime.Now;
        }

        public int ID { get; set; }
        public SecurityEssentials.Permission RequiredPermission { get; set; }
        public string Name { get; set; }
        public string Csharp_Code { get; set; }
        public bool IsEncrypted { get; set; }
        public DateTime? Created { get; protected set; }
        public DateTime? Updated { get; set; }

        public override string Identifier
        {
            get { return this.ID == 0 ? this.Name : string.Concat("PersistedCustomReport:", ID); }
        }
    }
}
