using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class TechnicalZoneOnBeforeSaveHandler : GenericBEOnBeforeInsertHandler
    {
        public override Guid ConfigId { get { return new Guid("DA50B323-DAB4-4E5D-807F-7BF9642215B4"); } }

        public override void Execute(IGenericBEOnBeforeInsertHandlerContext context)
        {
            context.ThrowIfNull("context");
            context.GenericBusinessEntity.ThrowIfNull("context.GenericBusinessEntity");
            context.GenericBusinessEntity.FieldValues.ThrowIfNull("context.GenericBusinessEntity.FieldValues");

            TechnicalZoneManager technicalZoneManager = new TechnicalZoneManager();
            if (!technicalZoneManager.CanAddMoreZones())
            {
                var zoneName = (string)context.GenericBusinessEntity.FieldValues.GetRecord("ZoneName");
                context.OutputResult.Result = false;
                context.OutputResult.Messages.Add($"Cannot Add More Zones! Failed to add Zone '{zoneName}'");
            }
        }
    }
}
