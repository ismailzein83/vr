using System;
using System.Collections.Generic;

namespace TABS
{
    public class StateBackupInfo : Components.BaseEntity
    {
        public virtual int ID { get; set; }
        public virtual StateBackupType StateBackupType { get; set; }
        public virtual CarrierAccount Customer { get; set; }
        public virtual CarrierAccount Supplier { get; set; }
        public virtual DateTime Created { get; set; }
        public virtual DateTime? RestoreDate { get; set; }
        public virtual string Notes { get; set; }
        public virtual string ResponsibleForRestoring { get; set; }

        public virtual string CustomerName { get { return Customer == null ? null : Customer.Name; } }
        public virtual string SupplierName { get { return Supplier == null ? null : Supplier.Name; } }
        public virtual string UserName { get { return User == null ? null : User.Name; } }


        protected StateBackupInfo() { }

        protected class DateComparer : IComparer<StateBackupInfo>
        {
            public int Compare(StateBackupInfo x, StateBackupInfo y)
            {
                DateTime xDate = x.RestoreDate.HasValue && x.RestoreDate.Value > x.Created ? x.RestoreDate.Value : x.Created;
                DateTime yDate = y.RestoreDate.HasValue && y.RestoreDate.Value > y.Created ? y.RestoreDate.Value : y.Created;
                return yDate.CompareTo(xDate);
            }
        }

        public static IList<StateBackupInfo> List()
        {
            var allStateBackups = TABS.ObjectAssembler.CurrentSession.CreateQuery("FROM StateBackupInfo").List<StateBackupInfo>();
            List<StateBackupInfo> list = new List<StateBackupInfo>(allStateBackups);
            list.Sort(new DateComparer());
            return list;
        }

        public override string ToString()
        {
            return string.Format("State Backup, Type: {0}{1}, Created: {2:yyyy-MM-dd HH:mm:ss}"
                , this.StateBackupType
                , this.StateBackupType == StateBackupType.Full ? "" : string.Format(", {0}{1}", this.Customer, this.Supplier)
                , this.Created
                );
        }

        // used to identify for who this state backup
        public string StateBackupCarriers
        {
            get
            {
                string r = string.Empty;
                if (this.Customer != null) r += this.Customer.CarrierAccountID;
                if (this.Supplier != null) r += this.Supplier.CarrierAccountID;
                return r;
            }
        }

        public override string Identifier
        {
            get { return "StateBackupInfo:" + (ID == 0 ? Created.ToString("yyyyMMddHHmmss") : ID.ToString()); }
        }
    }
}
