using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions
{
    public class UpdateGenericBEBulkActionRuntimeSettings : GenericBEBulkActionRuntimeSettings
    {
        public Dictionary<string, object> FieldValues { get; set; }
        public override void Execute(IGenericBEBulkActionRuntimeSettingsContext context)
        {
            if (FieldValues != null && FieldValues.Count>0)
            {
                context.GenericBusinessEntity.ThrowIfNull("context.GenericBusinessEntity");
                context.GenericBusinessEntity.FieldValues.ThrowIfNull("context.GenericBusinessEntity.FieldValues");

                GenericBusinessEntityDefinitionManager genericBusinessEntityDefinitionManager = new GenericBusinessEntityDefinitionManager();
                GenericBusinessEntityManager genericBusinessEntityManager = new GenericBusinessEntityManager();

                DataRecordField dataRecordField = genericBusinessEntityDefinitionManager.GetIdFieldTypeForGenericBE(context.BEDefinitionId);
                dataRecordField.ThrowIfNull("dataRecordField BEDefinitionId:", context.BEDefinitionId);
                var genericBusinessEntityToUpdate = new GenericBusinessEntityToUpdate
                {
                    GenericBusinessEntityId = context.GenericBusinessEntity.FieldValues.GetRecord(dataRecordField.Name),
                    BusinessEntityDefinitionId = context.BEDefinitionId,
                    FieldValues = new Dictionary<string, object>()
                };
                foreach (var fieldValue in this.FieldValues)
                {
                    genericBusinessEntityToUpdate.FieldValues.Add(fieldValue.Key, fieldValue.Value);

                }

               context.ErrorMessage=genericBusinessEntityManager.UpdateGenericBusinessEntity(genericBusinessEntityToUpdate).Message;
            }

        }

    }
}
