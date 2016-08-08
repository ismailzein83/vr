using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.BP.Activities
{
    public class PrepareActions : CodeActivity
    {
        #region Input Arguments

        [RequiredArgument]
        public InArgument<Changes> Changes { get; set; }

        [RequiredArgument]
        public InArgument<int> CurrencyId { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> EffectiveDate { get; set; }

        #endregion

        #region Output Arguments

        [RequiredArgument]
        public OutArgument<IEnumerable<RateToChange>> RatesToChange { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<RateToClose>> RatesToClose { get; set; }

        [RequiredArgument]
        public OutArgument<DefaultRoutingProductToAdd> DefaultRoutingProductToAdd { get; set; }

        [RequiredArgument]
        public OutArgument<DefaultRoutingProductToClose> DefaultRoutingProductToClose { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<SaleZoneRoutingProductToAdd>> SaleZoneRoutingProductsToAdd { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<SaleZoneRoutingProductToClose>> SaleZoneRoutingProductsToClose { get; set; }

        [RequiredArgument]
        public OutArgument<DateTime> MinimumDate { get; set; }

        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            Changes changes = this.Changes.Get(context);
            int currencyId = this.CurrencyId.Get(context);
            DateTime effectiveDate = this.EffectiveDate.Get(context);

            var ratesToChange = new List<RateToChange>();
            var ratesToClose = new List<RateToClose>();

            DefaultRoutingProductToAdd defaultRoutingProductToAdd = null;
            DefaultRoutingProductToClose defaultRoutingProductToClose = null;

            var saleZoneRoutingProductsToAdd = new List<SaleZoneRoutingProductToAdd>();
            var saleZoneRoutingProductsToClose = new List<SaleZoneRoutingProductToClose>();

            var minDate = effectiveDate;

            if (changes != null)
            {
                SetDefaultRoutingProductActions(ref defaultRoutingProductToAdd, ref defaultRoutingProductToClose, changes.DefaultChanges, ref minDate);
                SetZoneActions(ref ratesToChange, ref ratesToClose, ref saleZoneRoutingProductsToAdd, ref saleZoneRoutingProductsToClose, changes.ZoneChanges, currencyId, ref minDate);
            }

            RatesToChange.Set(context, ratesToChange);
            RatesToClose.Set(context, ratesToClose);

            DefaultRoutingProductToAdd.Set(context, defaultRoutingProductToAdd);
            DefaultRoutingProductToClose.Set(context, defaultRoutingProductToClose);

            SaleZoneRoutingProductsToAdd.Set(context, saleZoneRoutingProductsToAdd);
            SaleZoneRoutingProductsToClose.Set(context, saleZoneRoutingProductsToClose);

            MinimumDate.Set(context, minDate);
        }

        #region Private Methods

        private void SetDefaultRoutingProductActions(ref DefaultRoutingProductToAdd defaultRoutingProductToAdd, ref DefaultRoutingProductToClose defaultRoutingProductToClose, DefaultChanges defaultChanges, ref DateTime minDate)
        {
            if (defaultChanges == null)
                return;

            var routingProductManager = new RoutingProductManager();

            if (defaultChanges.NewDefaultRoutingProduct != null)
            {
                var newDefaultRoutingProduct = new NewDefaultRoutingProduct()
                {
                    RoutingProductId = defaultChanges.NewDefaultRoutingProduct.DefaultRoutingProductId,
                    BED = defaultChanges.NewDefaultRoutingProduct.BED,
                    EED = defaultChanges.NewDefaultRoutingProduct.EED
                };
                defaultRoutingProductToAdd = new DefaultRoutingProductToAdd()
                {
                    NewDefaultRoutingProduct = newDefaultRoutingProduct,
                    BED = defaultChanges.NewDefaultRoutingProduct.BED,
                    EED = defaultChanges.NewDefaultRoutingProduct.EED
                };
                minDate = Vanrise.Common.Utilities.Min(minDate, defaultChanges.NewDefaultRoutingProduct.BED);
            }
            else if (defaultChanges.DefaultRoutingProductChange != null)
            {
                defaultRoutingProductToClose = new DefaultRoutingProductToClose()
                {
                    CloseEffectiveDate = defaultChanges.DefaultRoutingProductChange.EED
                };
                minDate = Vanrise.Common.Utilities.Min(minDate, defaultChanges.DefaultRoutingProductChange.EED);
            }
        }

        #region Set Zone Actions

        private void SetZoneActions
        (
            ref List<RateToChange> ratesToChange,
            ref List<RateToClose> ratesToClose,
            ref List<SaleZoneRoutingProductToAdd> saleZoneRoutingProductsToAdd,
            ref List<SaleZoneRoutingProductToClose> saleZoneRoutingProductsToClose,
            IEnumerable<ZoneChanges> zoneChangesList,
            int currencyId,
            ref DateTime minDate
        )
        {
            if (zoneChangesList == null)
                return;

            var saleZoneManager = new SaleZoneManager();

            foreach (ZoneChanges zoneChanges in zoneChangesList)
            {
                SetZoneRateActions(ref ratesToChange, ref ratesToClose, zoneChanges, saleZoneManager, currencyId, ref minDate);
                SetZoneRoutingProductActions(ref saleZoneRoutingProductsToAdd, ref saleZoneRoutingProductsToClose, zoneChanges, saleZoneManager, ref minDate);
            }
        }

        private void SetZoneRateActions
        (
            ref List<RateToChange> ratesToChange,
            ref List<RateToClose> ratesToClose,
            ZoneChanges zoneChanges,
            SaleZoneManager saleZoneManager,
            int currencyId,
            ref DateTime minDate
        )
        {
            if (zoneChanges.NewRates != null)
            {
                foreach (DraftRateToChange newRate in zoneChanges.NewRates)
                {
                    ratesToChange.Add(new RateToChange()
                    {
                        ZoneId = newRate.ZoneId,
                        ZoneName = saleZoneManager.GetSaleZoneName(zoneChanges.ZoneId),
                        RateTypeId = newRate.RateTypeId,
                        NormalRate = newRate.NormalRate,
                        CurrencyId = currencyId,
                        BED = newRate.BED,
                        EED = newRate.EED
                    });
                    minDate = Vanrise.Common.Utilities.Min(minDate, newRate.BED);
                }
            }
            else if (zoneChanges.ClosedRates != null)
            {
                foreach (DraftRateToClose closedRate in zoneChanges.ClosedRates)
                {
                    ratesToClose.Add(new RateToClose()
                    {
                        ZoneName = saleZoneManager.GetSaleZoneName(zoneChanges.ZoneId),
                        RateTypeId = closedRate.RateTypeId,
                        CloseEffectiveDate = closedRate.EED
                    });
                    minDate = Vanrise.Common.Utilities.Min(minDate, closedRate.EED);
                }
            }
        }

        private void SetZoneRoutingProductActions
        (
            ref List<SaleZoneRoutingProductToAdd> saleZoneRoutingProductsToAdd,
            ref List<SaleZoneRoutingProductToClose> saleZoneRoutingProductsToClose,
            ZoneChanges zoneChanges,
            SaleZoneManager saleZoneManager,
            ref DateTime minDate
        )
        {
            if (zoneChanges.NewRoutingProduct != null)
            {
                saleZoneRoutingProductsToAdd.Add(new SaleZoneRoutingProductToAdd()
                {
                    ZoneId = zoneChanges.NewRoutingProduct.ZoneId,
                    ZoneName = saleZoneManager.GetSaleZoneName(zoneChanges.NewRoutingProduct.ZoneId),
                    ZoneRoutingProductId = zoneChanges.NewRoutingProduct.ZoneRoutingProductId,
                    BED = zoneChanges.NewRoutingProduct.BED,
                    EED = zoneChanges.NewRoutingProduct.EED
                });
                minDate = Vanrise.Common.Utilities.Min(minDate, zoneChanges.NewRoutingProduct.BED);
            }
            else if (zoneChanges.RoutingProductChange != null)
            {
                saleZoneRoutingProductsToClose.Add(new SaleZoneRoutingProductToClose()
                {
                    ZoneId = zoneChanges.RoutingProductChange.ZoneId,
                    ZoneName = saleZoneManager.GetSaleZoneName(zoneChanges.RoutingProductChange.ZoneId),
                    CloseEffectiveDate = zoneChanges.RoutingProductChange.EED
                });
                minDate = Vanrise.Common.Utilities.Min(minDate, zoneChanges.RoutingProductChange.EED);
            }
        }

        #endregion

        #endregion
    }
}
