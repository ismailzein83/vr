using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions.DataRecordFields
{
    public class FieldListDataRecordType : DataRecordFieldType
    {
        public override Guid ConfigId => new Guid("8C0574CD-8862-4F29-828A-CE398635F2F7");

        public override string RuntimeEditor => "vr-genericdata-fieldtype-datarecordtypelist-runtimeeditor";
        public Guid DataRecordTypeId { get; set; }
        public ListRecordRuntimeViewType RuntimeViewType { get; set; }
        public override RecordFilter ConvertToRecordFilter(IDataRecordFieldTypeConvertToRecordFilterContext context)
        {
            throw new NotImplementedException();
        }

        public override RDBDataRecordFieldAttribute GetDefaultRDBFieldAttribute(IDataRecordFieldTypeDefaultRDBFieldAttributeContext context)
        {
            return new RDBDataRecordFieldAttribute
            {
                RdbDataType = RDBDataType.NVarchar
            };
        }

        public override string GetDescription(object value)
        {
            throw new NotImplementedException();
        }

        public override GridColumnAttribute GetGridColumnAttribute(FieldTypeGetGridColumnAttributeContext context)
        {
            throw new NotImplementedException();
        }

        Type _nonNullableRuntimeType;
        public override Type GetNonNullableRuntimeType()
        {
            if (_nonNullableRuntimeType == null)
            {
                lock (this)
                {
                    if (_nonNullableRuntimeType == null)
                    {
                        Type dataRecordType = new DataRecordTypeManager().GetDataRecordRuntimeType(DataRecordTypeId);
                        _nonNullableRuntimeType = typeof(List<>).MakeGenericType(dataRecordType);
                    }
                }
            }
            return _nonNullableRuntimeType;
        }

        public override Type GetRuntimeType()
        {
            return GetNonNullableRuntimeType();
        }

        public override string GetRuntimeTypeDescription()
        {
            return Vanrise.Common.CSharpCompiler.TypeToString(GetRuntimeType());
        }

        public override bool IsMatched(object fieldValue, object filterValue)
        {
            throw new NotImplementedException();
        }

        public override bool IsMatched(object fieldValue, RecordFilter recordFilter)
        {
            throw new NotImplementedException();
        }
    }

    public abstract class ListRecordRuntimeViewType
    {
        public abstract Guid ConfigId { get; }
        public abstract string RuntimeEditor { get; }
    }

    public class GridViewListRecordRuntimeViewType : ListRecordRuntimeViewType
    {
        public override Guid ConfigId =>  new Guid("661E02F1-7A44-4D56-A73F-7912EF3017B1");
        public override string RuntimeEditor => "";
    }
    public class GridEditorViewListRecordRuntimeViewType : ListRecordRuntimeViewType
    {
        public override Guid ConfigId => new Guid("03925E9D-6A0F-4D4F-A4A4-36F5757D71EB");
        public override string RuntimeEditor => "";
    }
    public class FieldViewListRecordRuntimeViewType : ListRecordRuntimeViewType
    {
        public override Guid ConfigId => new Guid("6A8E0D5E-318C-4C6E-A99B-991EAE562B7C");
        public override string RuntimeEditor => "";
        public DataRecordField RecordField { get; set; }
    }
}
