using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.NumberingPlan.Entities;

namespace Vanrise.NumberingPlan.BP.Activities
{

    public sealed class SetCPContext : CodeActivity
    {

        [RequiredArgument]
        public InArgument<DateTime> EffectiveDate { get; set; }

        [RequiredArgument]
        public InArgument<int> SellingNumberPlanId { get; set; }
        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            metadata.AddDefaultExtensionProvider<ICPParametersContext>(() => new CPParametersContext());
            base.CacheMetadata(metadata);
        }

        protected override void Execute(CodeActivityContext context)
        {
            CPParametersContext cpParametersContext = context.GetCPParameterContext() as CPParametersContext;
            cpParametersContext.SellingNumberPlanId = SellingNumberPlanId.Get(context);
            cpParametersContext.EffectiveDate = EffectiveDate.Get(context);
        }
    }

    internal static class ContextExtensionMethods
    {
        public static ICPParametersContext GetCPParameterContext(this ActivityContext context)
        {
            var parameterContext = context.GetExtension<ICPParametersContext>();
            if (parameterContext == null)
                throw new NullReferenceException("parameterContext");
            return parameterContext;
        }
    }


    public class CPParametersContext : ICPParametersContext
    {

        public DateTime EffectiveDate { get; set; }

        public int SellingNumberPlanId { get; set; }
      
        public ExistingZoneInfoByZoneName ExistingZonesInfoByZoneName { get; set; }
    }
}
