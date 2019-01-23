using System;
using System.Collections.Generic;
using System.Text;
using Vanrise.GenericData.Entities;
using TOne.WhS.Jazz.Entities;
namespace TOne.WhS.Jazz.Business
{
public class WhSJazzAmountCalculationCustomObject : FieldCustomObjectTypeSettings
    {

        public override Guid ConfigId { get { return new Guid("7AF6055C-86A5-4E3F-A3B8-F37EC1A056E9"); } }

        public override string GetDescription(IFieldCustomObjectTypeSettingsContext context)
        {
            return null;
        }

        public override bool AreEqual(Object newValue, Object oldValue)
        {
            return true;
        }

        public override Type GetNonNullableRuntimeType()
        {
            return typeof(AmountCalculation);
        }

        public override dynamic ParseNonNullValueToFieldType(object originalValue)
        {
                return originalValue as AmountCalculation;
        }

        public override string GetRuntimeTypeDescription()
        {
            return "Ammount Calculation";
        }
    }
}