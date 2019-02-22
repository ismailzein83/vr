using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Business;

namespace TOne.WhS.BusinessEntity.Business
{
    public class TechnicalNumberPlanOnBeforeSaveHandler : GenericBEOnBeforeInsertHandler
    {
        public override Guid ConfigId { get { return new Guid("405A4099-3F38-426C-A5A4-DCBB6A44F90A"); } }

        public override void Execute(IGenericBEOnBeforeInsertHandlerContext context)
        {
            //TechnicalNumberPlanManager technicalNumberPlanManager = new TechnicalNumberPlanManager();
            //context.ThrowIfNull("context");
            //context.GenericBusinessEntity.ThrowIfNull("context.GenericBusinessEntity");
            //context.GenericBusinessEntity.FieldValues.ThrowIfNull("context.GenericBusinessEntity.FieldValues");
            //var zoneName = (string)context.GenericBusinessEntity.FieldValues.GetRecord("ZoneName");
            //zoneName.ThrowIfNull("codes");

            //var zoneCodes = (List<ZoneCode>)context.GenericBusinessEntity.FieldValues.GetRecord("Codes");
            //zoneCodes.ThrowIfNull("codes");

            //List<TechnicalNumberPlan> allTechnicalNumberPlans = technicalNumberPlanManager.GetTechnicalNumberPlans();

            //if (context.OperationType == HandlerOperationType.Add)
            //{
            //    foreach (var zoneCode in zoneCodes)
            //    {
            //        if (allTechnicalNumberPlans.Any(itm => itm.Codes.Select(item => item.Code).Contains(zoneCode.Code)))
            //        {
            //            context.OutputResult.Result = false;
            //            context.OutputResult.Messages.Add($"Code '{zoneCode.Code}' already exist.");
            //        }
            //    }

            //    return;
            //}

            //foreach (var zoneCode in zoneCodes)
            //{
            //    if (allTechnicalNumberPlans.Any(itm => itm.Codes.Select(item => item.Code).Contains(zoneCode.Code)))
            //    {
            //        context.OutputResult.Result = false;
            //        context.OutputResult.Messages.Add($"Code '{zoneCode.Code}' already exist.");
            //    }
            //}
        }

    }
}
