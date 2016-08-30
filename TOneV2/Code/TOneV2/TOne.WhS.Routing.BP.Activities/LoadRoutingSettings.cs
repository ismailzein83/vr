using System.Activities;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.BP.Activities
{
    public sealed class LoadRoutingSettings : CodeActivity
    {
        [RequiredArgument]
        public OutArgument<int> CustomerGroupSize { get; set; }

        [RequiredArgument]
        public OutArgument<int> SupplierGroupSize { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            ConfigManager configManager = new ConfigManager();
            SubProcessSettings settings = configManager.GetSubProcessSettings();
            int customerGroupSize = settings.CustomerGroupSize;
            int supplierGroupSize = settings.SupplierGroupSize;
            this.CustomerGroupSize.Set(context, customerGroupSize);
            this.SupplierGroupSize.Set(context, supplierGroupSize);
        }
    }
}