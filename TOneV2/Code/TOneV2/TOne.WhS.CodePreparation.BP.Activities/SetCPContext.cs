using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.CodePreparation.Entities;
using TOne.WhS.CodePreparation.Entities.Processing;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.CodePreparation.BP.Activities
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
            CustomerCountryManager customerCountryManager = new CustomerCountryManager();
            CPParametersContext cpParametersContext = context.GetCPParameterContext() as CPParametersContext;
            cpParametersContext.SellingNumberPlanId = SellingNumberPlanId.Get(context);
            cpParametersContext.EffectiveDate = EffectiveDate.Get(context);
            cpParametersContext.Customers = new CarrierAccountManager().GetCustomersBySellingNumberPlanId(cpParametersContext.SellingNumberPlanId);
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
        public IEnumerable<CarrierAccountInfo> Customers { get; set; }
        public DateTime EffectiveDate { get; set; }

        public int SellingNumberPlanId { get; set; }
      
        public ExistingZoneInfoByZoneName ExistingZonesInfoByZoneName { get; set; }
    }
}
