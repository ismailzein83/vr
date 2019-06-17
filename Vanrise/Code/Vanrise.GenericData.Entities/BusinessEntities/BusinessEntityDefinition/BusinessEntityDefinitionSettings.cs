﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.GenericData.Entities
{
    public abstract class BusinessEntityDefinitionSettings
    {
        public abstract Guid ConfigId { get; }
        public virtual string IconPath { get; set; }
        public virtual string SelectorUIControl { get; set; }
        public virtual string GroupSelectorUIControl { get; set; }
        public virtual string ManagerFQTN { get; set; }
        public virtual string DefinitionEditor { get; set; }
        public virtual string ViewerEditor { get; set; }
        public virtual string IdType { get; set; }
        public virtual string SelectorFilterEditor { get; set; }
        public virtual string NullDisplayText { get; set; }
        public virtual string WorkFlowAddBEActivityEditor { get; set; }
        public virtual string WorkFlowUpdateBEActivityEditor { get; set; }
        public virtual string WorkFlowGetBEActivityEditor { get; set; }
        public virtual Dictionary<string, object> GetAdditionalSettings(IBEDefinitionSettingsGetAdditionalSettingsContext context)
        {
            return null;
        }
        public virtual Dictionary<string, DataRecordField> TryGetRecordTypeFields(IBEDefinitionSettingsTryGetRecordTypeFieldsContext context)
        {
            return null;
        }
        public virtual VRLoggableEntityBase GetLoggableEntity(IBusinessEntityDefinitionSettingsGetLoggableEntityContext context)
        {
            return null;
        }
		public virtual Object GetEditorRuntimeAdditionalData(IBEDefinitionSettingsGetEditorRuntimeAdditionalDataContext context)
		{
			return null;
		}

	}
    public interface IBEDefinitionSettingsGetAdditionalSettingsContext
    {

    }
    public interface IBEDefinitionSettingsTryGetRecordTypeFieldsContext
    {
        BusinessEntityDefinition BEDefinition { get; }
    }
    public interface IBusinessEntityDefinitionSettingsGetLoggableEntityContext
    {
        BusinessEntityDefinition BEDefinition { get; }
    }
	public interface IBEDefinitionSettingsGetEditorRuntimeAdditionalDataContext
	{
		BusinessEntityDefinition BEDefinition { get; }
	}
}
