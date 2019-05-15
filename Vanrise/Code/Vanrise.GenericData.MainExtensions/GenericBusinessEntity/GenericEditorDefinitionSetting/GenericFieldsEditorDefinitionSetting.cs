using System;
using System.Collections.Generic;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions
{
    public class GenericFieldsEditorDefinitionSetting : VRGenericEditorDefinitionSetting
    {
        public override Guid ConfigId { get { return new Guid("F290120F-E657-439F-9897-3D1AB8C6E107"); } }

        public override string RuntimeEditor { get { return "vr-genericdata-genericfieldseditorsetting-runtime"; } }

        public List<GenericEditorField> Fields { get; set; }
    }
}