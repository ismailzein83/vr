using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions
{
    public enum HTTPMethodType { Get = 0, Post = 1 }
    public class CallRestAPIAction : ExecuteActionTypeEditorDefinitionSetting
    {
        public override Guid ConfigId { get { return new Guid("0EADD56F-4415-465C-BC26-C2C9B7960817"); } }

        public override string ActionRuntimeEditor { get { return "vr-genericdata-executeactioneditorsetting-runtime-callrestapiaction"; } }

        public string APIAction { get; set; }

        public HTTPMethodType HTTPMethodType { get; set; }

        public List<RestAPIInputItem> InputItems { get; set; }

        public List<RestAPIOutputItem> OutputItems { get; set; }
    }

    public class ConsoleDotLogAction : ExecuteActionTypeEditorDefinitionSetting
    {
        public override Guid ConfigId { get { return new Guid("253450EF-0F39-42AE-98D5-05B45894942D"); } }

        public override string ActionRuntimeEditor { get { return "vr-genericdata-executeactioneditorsetting-runtime-consoledotlogaction"; } }
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
