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
        public OutArgument<DefaultServiceToAdd> DefaultServiceToAdd { get; set; }

        [RequiredArgument]
        public OutArgument<DefaultServiceToClose> DefaultServiceToClose { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<SaleZoneRoutingProductToAdd>> SaleZoneRoutingProductsToAdd { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<SaleZoneRoutingProductToClose>> SaleZoneRoutingProductsToClose { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<SaleZoneServiceToAdd>> SaleZoneServicesToAdd { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<SaleZoneServiceToClose>> SaleZoneServicesToClose { get; set; }

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
            DefaultServiceToAdd defaultServiceToAdd = null;
            DefaultServiceToClose defaultServiceToClose = null;

            var saleZoneRoutingProductsToAdd = new List<SaleZoneRoutingProductToAdd>();
            var saleZoneRoutingProductsToClose = new List<SaleZoneRoutingProductToClose>();
            var saleZoneServicesToAdd = new List<SaleZoneServiceToAdd>();
            var saleZoneServicesToClose = new List<SaleZoneServiceToClose>();

            var minDate = effectiveDate;

            if (changes != null)
            {
                SetDefaultActions(out defaultRoutingProductToAdd, out defaultRoutingProductToClose, out defaultServiceToAdd, out defaultServiceToClose, changes.DefaultChanges, ref minDate);
                SetZoneActions(ref ratesToChange, ref ratesToClose, ref saleZoneRoutingProductsToAdd, ref saleZoneRoutingProductsToClose, ref saleZoneServicesToAdd, ref saleZoneServicesToClose, changes.ZoneChanges, currencyId, ref minDate);
            }

            RatesToChange.Set(context, ratesToChange);
            RatesToClose.Set(context, ratesToClose);

            DefaultRoutingProductToAdd.Set(context, defaultRoutingProductToAdd);
            DefaultRoutingProductToClose.Set(context, defaultRoutingProductToClose);

            DefaultServiceToAdd.Set(context, defaultServiceToAdd);
            DefaultServiceToClose.Set(context, defaultServiceToClose);

            SaleZoneRoutingProductsToAdd.Set(context, saleZoneRoutingProductsToAdd);
            SaleZoneRoutingProductsToClose.Set(context, saleZoneRoutingProductsToClose);

            SaleZoneServicesToAdd.Set(context, saleZoneServicesToAdd);
            SaleZoneServicesToClose.Set(context, saleZoneServicesToClose);

            MinimumDate.Set(context, minDate);
        }

        #region Private Methods

        #region Set Default Actions

        private void SetDefaultActions(out DefaultRoutingProductToAdd defaultRoutingProductToAdd, out DefaultRoutingProductToClose defaultRoutingProductToClose, out DefaultServiceToAdd defaultServiceToAdd, out DefaultServiceToClose defaultServiceToClose, DefaultChanges defaultDraft, ref DateTime minDate)
        {
            if (defaultDraft != null)
            {
                SetDefaultRoutingProductActions(defaultDraft.NewDefaultRoutingProduct, defaultDraft.DefaultRoutingProductChange, out defaultRoutingProductToAdd, out defaultRoutingProductToClose, ref minDate);

                SetDefaultServiceActions(defaultDraft.NewService, defaultDraft.ClosedService, defaultDraft.ResetService, out defaultServiceToAdd, out defaultServiceToClose, ref minDate);
            }
            else
            {
                defaultRoutingProductToAdd = null;
                defaultRoutingProductToClose = null;
                defaultServiceToAdd = null;
                defaultServiceToClose = null;
            }
        }

        private void SetDefaultRoutingProductActions(DraftNewDefaultRoutingProduct newDefaultRoutingProduct, DraftChangedDefaultRoutingProduct changedDefaultRoutingProduct, out DefaultRoutingProductToAdd defaultRoutingProductToAdd, out DefaultRoutingProductToClose defaultRoutingProductToClose, ref DateTime minDate)
        {
            defaultRoutingProductToAdd = null;
            defaultRoutingProductToClose = null;

            if (newDefaultRoutingProduct != null)
            {
                var newRoutingProduct = new NewDefaultRoutingProduct()
                {
                    RoutingProductId = newDefaultRoutingProduct.DefaultRoutingProductId,
                    BED = newDefaultRoutingProduct.BED,
                    EED = newDefaultRoutingProduct.EED
                };
                defaultRoutingProductToAdd = new DefaultRoutingProductToAdd()
                {
                    NewDefaultRoutingProduct = newRoutingProduct,
                    BED = newDefaultRoutingProduct.BED,
                    EED = newDefaultRoutingProduct.EED
                };
                minDate = Vanrise.Common.Utilities.Min(minDate, newDefaultRoutingProduct.BED);
            }
            else if (changedDefaultRoutingProduct != null)
            {
                defaultRoutingProductToClose = new DefaultRoutingProductToClose()
                {
                    CloseEffectiveDate = changedDefaultRoutingProduct.EED
                };
                minDate = Vanrise.Common.Utilities.Min(minDate, changedDefaultRoutingProduct.EED);
            }
        }

        private void SetDefaultServiceActions(DraftNewDefaultService newDefaultService, DraftClosedDefaultService closedDefaultService, DraftResetDefaultService resetDefaultService, out DefaultServiceToAdd defaultServiceToAdd, out DefaultServiceToClose defaultServiceToClose, ref DateTime minDate)
        {
            defaultServiceToAdd = null;
            defaultServiceToClose = null;

            if (newDefaultService != null)
            {
                defaultServiceToAdd = new DefaultServiceToAdd()
                {
                    NewDefaultService = new NewDefaultService()
                    {
                        Services = newDefaultService.Services,
                        BED = newDefaultService.BED,
                        EED = newDefaultService.EED
                    },
                    BED = newDefaultService.BED,
                    EED = newDefaultService.EED
                };
                minDate = Vanrise.Common.Utilities.Min(minDate, newDefaultService.BED);
            }
            else if (closedDefaultService != null)
            {
                defaultServiceToClose = new DefaultServiceToClose()
                {
                    CloseEffectiveDate = closedDefaultService.EED
                };
                minDate = Vanrise.Common.Utilities.Min(minDate, closedDefaultService.EED);
            }
            else if (resetDefaultService != null)
            {
                defaultServiceToClose = new DefaultServiceToClose()
                {
                    CloseEffectiveDate = resetDefaultService.EED
                };
                minDate = Vanrise.Common.Utilities.Min(minDate, resetDefaultService.EED);
            }
        }

        #endregion

        #region Set Zone Actions

        private void SetZoneActions(ref List<RateToChange> ratesToChange, ref List<RateToClose> ratesToClose, ref List<SaleZoneRoutingProductToAdd> saleZoneRoutingProductsToAdd, ref List<SaleZoneRoutingProductToClose> saleZoneRoutingProductsToClose, ref List<SaleZoneServiceToAdd> saleZoneServicesToAdd, ref List<SaleZoneServiceToClose> saleZoneServicesToClose, IEnumerable<ZoneChanges> zoneDrafts, int currencyId, ref DateTime minDate)
        {
            if (zoneDrafts == null)
                return;

            foreach (ZoneChanges zoneDraft in zoneDrafts)
            {
                SetZoneRateActions(ref ratesToChange, ref ratesToClose, zoneDraft, currencyId, ref minDate);
                SetZoneRoutingProductActions(ref saleZoneRoutingProductsToAdd, ref saleZoneRoutingProductsToClose, zoneDraft, ref minDate);
                SetZoneServiceActions(ref saleZoneServicesToAdd, ref saleZoneServicesToClose, zoneDraft, ref minDate);
            }
        }

        private void SetZoneRateActions(ref List<RateToChange> ratesToChange, ref List<RateToClose> ratesToClose, ZoneChanges zoneChanges, int currencyId, ref DateTime minDate)
        {
            if (zoneChanges.NewRates != null)
            {
                foreach (DraftRateToChange newRate in zoneChanges.NewRates)
                {
                    ratesToChange.Add(new RateToChange()
                    {
                        ZoneId = zoneChanges.ZoneId,
                        ZoneName = zoneChanges.ZoneName,
                        RateTypeId = newRate.RateTypeId,
                        NormalRate = newRate.NormalRate,
                        CurrencyId = currencyId,
                        BED = newRate.BED,
                        EED = newRate.EED
                    });
                    minDate = Vanrise.Common.Utilities.Min(minDate, newRate.BED);
                }
            }

            if (zoneChanges.ClosedRates != null)
            {
                foreach (DraftRateToClose closedRate in zoneChanges.ClosedRates)
                {
                    ratesToClose.Add(new RateToClose()
                    {
                        ZoneId = zoneChanges.ZoneId,
                        ZoneName = zoneChanges.ZoneName,
                        RateTypeId = closedRate.RateTypeId,
                        CloseEffectiveDate = closedRate.EED
                    });
                    minDate = Vanrise.Common.Utilities.Min(minDate, closedRate.EED);
                }
            }
        }

        private void SetZoneRoutingProductActions(ref List<SaleZoneRoutingProductToAdd> saleZoneRoutingProductsToAdd, ref List<SaleZoneRoutingProductToClose> saleZoneRoutingProductsToClose, ZoneChanges zoneChanges, ref DateTime minDate)
        {
            if (zoneChanges.NewRoutingProduct != null)
            {
                saleZoneRoutingProductsToAdd.Add(new SaleZoneRoutingProductToAdd()
                {
                    ZoneId = zoneChanges.NewRoutingProduct.ZoneId,
                    ZoneName = zoneChanges.ZoneName,
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
                    ZoneName = zoneChanges.ZoneName,
                    CloseEffectiveDate = zoneChanges.RoutingProductChange.EED
                });
                minDate = Vanrise.Common.Utilities.Min(minDate, zoneChanges.RoutingProductChange.EED);
            }
        }

        private void SetZoneServiceActions(ref List<SaleZoneServiceToAdd> saleZoneServicesToAdd, ref List<SaleZoneServiceToClose> saleZoneServicesToClose, ZoneChanges zoneDraft, ref DateTime minDate)
        {
            if (zoneDraft.NewService != null)
            {
                saleZoneServicesToAdd.Add(new SaleZoneServiceToAdd()
                {
                    ZoneId = zoneDraft.ZoneId,
                    ZoneName = zoneDraft.ZoneName,
                    Services = zoneDraft.NewService.Services,
                    BED = zoneDraft.NewService.BED,
                    EED = zoneDraft.NewService.EED
                });
                minDate = Vanrise.Common.Utilities.Min(minDate, zoneDraft.NewService.BED);
            }
            else if (zoneDraft.ClosedService != null)
            {
                saleZoneServicesToClose.Add(new SaleZoneServiceToClose()
                {
                    ZoneId = zoneDraft.ZoneId,
                    ZoneName = zoneDraft.ZoneName,
                    CloseEffectiveDate = zoneDraft.ClosedService.EED
                });
                minDate = Vanrise.Common.Utilities.Min(minDate, zoneDraft.ClosedService.EED);
            }
            else if (zoneDraft.ResetService != null)
            {
                saleZoneServicesToClose.Add(new SaleZoneServiceToClose()
                {
                    ZoneId = zoneDraft.ZoneId,
                    ZoneName = zoneDraft.ZoneName,
                    CloseEffectiveDate = zoneDraft.ResetService.EED
                });
                minDate = Vanrise.Common.Utilities.Min(minDate, zoneDraft.ResetService.EED);
            }
        }

        #endregion

        #endregion
    }
}
