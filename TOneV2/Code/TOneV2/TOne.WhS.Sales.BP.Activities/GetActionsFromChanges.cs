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
    public class GetActionsFromChanges : CodeActivity
    {
        #region Arguments

        [RequiredArgument]
        public InArgument<Changes> Changes { get; set; }

        [RequiredArgument]
        public InArgument<int> CurrencyId { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<RateToChange>> RatesToChange { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<RateToClose>> RatesToClose { get; set; }

        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            Changes changes = this.Changes.Get(context);
            int currencyId = this.CurrencyId.Get(context);

            var ratesToChange = new List<RateToChange>();
            var ratesToClose = new List<RateToClose>();

            if (changes != null && changes.ZoneChanges != null)
            {
                var saleZoneManager = new SaleZoneManager();

                foreach (ZoneChanges zoneChanges in changes.ZoneChanges)
                {
                    if (zoneChanges.NewRate != null)
                    {
                        ratesToChange.Add(new RateToChange()
                        {
                            ZoneName = saleZoneManager.GetSaleZoneName(zoneChanges.ZoneId),
                            NormalRate = zoneChanges.NewRate.NormalRate,
                            CurrencyId = currencyId,
                            BED = zoneChanges.NewRate.BED,
                            EED = zoneChanges.NewRate.EED
                        });
                    }
                    else if (zoneChanges.RateChange != null) // Is this check necessary?
                    {
                        ratesToClose.Add(new RateToClose()
                        {
                            ZoneName = saleZoneManager.GetSaleZoneName(zoneChanges.ZoneId),
                            CloseEffectiveDate = zoneChanges.RateChange.EED
                        });
                    }
                }
            }

            this.RatesToChange.Set(context, (ratesToChange.Count > 0) ? ratesToChange : null);
            this.RatesToClose.Set(context, (ratesToClose.Count > 0) ? ratesToClose : null);
        }
    }
}
