using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;

namespace TOne.WhS.Sales.BP.Activities
{
    public class PrepareExistingRates : CodeActivity
    {
        #region Input Arguments

        [RequiredArgument]
        public InArgument<IEnumerable<SaleRate>> ExistingSaleRates { get; set; }

        [RequiredArgument]
        public InArgument<Dictionary<long, ExistingZone>> ExistingZonesById { get; set; }

        #endregion

        #region Output Arguments

        [RequiredArgument]
        public OutArgument<IEnumerable<ExistingRate>> ExistingRates { get; set; }

        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<SaleRate> saleRates = this.ExistingSaleRates.Get(context);
            Dictionary<long, ExistingZone> existingZonesById = ExistingZonesById.Get(context);

            var existingRates = saleRates.MapRecords(saleRate => ExistingRateMapper(saleRate, existingZonesById));
            this.ExistingRates.Set(context, existingRates);
        }

        #region Private Methods

        private ExistingRate ExistingRateMapper(SaleRate saleRate, Dictionary<long, ExistingZone> existingZonesById)
        {
            ExistingZone existingZone;

            if (!existingZonesById.TryGetValue(saleRate.ZoneId, out existingZone))
                throw new NullReferenceException(String.Format("SaleRate '{0}' is not linked to SaleZone '{1}'", saleRate.SaleRateId, saleRate.ZoneId));

            return new ExistingRate()
            {
                RateEntity = saleRate,
                ParentZone = existingZone
            };
        }

        #endregion
    }
}
