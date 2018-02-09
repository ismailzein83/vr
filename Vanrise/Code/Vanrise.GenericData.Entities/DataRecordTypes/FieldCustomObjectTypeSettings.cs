using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public abstract class FieldCustomObjectTypeSettings
    {
        public abstract Guid ConfigId { get; }
        public abstract string GetDescription(IFieldCustomObjectTypeSettingsContext context);
        public abstract Type GetNonNullableRuntimeType();
        public abstract bool AreEqual(Object newValue, Object oldValue);
    }
    public interface IFieldCustomObjectTypeSettingsContext
    {
        object FieldValue { get; }
    }
}
