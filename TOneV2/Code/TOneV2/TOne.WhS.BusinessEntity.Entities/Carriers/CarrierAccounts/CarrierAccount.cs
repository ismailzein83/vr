using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities.EntitySynchronization;

namespace TOne.WhS.BusinessEntity.Entities
{
    public enum CarrierAccountType { Exchange = 1, Supplier = 2, Customer = 3 }

    public class BaseCarrierAccount
    {
        public int CarrierAccountId { get; set; }
        public string NameSuffix { get; set; }
        public int? SellingProductId { get; set; }
        public CarrierAccountSettings CarrierAccountSettings { get; set; }
        public CarrierAccountSupplierSettings SupplierSettings { get; set; }
        public CarrierAccountCustomerSettings CustomerSettings { get; set; }
        public string SourceId { get; set; }
        public DateTime CreatedTime { get; set; }
        public int CreatedBy { get; set; }
        public int LastModifiedBy { get; set; }
        public DateTime LastModifiedTime { get; set; }
    }

    public class CarrierAccount : BaseCarrierAccount
    {
        public int CarrierProfileId { get; set; }
        public CarrierAccountType AccountType { get; set; }
        public int? SellingNumberPlanId { get; set; }
        public Dictionary<string, Object> ExtendedSettings { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class CarrierAccountToEdit : BaseCarrierAccount
    {

    }
}
