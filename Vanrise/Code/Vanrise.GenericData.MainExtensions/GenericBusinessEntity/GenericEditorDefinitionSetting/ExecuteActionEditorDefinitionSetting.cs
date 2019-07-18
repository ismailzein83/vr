using System;
using System.Collections.Generic;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions
{
    public class ExecuteActionEditorDefinitionSetting : VRGenericEditorDefinitionSetting
    {
        public override Guid ConfigId { get { return new Guid("8A2D901C-3CB5-4E7E-A1B2-F77E15106835"); } }

        public override string RuntimeEditor { get { return "vr-genericdata-executeactioneditorsetting-runtime"; } }

        public CallOnLoadExecuteActionEditorDefinitionSection CallOnLoadSection { get; set; }

        public CallOnValueChangedExecuteActionEditorDefinitionSection CallOnValueChangedSection { get; set; }

        public CallOnButtonClickedExecuteActionEditorDefinitionSection CallOnButtonClickedSection { get; set; }

        public ExecuteActionTypeEditorDefinitionSetting ExecuteActionType { get; set; }
    }

    public class CallOnLoadExecuteActionEditorDefinitionSection
    {
        public bool CallOnLoad { get; set; }
    }

    public class CallOnValueChangedExecuteActionEditorDefinitionSection
    {
        public bool CallOnValueChanged { get; set; }

        public List<OnFieldNameChanged> OnFieldNameChanged { get; set; }
    }

    public class CallOnButtonClickedExecuteActionEditorDefinitionSection
    {
        public bool CallOnButtonClicked { get; set; }
        public VRButtonType VRButtonType { get; set; }
    }

    public class OnFieldNameChanged
    {
        public string FieldName { get; set; }
    }
}