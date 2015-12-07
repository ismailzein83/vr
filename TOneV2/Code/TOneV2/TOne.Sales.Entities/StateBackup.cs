using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Sales.Entities
{
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
    public class StateBackup
    {
        public StateBackup(StateBackupType backupType, string carrierAccount)
        {
            this.StateBackupType = backupType;
            switch (backupType)
            {
                case StateBackupType.Customer:
                    this.CustomerId = carrierAccount;
                    this.SupplierId = null;
                    break;
                case StateBackupType.Supplier:
                    this.CustomerId = null;
                    this.SupplierId = carrierAccount;
                    break;
            }
        }
        public int ID { get; set; }
        public StateBackupType StateBackupType { get; set; }
        public string CustomerId { get; set; }
        public string SupplierId { get; set; }
        public DateTime Created { get; set; }
        public DateTime? RestoreDate { get; set; }
        public string Notes { get; set; }
        public string ResponsibleForRestoring { get; set; }
        public byte[] StateData { get; set; }

    }
}
