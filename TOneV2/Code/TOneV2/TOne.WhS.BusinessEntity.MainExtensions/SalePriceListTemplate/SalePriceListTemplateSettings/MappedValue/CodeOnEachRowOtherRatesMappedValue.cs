using System;
using Vanrise.Common;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions
{
    public class CodeOnEachRowOtherRatesMappedValue : CodeOnEachRowMappedValue
    {
        public override Guid ConfigId
        {
            get { return new Guid("70500270-0BFD-4AEA-A20A-969BA0F6489B"); }
        }

        public int RateTypeId { get; set; }
        public override void Execute(ICodeOnEachRowMappedValueContext context)
        {
            var otherRate = context.OtherRateByRateTypeId.GetRecord(RateTypeId);
            if (otherRate != null && otherRate.EED == null)
                context.Value = otherRate.Rate;
        }

    }
}
