using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.Security.Entities;
using Vanrise.Common;

namespace Vanrise.Security.MainExtensions.GenericBEOnBeforeInsertHandlers
{
    public class SecurityProviderOnBeforeInsertHandler : GenericBEOnBeforeInsertHandler
    {
        public override Guid ConfigId { get { return new Guid("B47D2D8D-A9D3-42E1-91B1-25807167CE86"); } }

        public override void Execute(IGenericBEOnBeforeInsertHandlerContext context)
        {
            context.ThrowIfNull("context");
            context.GenericBusinessEntity.ThrowIfNull("context.GenericBusinessEntity");
            context.GenericBusinessEntity.FieldValues.ThrowIfNull("context.GenericBusinessEntity.FieldValues");

            SecurityProviderSettings previousSecurityProviderSettings = null;
            if (context.OldGenericBusinessEntity != null && context.OldGenericBusinessEntity.FieldValues != null)
                previousSecurityProviderSettings = context.OldGenericBusinessEntity.FieldValues.GetRecord("Settings") as SecurityProviderSettings;

            SecurityProviderOnBeforeSaveContext securityProviderOnBeforeSaveContext = new Entities.SecurityProviderOnBeforeSaveContext()
            {
                PreviousSettings = previousSecurityProviderSettings != null ? previousSecurityProviderSettings.ExtendedSettings : null
            };

            var securityProviderSettings = context.GenericBusinessEntity.FieldValues.GetRecord("Settings") as SecurityProviderSettings;
            securityProviderSettings.ThrowIfNull("securityProviderSettings");
            securityProviderSettings.ExtendedSettings.ThrowIfNull("securityProviderSettings.ExtendedSettings");
            securityProviderSettings.ExtendedSettings.OnBeforeSave(securityProviderOnBeforeSaveContext);

            if (!string.IsNullOrEmpty(securityProviderOnBeforeSaveContext.ErrorMessage))
                throw new Exception(securityProviderOnBeforeSaveContext.ErrorMessage);
        }
    }
}