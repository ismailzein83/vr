using System;
using System.Collections.Generic;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions
{
    public enum HTTPMethodType { Get = 0, Post = 1 }
    public class CallRestAPIEditorDefinitionSetting : VRGenericEditorDefinitionSetting
    {
        public override Guid ConfigId { get { return new Guid("8D9FB17A-9DF2-4A59-95B1-2F3402A1EAD3"); } }

        public override string RuntimeEditor { get { return "vr-genericdata-callrestapieditorsetting-runtime"; } }

        public VRButtonType VRButtonType { get; set; }

        public string APIAction { get; set; }

        public HTTPMethodType HTTPMethodType { get; set; }

        public List<RestAPIInputItem> InputItems { get; set; }

        public List<RestAPIOutputItem> OutputItems { get; set; }
    }

    public abstract class BaseRestAPIItem
    {
        public string FieldName { get; set; }

        public string PropertyName { get; set; }
    }

    public class RestAPIInputItem : BaseRestAPIItem
    {
        public bool IsRequired { get; set; }
    }

    public class RestAPIOutputItem : BaseRestAPIItem
    {
    }
}