using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions
{
    public class ConditionalRuleContainerSetting : VRGenericEditorDefinitionSetting
    {
        public override Guid ConfigId { get { return new Guid("FA9A6147-205C-43F7-BA81-73EB530F245F"); } }

        public VRGenericEditorDefinitionSetting EditorType { get; set; }

        public GenericEditorConditionalRule GenericEditorConditionalRule { get; set; }
    }

    public abstract class GenericEditorConditionalRule
    {
        public abstract Guid ConfigId { get; }
    }
}