using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.CodePreparation.Entities.Processing;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using TOne.WhS.CodePreparation.Entities;

namespace TOne.WhS.CodePreparation.BP.Activities
{

    public sealed class PrepareExistingEffectiveRates : CodeActivity
    {

        [RequiredArgument]
        public InArgument<IEnumerable<ExistingRate>> ExistingRates { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> MinimumDate { get; set; }

        [RequiredArgument]
        public OutArgument<ExistingRatesByZoneName> EffectiveExistingRatesByZoneName { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<ExistingRate> existingRates = this.ExistingRates.Get(context);
            DateTime minimumDate = this.MinimumDate.Get(context);

            IEnumerable<ExistingRate> effectiveExistingRates = existingRates.FindAllRecords(itm => itm.BED <= minimumDate && (itm.EED == null || itm.EED.Value > minimumDate));
            ExistingRatesByZoneName existingEffectiveRatesByZoneName = StructureEffectiveExistingRatesByZoneName(effectiveExistingRates);

            EffectiveExistingRatesByZoneName.Set(context, existingEffectiveRatesByZoneName);
        }

        private ExistingRatesByZoneName StructureEffectiveExistingRatesByZoneName(IEnumerable<ExistingRate> effectiveExistingRates)
        {
            ExistingRatesByZoneName effectiveExistingRatesByZoneName = new ExistingRatesByZoneName();
            if (effectiveExistingRates != null)
            {
                List<ExistingRate> existingRates;
                foreach (ExistingRate existingRate in effectiveExistingRates)
                {
                    if (!effectiveExistingRatesByZoneName.TryGetValue(existingRate.ParentZone.Name, out existingRates))
                    {
                        existingRates = new List<ExistingRate>();
                        effectiveExistingRatesByZoneName.Add(existingRate.ParentZone.Name, existingRates);
                    }
                    existingRates.Add(existingRate);
                }
            }
            return effectiveExistingRatesByZoneName;
        }
    }
}
