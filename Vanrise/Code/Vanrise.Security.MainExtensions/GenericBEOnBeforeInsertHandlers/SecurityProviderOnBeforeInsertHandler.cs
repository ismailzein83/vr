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
            //SecurityProviderOnBeforeSaveContext securityProviderOnBeforeSaveContext = new Entities.SecurityProviderOnBeforeSaveContext();
            //var securityProviderSettings = context.GenericBusinessEntity.FieldValues.GetRecord("Settings") as SecurityProviderSettings;
            //securityProviderSettings.ExtendedSettings.OnBeforeSave(securityProviderOnBeforeSaveContext);

            //if (!string.IsNullOrEmpty(securityProviderOnBeforeSaveContext.ErrorMessage))
            //    throw new Exception(securityProviderOnBeforeSaveContext.ErrorMessage);
        }
    }
}