using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Common.Business;

namespace Vanrise.GenericData.MainExtensions
{
    public class GenericFieldsEditorDefinitionSetting : VRGenericEditorDefinitionSetting
    {
        public override Guid ConfigId { get { return new Guid("F290120F-E657-439F-9897-3D1AB8C6E107"); } }

        public override string RuntimeEditor { get { return "vr-genericdata-genericfieldseditorsetting-runtime"; } }

        public override void ApplyTranslation(IGenericBETranslationContext context)
        {
            VRLocalizationManager vrLocalizationManager = new VRLocalizationManager();

            if (Fields != null)
            {
                foreach (var field in Fields)
                {
                    if (!String.IsNullOrEmpty(field.TextResourceKey))
                    {
                        field.FieldTitle = vrLocalizationManager.GetTranslatedTextResourceValue(field.TextResourceKey, field.FieldTitle, context.LanguageId);
                    }
                }
            }

        }
        public override List<GridColumnAttribute> GetGridColumnsAttributes(IGetGenericEditorColumnsInfoContext context)
        {
            List<GridColumnAttribute> columnsInfo = new List<GridColumnAttribute>();

            Dictionary<string, GenericEditorField> fields = new Dictionary<string, GenericEditorField>();
            DataRecordTypeManager dataRecordTypeManager = new DataRecordTypeManager();

            if (Fields != null && Fields.Count > 0)
            {
                foreach (var field in Fields)
                {
                    fields.Add(field.FieldPath, field);
                }
            }
            if (fields != null && fields.Count > 0)
            {
                var dataRecordAttributes = dataRecordTypeManager.GetDataRecordAttributes(context.DataRecordTypeId, fields.Keys.ToList());

                foreach (var att in dataRecordAttributes)
                {
                    var field = fields.GetRecord(att.Name);
                    var columnAttribute = att.Attribute;
                    columnAttribute.Field = field.FieldPath;
                    columnAttribute.HeaderText = field.FieldTitle;
                    columnsInfo.Add(att.Attribute);
                }
            }
            return columnsInfo;
        }
        public List<GenericEditorField> Fields { get; set; }
    }
}