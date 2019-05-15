using System;
using System.Collections.Generic;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions
{
    public class RowsContainerEditorDefinitionSetting : VRGenericEditorDefinitionSetting
    {
        public override Guid ConfigId { get { return new Guid("747F6659-2541-4008-A9CF-56A604E3F63C"); } }

        public override string RuntimeEditor { get { return "vr-genericdata-rowscontainereditor-runtime"; } }

        public List<VRRowContainer> RowContainers { get; set; }
    }

    public class VRRowContainer
    {
        public List<VRGenericEditorDefinitionSetting> RowSettings { get; set; }
    }
}