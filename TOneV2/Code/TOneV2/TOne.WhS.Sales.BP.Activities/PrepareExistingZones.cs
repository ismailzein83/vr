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
        #region Arguments

        [RequiredArgument]
        public InArgument<IEnumerable<SaleZone>> ExistingSaleZones { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<ExistingZone>> ExistingZones { get; set; }

        //[RequiredArgument]
        //public OutArgument<ExistingZonesByName> ExistingZonesByName { get; set; }

        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<SaleZone> saleZones = this.ExistingSaleZones.Get(context);
            var existingZones = new List<ExistingZone>();
            //var existingZonesByNameDictionary = new ExistingZonesByName();

            foreach (SaleZone saleZone in saleZones)
            {
                existingZones.Add(new ExistingZone()
                {
                    ZoneEntity = saleZone
                });
            }

            //IEnumerable<string> distinctSaleZoneNames = saleZones.MapRecords((saleZone) => saleZone.Name).Distinct();

            //foreach (string saleZoneName in distinctSaleZoneNames)
            //{
            //    List<ExistingZone> existingZonesByNameList = existingZones.FindAllRecords((existingZone) => existingZone.Name == saleZoneName).ToList();
            //    existingZonesByNameDictionary.Add(saleZoneName, existingZonesByNameList);
            //}

            this.ExistingZones.Set(context, existingZones);
            //this.ExistingZonesByName.Set(context, existingZonesByNameDictionary);
        }
    }
}
