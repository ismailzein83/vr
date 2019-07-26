using System;
using System.Collections.Generic;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Business
{
    public class CustomerFaultTicketBeforeInsertHandler : GenericBEOnBeforeInsertHandler
    {
        public override Guid ConfigId
        {
            get { return new Guid("23FA8A42-8E96-4F26-96BB-DAB1E50B9534"); }
        }

        public override void Execute(IGenericBEOnBeforeInsertHandlerContext context)
        {
            if (context.OperationType == HandlerOperationType.Add)
            {
                context.GenericBusinessEntity.ThrowIfNull("context.BusinessEntityDefinitionId");
                context.GenericBusinessEntity.ThrowIfNull("context.GenericBusinessEntity");
                GenericBusinessEntityManager genericBusinessEntityManager = new GenericBusinessEntityManager();

                var entities = genericBusinessEntityManager.GetAllGenericBusinessEntities(context.BusinessEntityDefinitionId, new List<string> { "SaleZoneId", "CustomerId", "StatusId" }, new RecordFilterGroup
                {
                    LogicalOperator = RecordQueryLogicalOperator.And,
                    Filters = new List<RecordFilter>
                {
                    new ObjectListRecordFilter
                    {
                        FieldName = "SaleZoneId",
                        Values =new List<object> { (long)context.GenericBusinessEntity.FieldValues.GetRecord("SaleZoneId") }
                    },
                    new ObjectListRecordFilter
                    {
                        FieldName ="CustomerId",
                        Values =new List<object>{(int)context.GenericBusinessEntity.FieldValues.GetRecord("CustomerId") }
                    }
                    ,
                    new ObjectListRecordFilter
                    {
                        CompareOperator = ListRecordFilterOperator.NotIn,
                        FieldName ="StatusId",
                        Values =new List<object> { new Guid("f299eb6d-b50c-4338-812f-142d4d8515ca") }
                    }
                }
                });

                if (entities != null && entities.Count > 0)
                {
                    context.OutputResult.Result = false;
                    context.OutputResult.Messages.Add("Same Customer And SaleZone Already Exist In Another Fault Ticket");
                }
            }
        }

    }

}

