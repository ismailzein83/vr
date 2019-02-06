    using System;
using System.Collections.Generic;
using System.Text;


namespace TOne.WhS.Jazz.Entities
{
    public class TransactionType
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public CarrierTypeEnum CarrierType { get; set; }
        public TransactionScopeEnum TransactionScope { get; set; }
        public bool IsCredit { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime LastModifiedTime { get; set; }
        public int LastModifiedBy { get; set; }
        public int CreatedBy { get; set; }
    }
    public enum TransactionScopeEnum { Account = 0, Region=1}
    public enum CarrierTypeEnum { ApplicableForCustomers=1,ApplicableForSuppliers=2}

}