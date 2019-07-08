using System;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions.GenericBusinessEntity.GenericBECustomAction
{
    public class GenericBENewOrExistingCustomAction : GenericBECustomActionSettings
    {
        public override Guid ConfigId => new Guid("AE78708F-DFD0-49E7-8B60-909AC4D8ABD2");
        public override string ActionTypeName => "NewOrExistingCustomAction";
        public VRGenericEditorDefinitionSetting EditorDefinitionSetting { get; set; }

    }
}
