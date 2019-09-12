using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.GenericData.Entities
{
    public abstract class ListRecordRuntimeViewType : FieldTypeRuntimeViewSettings
    {
        public abstract Guid ConfigId { get; }
        public abstract string RuntimeEditor { get; }
        public virtual void ApplyTranslation(IListRecordRuntimeViewTypeTranslationContext context) { }
        public virtual Dictionary<string, GridColumnAttribute> GetGenericEditorColumnsInfo(IListRecordRuntimeViewTypeColumnsInfoContext context)
        {
            return null;
        }

    }
    public interface IListRecordRuntimeViewTypeTranslationContext : IGenericBETranslationContext
    {
        Guid DataRecordTypeId { get; }
    }
    public interface IListRecordRuntimeViewTypeColumnsInfoContext
    {
        Guid DataRecordTypeId { get; }
    }
    public class ListRecordRuntimeViewTypeColumnsInfoContext: IListRecordRuntimeViewTypeColumnsInfoContext
    {
        public Guid DataRecordTypeId { get; set; }
    }
}
