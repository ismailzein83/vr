﻿using System;
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
        public abstract dynamic ParseNonNullValueToFieldType(Object originalValue);
        public abstract string GetRuntimeTypeDescription();
        public virtual string SelectorUIControl
        {
            get
            {
                return null;
            }
        }

        public virtual string RuleFilterSelectorUIControl
        {
            get
            {
                return this.SelectorUIControl;
            }
        }

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
        RecordFilter RecordFilter { get;  }
    }
}