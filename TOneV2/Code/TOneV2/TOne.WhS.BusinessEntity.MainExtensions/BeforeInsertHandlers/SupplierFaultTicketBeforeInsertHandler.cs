using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using TOne.WhS.Jazz.Business;
using Vanrise.Entities;

namespace TOne.WhS.Jazz.Business
{
    public class SupplierFaultTicketBeforeInsertHandler : GenericBEOnBeforeInsertHandler
    {
        public override Guid ConfigId
        {
            get { return new Guid("562B655A-3D2A-4CD7-85AA-BABFF12D5F5E"); }
        }


        public override void Execute(IGenericBEOnBeforeInsertHandlerContext context)
        {

            context.GenericBusinessEntity.ThrowIfNull("context.GenericBusinessEntity");
            GenericBusinessEntityManager genericBusinessEntityManager = new GenericBusinessEntityManager();

            var entities = genericBusinessEntityManager.GetAllGenericBusinessEntities(context.BusinessEntityDefinitionId, new List<string> { "SaleZoneId", "SupplierId", "StatusId" }, new RecordFilterGroup
            {

                Filters = new List<RecordFilter>
                {
                    new ObjectListRecordFilter
                    {
                        FieldName = "SaleZoneId",
                        Values =new List<object> { (long)context.GenericBusinessEntity.FieldValues.GetRecord("SaleZoneId") }
                    },
                    new ObjectListRecordFilter
                    {
                        FieldName ="SupplierId",
                        Values =new List<object>{(int)context.GenericBusinessEntity.FieldValues.GetRecord("SupplierId") }
                    },
                    new ObjectListRecordFilter
                    {
                        FieldName ="StatusId",
                        Values =new List<object>{"7EB94106-43F1-43EB-8952-8F0B585FD7E5", "05A87955-DC2A-4E22-A879-6BEA3C31690E" }
                    }
                }
            });

            if (entities != null && entities.Count > 0)
            {
                {
                    context.OutputResult.Result = false;
                    context.OutputResult.Messages.Add("Same Supplier And SaleZone Already Exists In Another Fault Ticket");
                }
            }
        }

    }

}

