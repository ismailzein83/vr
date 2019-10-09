using System;

namespace Vanrise.GenericData.Entities
{
    public abstract class FieldCustomObjectTypeSettings
    {
        public abstract Guid ConfigId { get; }

        public virtual string SelectorUIControl { get { return null; } }

        public virtual string RuleFilterSelectorUIControl { get { return this.SelectorUIControl; } }

        public abstract Type GetNonNullableRuntimeType();

        public abstract string GetRuntimeTypeDescription();

        public abstract string GetDescription(IFieldCustomObjectTypeSettingsContext context);

        public abstract dynamic ParseNonNullValueToFieldType(Object originalValue);

        public abstract bool AreEqual(Object newValue, Object oldValue);

        public virtual bool IsMatched(IFieldCustomObjectTypeSettingsContext context)
        {
            return true;
        }

        public virtual void GetValueByDescription(IFieldCustomObjectTypeSettingsGetValueByDescriptionContext context)
        {
            throw new NotImplementedException();
        }
    }

    public interface IFieldCustomObjectTypeSettingsContext
    {
        object FieldValue { get; }
        RecordFilter RecordFilter { get; }
    }
}