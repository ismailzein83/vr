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
    public class PrepareExistingZones : CodeActivity
    {
        #region Input Arguments

        [RequiredArgument]
        public InArgument<IEnumerable<SaleZone>> ExistingSaleZones { get; set; }

        #endregion

        #region Output Arguments

        [RequiredArgument]
        public OutArgument<IEnumerable<ExistingZone>> ExistingZones { get; set; }

        [RequiredArgument]
        public OutArgument<Dictionary<long, ExistingZone>> ExistingZonesById { get; set; }

        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<SaleZone> saleZones = this.ExistingSaleZones.Get(context);
            var existingZones = new List<ExistingZone>();

            foreach (SaleZone saleZone in saleZones)
            {
                existingZones.Add(new ExistingZone()
                {
                    ZoneEntity = saleZone
                });
            }

            Dictionary<long, ExistingZone> existingZonesById = existingZones.ToDictionary<ExistingZone, long>(x => x.ZoneId);

            ExistingZones.Set(context, existingZones);
            ExistingZonesById.Set(context, existingZonesById);
        }
    }
}
