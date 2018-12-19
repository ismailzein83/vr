using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions
{
    public class UpdateGenericBEBulkAction : GenericBEBulkActionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("67B17B65-A7D9-4D0E-9788-A2CAE08C95B5"); }
        }
        public override string RuntimeEditor { get { return "vr-genericdata-genericbe-bulkactionsettings-update"; } }

        public List<UpdateGenericBEField> GenericBEFields { get; set; }
    }
    public class UpdateGenericBEField
    {
        public string FieldName { get; set; }
        public bool IsRequired { get; set; }
        public object DefaultValue { get; set; }
        public UpdateGenericBEFieldState FieldState { get; set; }
    }
    public enum UpdateGenericBEFieldState { ReadOnly = 1, Editable = 2, NotVisible = 3 }
}
