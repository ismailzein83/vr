using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.GenericData.Business;

namespace Retail.RA.Business
{
    public class RAPeriodDefinitionOnBeforeSaveHandler : GenericBEOnBeforeInsertHandler
    {
        public override Guid ConfigId { get { return new Guid("56EE0B5C-DA52-4B8C-B205-FE536F969659"); } }
        public override void Execute(IGenericBEOnBeforeInsertHandlerContext context)
        {
            PeriodDefinitionManager periodDefinitionManager = new PeriodDefinitionManager();
            context.ThrowIfNull("context");
            context.GenericBusinessEntity.ThrowIfNull("context.GenericBusinessEntity");
            context.GenericBusinessEntity.FieldValues.ThrowIfNull("context.GenericBusinessEntity.FieldValues");
            var from = context.GenericBusinessEntity.FieldValues.GetRecord("From");
            if (from == null)
                throw new NullReferenceException("from");
            var to = context.GenericBusinessEntity.FieldValues.GetRecord("To");
            if (to == null)
                throw new NullReferenceException("to");
            var currentName = context.GenericBusinessEntity.FieldValues.GetRecord("Name");
            var name = string.Join(" To ", ((DateTime)from).ToString(Utilities.GetDateTimeFormat(Vanrise.Entities.DateTimeType.Date)), ((DateTime)from).ToString(Utilities.GetDateTimeFormat(Vanrise.Entities.DateTimeType.Date)));
            if (currentName == null)
                context.GenericBusinessEntity.FieldValues.Add("Name", name);
            else
            {
                currentName = name;
            }

            if (periodDefinitionManager.IsPeriodOverlapping((DateTime)from, (DateTime)to))
            {
                context.OutputResult.Result = false;
                context.OutputResult.Messages.Add(string.Format("Period definition '{0}' is overlapping with other periods", name));
            }
        }
    }
}
