using System;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions
{
    public class ConditionalRuleContainerEditorDefinitionSetting : VRGenericEditorDefinitionSetting
    {
        public override Guid ConfigId { get { return new Guid("FA9A6147-205C-43F7-BA81-73EB530F245F"); } }

        public override string RuntimeEditor { get { return "vr-genericdata-conditionalrulecontainereditorsetting-runtime"; } }

        public VRGenericEditorDefinitionSetting EditorDefinitionSetting { get; set; }

        public GenericEditorConditionalRule GenericEditorConditionalRule { get; set; }
    }

    public abstract class GenericEditorConditionalRule
    {
        public abstract Guid ConfigId { get; }

        public abstract string ActionName { get; }
    }
}