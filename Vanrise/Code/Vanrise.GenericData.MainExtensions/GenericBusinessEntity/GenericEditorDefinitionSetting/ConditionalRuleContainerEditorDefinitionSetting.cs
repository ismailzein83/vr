using System;
using System.Collections.Generic;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions
{
    public class ConditionalRuleContainerEditorDefinitionSetting : VRGenericEditorDefinitionSetting
    {
        public override Guid ConfigId { get { return new Guid("FA9A6147-205C-43F7-BA81-73EB530F245F"); } }

        public override string RuntimeEditor { get { return "vr-genericdata-conditionalrulecontainereditorsetting-runtime"; } }

        public VRGenericEditorDefinitionSetting EditorDefinitionSetting { get; set; }

        public override List<GridColumnAttribute> GetGridColumnsAttributes(IGetGenericEditorColumnsInfoContext context)
        {
            List<GridColumnAttribute> columnsAttributes = new List<GridColumnAttribute>();

            if (EditorDefinitionSetting != null)
            {
                var attributes = EditorDefinitionSetting.GetGridColumnsAttributes(new GetGenericEditorColumnsInfoContext
                {
                    DataRecordTypeId = context.DataRecordTypeId
                });

                if (attributes != null && attributes.Count > 0)
                    columnsAttributes.AddRange(attributes);
            }
            return columnsAttributes;
        }
        public GenericEditorConditionalRule GenericEditorConditionalRule { get; set; }
    }

    public abstract class GenericEditorConditionalRule
    {
        public abstract Guid ConfigId { get; }

        public abstract string ActionName { get; }
    }
}