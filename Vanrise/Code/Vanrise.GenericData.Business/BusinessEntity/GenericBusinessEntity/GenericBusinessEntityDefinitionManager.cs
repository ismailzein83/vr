using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
namespace Vanrise.GenericData.Business
{
    public class GenericBusinessEntityDefinitionManager
    {
        public BusinessEntityDefinition GetGenericBEDefinition(Guid businessEntityDefinitionId)
        {
            BusinessEntityDefinitionManager manager = new BusinessEntityDefinitionManager();
            return manager.GetBusinessEntityDefinition(businessEntityDefinitionId);
        }
        public string GetGenericBEDefinitionName(Guid businessEntityDefinitionId)
        {
            var genericBEDefinition = GetGenericBEDefinition(businessEntityDefinitionId);
            genericBEDefinition.ThrowIfNull("genericBEDefinition", businessEntityDefinitionId);
            return genericBEDefinition.Name;
        }
        public GenericBEDefinitionSettings GetGenericBEDefinitionSettings(Guid businessEntityDefinitionId)
        {
            var genericBEDefinition = GetGenericBEDefinition(businessEntityDefinitionId);
            genericBEDefinition.ThrowIfNull("genericBEDefinition", businessEntityDefinitionId);
            genericBEDefinition.Settings.ThrowIfNull("genericBEDefinition.Settings");
            return genericBEDefinition.Settings.CastWithValidate<GenericBEDefinitionSettings>("genericBEDefinition.Settings");
        }
        public GenericBEDefinitionGridDefinition GetGenericBEDefinitionGridDefinition(Guid businessEntityDefinitionId)
        {
            var genericBEDefinitionSettings = GetGenericBEDefinitionSettings(businessEntityDefinitionId);
            genericBEDefinitionSettings.ThrowIfNull("genericBEDefinitionSettings", businessEntityDefinitionId);
            return genericBEDefinitionSettings.GridDefinition;
        }
        public List<GenericBEDefinitionGridColumnAttribute> GetGenericBEDefinitionGridColumnAttributes(Guid businessEntityDefinitionId)
        {
            List<GenericBEDefinitionGridColumnAttribute> gridColumns = new List<GenericBEDefinitionGridColumnAttribute>();
            var genericBEDefinitionSettings = GetGenericBEDefinitionSettings(businessEntityDefinitionId);
            genericBEDefinitionSettings.GridDefinition.ThrowIfNull("genericBEDefinitionSettings.GridDefinition");
            genericBEDefinitionSettings.GridDefinition.ColumnDefinitions.ThrowIfNull("genericBEDefinitionSettings.ColumnDefinitions");


            DataRecordTypeManager dataRecordTypeManager = new DataRecordTypeManager();
            var dataRecordTypeFields = dataRecordTypeManager.GetDataRecordTypeFields(genericBEDefinitionSettings.DataRecordTypeId);
            dataRecordTypeFields.ThrowIfNull("dataRecordTypeFields");

            foreach (var vrCaseGridColumn in genericBEDefinitionSettings.GridDefinition.ColumnDefinitions)
            {

                DataRecordField dataRecordTypeField = dataRecordTypeFields.FindRecord(x => x.Name == vrCaseGridColumn.FieldName);
                if (dataRecordTypeField == null)
                    continue;
                FieldTypeGetGridColumnAttributeContext context = new FieldTypeGetGridColumnAttributeContext();
                context.ValueFieldPath = "FieldValues." + vrCaseGridColumn.FieldName + ".Value";
                context.DescriptionFieldPath = "FieldValues." + vrCaseGridColumn.FieldName + ".Description";
                var vrCaseGridColumnAttribute = new GenericBEDefinitionGridColumnAttribute
                {
                    Attribute = dataRecordTypeField.Type.GetGridColumnAttribute(context),
                    Name = vrCaseGridColumn.FieldName
                };
                vrCaseGridColumnAttribute.Attribute.HeaderText = vrCaseGridColumn.FieldTitle;
                gridColumns.Add(vrCaseGridColumnAttribute);
            }
            return gridColumns;
        }
    }
}
