using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions
{
    public class GenericEditorDefinitionSetting : VRGenericEditorDefinitionSetting
    {
        public override Guid ConfigId { get { return new Guid("5BE30B11-8EE3-47EB-8269-41BDAFE077E1"); } }

        public override string RuntimeEditor { get { return "vr-genericdata-genericeditorsetting-runtime"; } }

        public List<GenericEditorRow> Rows { get; set; }

        public override void TryTranslate()
        {
            VRLocalizationManager vrLocalizationManager = new VRLocalizationManager();
            if (Rows != null)
            {
                foreach (var row in Rows)
                {
                    if (row.Fields != null)
                    {
                        foreach (var filed in row.Fields)
                        {
                            if (!String.IsNullOrEmpty(filed.TextResourceKey))
                            {
                                filed.FieldTitle = vrLocalizationManager.GetTranslatedTextResourceValue(filed.TextResourceKey, filed.FieldTitle);
                            }
                        }
                    }
                }
            }
        }

        public override List<GridColumnAttribute> GetGridColumnsAttributes(IGetGenericEditorColumnsInfoContext context)
        {
            List<GridColumnAttribute> columnsInfo = new List<GridColumnAttribute>();
            if (Rows != null && Rows.Count > 0)
            {
                Dictionary<string, GenericEditorField> fields = new Dictionary<string, GenericEditorField>();
                DataRecordTypeManager dataRecordTypeManager = new DataRecordTypeManager();
                foreach (var row in Rows)
                {
                    if (row.Fields != null && row.Fields.Count > 0)
                    {
                        foreach (var field in row.Fields)
                        {
                            fields.Add(field.FieldPath, field);
                        }
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
            }
            return columnsInfo;
        }
    }
}