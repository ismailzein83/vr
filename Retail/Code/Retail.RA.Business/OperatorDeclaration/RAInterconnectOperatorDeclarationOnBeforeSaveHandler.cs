using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.Common;
using Retail.RA.Entities;

namespace Retail.RA.Business
{
    public class RAInterconnectOperatorDeclarationOnBeforeSaveHandler : GenericBEOnBeforeInsertHandler
    {
        public override Guid ConfigId { get { return new Guid("DD7AA977-2836-4BB1-9F22-B885B614D107"); } }
        public override void Execute(IGenericBEOnBeforeInsertHandlerContext context)
        {
            List<OperationDeclarationTrafficItem> traficItems = new List<OperationDeclarationTrafficItem>();
            context.ThrowIfNull("context");
            context.GenericBusinessEntity.ThrowIfNull("context.GenericBusinessEntity");
            context.GenericBusinessEntity.FieldValues.ThrowIfNull("context.GenericBusinessEntity.FieldValues");
            IcxOperatorDeclarationManager operatorDeclarationManager = new IcxOperatorDeclarationManager();
            var operatorDeclarations = operatorDeclarationManager.GetAllOperatorDecalarations();
            var id = context.GenericBusinessEntity.FieldValues.GetRecord("ID");
            var operatorId = context.GenericBusinessEntity.FieldValues.GetRecord("Operator");
            operatorId.ThrowIfNull("operatorId");
            var periodId = context.GenericBusinessEntity.FieldValues.GetRecord("Period");
            periodId.ThrowIfNull("periodId");

            var operatorDeclarationServices = context.GenericBusinessEntity.FieldValues.GetRecord("OperatorDeclarationServices") as OperatorDeclarationServices;
            foreach (var service in operatorDeclarationServices.Services)
            {
                service.ThrowIfNull("Service");
                service.Settings.ThrowIfNull("service.Settings");
                var trafficItemExist = traficItems.Any(x => x.ServiceType == service.Settings.GetServiceType() && x.TrafficDirection == service.Settings.GetTrafficDirection());
                if (trafficItemExist)
                {
                    context.OutputResult.Result = false;
                    context.OutputResult.Messages.Add("Cannot add more than one declaration with the same type and direction");
                    break;
                }
                else
                    traficItems.Add(new OperationDeclarationTrafficItem()
                    {
                        ServiceType = service.Settings.GetServiceType(),
                        TrafficDirection = service.Settings.GetTrafficDirection()
                    });

            }
            var operatorDeclaration = operatorDeclarations.FindRecord(x => x.PeriodId == Convert.ToInt32(periodId) && x.OperatorId == Convert.ToInt64(operatorId));
            if (operatorDeclaration != null && (id == null || operatorDeclaration.ID != Convert.ToInt64(id)))
            {
                context.OutputResult.Result = false;
                context.OutputResult.Messages.Add("An operator declaration is already created with the same period declaration");
            }

        }
        
    }
}
