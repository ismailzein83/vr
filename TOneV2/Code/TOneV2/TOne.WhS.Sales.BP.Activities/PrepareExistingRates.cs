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
        #region Arguments

        [RequiredArgument]
        public InArgument<IEnumerable<SaleRate>> ExistingSaleRates { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<ExistingZone>> ExistingZones { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<ExistingRate>> ExistingRates { get; set; }

        //[RequiredArgument]
        //public OutArgument<ExistingRatesByZoneName> ExistingRatesByZoneName { get; set; }

        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<SaleRate> saleRates = this.ExistingSaleRates.Get(context);
            IEnumerable<ExistingZone> existingZones = this.ExistingZones.Get(context);

            List<ExistingRate> existingRates = null;
            //ExistingRatesByZoneName existingRatesByZoneNameDictionary = null;

            if (saleRates != null)
            {
                existingRates = new List<ExistingRate>();
                //existingRatesByZoneNameDictionary = new ExistingRatesByZoneName();

                foreach (SaleRate saleRate in saleRates)
                {
                    ExistingZone parentExistingZone = existingZones.FindRecord((existingZone) => existingZone.ZoneId == saleRate.ZoneId);
                    if (parentExistingZone == null)
                        throw new NullReferenceException("parentExistingZone");
                    existingRates.Add(new ExistingRate()
                    {
                        RateEntity = saleRate,
                        ParentZone = parentExistingZone
                    });
                }

                //IEnumerable<string> distinctExistingZoneNames = existingZones.MapRecords((existingZone) => existingZone.Name).Distinct();

                //foreach (string existingZoneName in distinctExistingZoneNames)
                //{
                //    IEnumerable<ExistingRate> existingRatesByZoneNameList = existingRates.FindAllRecords((existingRate) => existingRate.ParentZone.Name == existingZoneName);
                //    existingRatesByZoneNameDictionary.Add(existingZoneName, (existingRatesByZoneNameList.Count() > 0) ? existingRatesByZoneNameList.ToList() : null);
                //}
            }

            this.ExistingRates.Set(context, existingRates);
            //this.ExistingRatesByZoneName.Set(context, existingRatesByZoneNameDictionary);
        }
    }
}
