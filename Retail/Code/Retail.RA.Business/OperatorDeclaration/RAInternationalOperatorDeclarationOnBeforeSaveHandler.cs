using Retail.RA.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;
using Vanrise.GenericData.Business;

namespace Retail.RA.Business
{
    public class RAInternationalOperatorDeclarationOnBeforeSaveHandler : GenericBEOnBeforeInsertHandler
    {
        public override Guid ConfigId { get { return new Guid("F7D8C1E5-DF05-4EA1-B43A-5BE5198BE6ED"); } }
        public override void Execute(IGenericBEOnBeforeInsertHandlerContext context)
        {
            List<IntlOperationDeclarationTrafficItem> traficItems = new List<IntlOperationDeclarationTrafficItem>();
            context.ThrowIfNull("context");
            context.GenericBusinessEntity.ThrowIfNull("context.GenericBusinessEntity");
            context.GenericBusinessEntity.FieldValues.ThrowIfNull("context.GenericBusinessEntity.FieldValues");
            IntlOperatorDeclarationManager operatorDeclarationManager = new IntlOperatorDeclarationManager();
            var operatorDeclarations = operatorDeclarationManager.GetAllOperatorDecalarations();
            var id = context.GenericBusinessEntity.FieldValues.GetRecord("ID");
            var operatorId = context.GenericBusinessEntity.FieldValues.GetRecord("Operator");
            operatorId.ThrowIfNull("operatorId");
            var periodId = context.GenericBusinessEntity.FieldValues.GetRecord("Period");
            periodId.ThrowIfNull("periodId");
            var operatorDeclarationServices = context.GenericBusinessEntity.FieldValues.GetRecord("OperatorDeclarationServices") as IntlOperatorDeclarationServices;
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
                    traficItems.Add(new IntlOperationDeclarationTrafficItem()
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