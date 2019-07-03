using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.MainExtensions
{
    public class ImportSupplierTargetMatch : BulkActionType
    {
        #region Bulk Action Members
        public override Guid ConfigId
        {
            get { return new Guid("E6D4013F-F618-4F42-8308-4D0C0A282B47"); }
        }
        public override void ValidateZone(IZoneValidationContext context)
        {

        }
        public override bool IsApplicableToCountry(IBulkActionApplicableToCountryContext context)
        {
            return true;
        }
        public override bool IsApplicableToZone(IActionApplicableToZoneContext context)
        {
            return true;
        }
        public override void ApplyBulkActionToZoneItem(IApplyBulkActionToZoneItemContext context)
        {
          
        }
        public override void ApplyBulkActionToZoneDraft(IApplyBulkActionToZoneDraftContext context)
        {
          
        }
        #endregion
    }
}
