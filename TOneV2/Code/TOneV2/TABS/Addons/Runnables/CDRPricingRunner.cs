using System;

namespace TABS.Addons.Runnables
{
    [NamedAddon("T.One - CDR Pricing", "Generates Billing CDR information (Main, Cost, Sale and Invalid) from raw CDR Records")]
    public class CDRPricingRunner : RunnableBase
    {
        public override void Run()
        {
            TABS.Components.Engine.GenerateCDRPricing();
        }

        /// <summary>
        /// Request a stop for the operation
        /// </summary>
        /// <returns></returns>
        public override bool Stop()
        {
            TABS.Components.Engine.StopCDRPricing();
            TABS.Components.Engine.PricingCodeMap = null;
            //TABS.CodeMap.ClearCachedCollections();
            GC.Collect();
            return base.Stop();
        }

        public override string Status { get { return string.Empty; } }
    }
}
