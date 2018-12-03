using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.GenericData.Business;

namespace Retail.RA.Business
{
    public class RAPeriodDefinitionOnBeforeSaveHandler: GenericBEOnBeforeInsertHandler
    {
        public override Guid ConfigId { get { return new Guid("56EE0B5C-DA52-4B8C-B205-FE536F969659"); } }
        public override void Execute(IGenericBEOnBeforeInsertHandlerContext context)
        {
            //context.ThrowIfNull("context");
            //context.GenericBusinessEntity.ThrowIfNull("context.GenericBusinessEntity");
            //context.GenericBusinessEntity.FieldValues.ThrowIfNull("context.GenericBusinessEntity.FieldValues");
            //var from = context.GenericBusinessEntity.FieldValues.GetRecord("From");
            //var to = context.GenericBusinessEntity.FieldValues.GetRecord("To");
            //var repeat = context.GenericBusinessEntity.FieldValues.GetRecord("Repeat");


        }
    }
}
