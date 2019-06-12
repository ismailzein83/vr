using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;

namespace Vanrise.GenericData.MainExtensions.GenericBusinessEntity.GenericBEOnBeforeInsertHandlers
{
    public class SetGenericBEFixedValuesBeforeSaveHandler : GenericBEOnBeforeInsertHandler
    {
        public override Guid ConfigId
        {
            get { return new Guid("080E340A-32FC-454A-A681-6FF5281A31E2"); }
        }
        public List<GenericBEFixedValue> FixedValues { get; set; }
        public override void Execute(IGenericBEOnBeforeInsertHandlerContext context)
        {
            if (FixedValues != null && FixedValues.Count > 0)
            {
                if (context.GenericBusinessEntity.FieldValues == null)
                    context.GenericBusinessEntity.FieldValues = new Dictionary<string, object>();

                foreach (var value in FixedValues)
                {
                    if (context.GenericBusinessEntity.FieldValues.TryGetValue(value.FieldName, out var fieldValue))
                    {
                        context.GenericBusinessEntity.FieldValues[value.FieldName] = value.Value;
                    }
                    else
                    {
                        context.GenericBusinessEntity.FieldValues.Add(value.FieldName, value.Value);
                    }
                }
            }
        }
    }
    public class GenericBEFixedValue
    {
        public string FieldName { get; set; }
        public object Value { get; set; }
    }
}
