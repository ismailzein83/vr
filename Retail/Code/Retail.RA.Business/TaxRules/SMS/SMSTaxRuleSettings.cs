using System;
using System.Collections.Generic;
using Vanrise.GenericData.Entities;

namespace Retail.RA.Business
{
    public abstract class SMSTaxRuleSettings
    {
        public abstract Guid ConfigId { get; }

        protected abstract void Execute(ISMSTaxRuleContext context);

        public void ApplySMSTaxRule(ISMSTaxRuleContext context)
        {
            this.Execute(context);
        }

        public abstract string GetDescription(IGenericRuleSettingsDescriptionContext context);

        public virtual Dictionary<string, object> GetSettingsValuesByName()
        {
            return null;
        }
    }
}