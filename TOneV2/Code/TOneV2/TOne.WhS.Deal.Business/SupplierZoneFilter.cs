using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Deal.Business
{
    public class SupplierZoneFilter : ISupplierZoneFilter
    {
        public int CarrierAccountId { get; set; }
        public int? DealId { get; set; }
        public DateTime BED { get; set; }
        public DateTime? EED { get; set; }
        public bool FollowSystemTimeZone { get; set; }
        public bool IsExcluded(ISupplierZoneFilterContext context)
        {
            DateTime BED = this.BED;
            DateTime? EED = this.EED;

            if (!FollowSystemTimeZone)
            {
                BED = Helper.ShiftCarrierDateTime(CarrierAccountId, true, BED).Value;
                EED = Helper.ShiftCarrierDateTime(CarrierAccountId, true, EED);
            }
            DealDefinitionManager dealDefinitionManager = new DealDefinitionManager();
            if (context.SupplierZone == null)
                throw new ArgumentNullException("SupplierZone");
            return dealDefinitionManager.IsZoneExcluded(context.SupplierZone.SupplierZoneId, BED, EED, CarrierAccountId, DealId, false);
        }
    }
}
