using System;
using System.Collections.Generic;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions
{
    public class ConditionalRuleContainerEditorDefinitionSetting : VRGenericEditorDefinitionSetting
    {
        public override Guid ConfigId { get { return new Guid("FA9A6147-205C-43F7-BA81-73EB530F245F"); } }

        public override string RuntimeEditor { get { return "vr-genericdata-conditionalrulecontainereditorsetting-runtime"; } }

        public VRGenericEditorDefinitionSetting EditorDefinitionSetting { get; set; }

        public override void ApplyTranslation(IGenericEditorTranslationContext context)
        {
            if (EditorDefinitionSetting != null)
                EditorDefinitionSetting.ApplyTranslation(new GenericEditorTranslationContext(context.DataRecordTypeId, context.LanguageId));
        }

        public override Dictionary<string,GridColumnAttribute> GetGridColumnsAttributes(IGetGenericEditorColumnsInfoContext context)
        {
            if (EditorDefinitionSetting != null)
            {
                return EditorDefinitionSetting.GetGridColumnsAttributes(new GetGenericEditorColumnsInfoContext
                {
                    DataRecordTypeId = context.DataRecordTypeId
                });

            }
            return null;
        }
        public GenericEditorConditionalRule GenericEditorConditionalRule { get; set; }
    }

    public abstract class GenericEditorConditionalRule
    {
        public abstract Guid ConfigId { get; }

        public abstract string ActionName { get; }
    }
}