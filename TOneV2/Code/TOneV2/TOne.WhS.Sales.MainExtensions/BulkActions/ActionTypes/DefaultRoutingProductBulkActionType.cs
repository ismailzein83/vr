using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.MainExtensions
{
    public class DefaultRoutingProductBulkActionType : BulkActionType
    {
        #region Fields

        private RoutingProductManager _routingProductManager = new RoutingProductManager();
        private string _defaultRoutingProductName;

        #endregion

        public int? DefaultRoutingProductId { get; set; }

        #region Bulk Action Members

        public override Guid ConfigId
        {
            get { return new Guid("AA91095E-07D2-4CA5-9269-FFDAFC93B9D1"); }
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
            if (context.SaleZone.EED.HasValue)
                return false;
            DateTime? countryEED = context.GetCountryEED(context.SaleZone.CountryId);
            if (countryEED.HasValue)
                return false;
            return true;
        }

        public override void ApplyBulkActionToZoneItem(IApplyBulkActionToZoneItemContext context)
        {
            if (context.ZoneDraft != null && context.ZoneDraft.NewRoutingProduct != null)
                return;
            ZoneItem contextZoneItem = context.GetContextZoneItem(context.ZoneItem.ZoneId);
            if (contextZoneItem.CurrentRoutingProductId.HasValue && contextZoneItem.IsCurrentRoutingProductEditable.Value && (context.ZoneDraft == null || context.ZoneDraft.RoutingProductChange == null))
                return;

            if (DefaultRoutingProductId.HasValue)
            {
                if (_defaultRoutingProductName == null)
                    _defaultRoutingProductName = _routingProductManager.GetRoutingProductName(DefaultRoutingProductId.Value);

                context.ZoneItem.EffectiveRoutingProductId = DefaultRoutingProductId;
                context.ZoneItem.EffectiveRoutingProductName = _defaultRoutingProductName;
                context.ZoneItem.EffectiveServiceIds = _routingProductManager.GetZoneServiceIds(DefaultRoutingProductId.Value, context.ZoneItem.ZoneId);
            }
            else
            {
                SaleEntityZoneRoutingProduct sellingProductZoneRP = context.GetSellingProductZoneRoutingProduct(context.ZoneItem.ZoneId);

                if (sellingProductZoneRP != null)
                {
                    context.ZoneItem.EffectiveRoutingProductId = sellingProductZoneRP.RoutingProductId;
                    context.ZoneItem.EffectiveRoutingProductName = _routingProductManager.GetRoutingProductName(sellingProductZoneRP.RoutingProductId);
                    context.ZoneItem.EffectiveServiceIds = _routingProductManager.GetZoneServiceIds(sellingProductZoneRP.RoutingProductId, context.ZoneItem.ZoneId);
                }
            }
        }

        public override void ApplyBulkActionToZoneDraft(IApplyBulkActionToZoneDraftContext context)
        {

        }

        public override void ApplyBulkActionToDefaultDraft(IApplyBulkActionToDefaultDraftContext context)
        {
            if (DefaultRoutingProductId.HasValue)
            {
                context.DefaultDraft.NewDefaultRoutingProduct = new DraftNewDefaultRoutingProduct()
                {
                    DefaultRoutingProductId = DefaultRoutingProductId.Value,
                    BED = DateTime.Today,
                    EED = null
                };
            }
            else
            {
                SaleEntityZoneRoutingProduct customerDefaultRP = context.GetCustomerDefaultRoutingProduct();

                if (customerDefaultRP == null)
                    throw new Vanrise.Entities.DataIntegrityValidationException("Default routing product was not found");

                if (customerDefaultRP.Source != SaleEntityZoneRoutingProductSource.CustomerDefault)
                    throw new Vanrise.Entities.DataIntegrityValidationException("Reset is an invalid action");

                context.DefaultDraft.DefaultRoutingProductChange = new DraftChangedDefaultRoutingProduct()
                {
                    DefaultRoutingProductId = customerDefaultRP.RoutingProductId,
                    EED = DateTime.Today
                };
            }
        }

        #endregion
    }
}
