using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using Vanrise.Security.Entities;
namespace Vanrise.GenericData.Business
{
    public enum GenericBEDefinitionType { RecordStorage = 0 , Remote = 1}
    public class GenericBEDefinitionSettings : BusinessEntityDefinitionSettings
    {
        public static Guid s_configId = new Guid("6F3FBD7B-275A-4D92-8E06-AD7F7B04C7D6");
        public override Guid ConfigId { get { return s_configId; } }

        public override string DefinitionEditor 
        { 
            get { return "vr-genericdata-genericbusinessentity-editor"; } 
        }
        public override string ViewerEditor
        {
            get { return "vr-genericdata-genericbusinessentity-runtimeeditor"; }
        }
        public override string IdType
        {
            get
            {
               return new DataRecordTypeManager().GetIdFieldRuntimeTypeAsString(DataRecordTypeId);
            }
        }
        public override string SelectorUIControl
        {
            get { return "vr-genericdata-genericbusinessentity-selector"; }
        }
        public override string ManagerFQTN
        {
            get { return "Vanrise.GenericData.Business.GenericBusinessEntityManager, Vanrise.GenericData.Business"; }
        }
        public override Dictionary<string, DataRecordField> TryGetRecordTypeFields(IBEDefinitionSettingsTryGetRecordTypeFieldsContext context)
        {
            return new DataRecordTypeManager().GetDataRecordTypeFields(this.DataRecordTypeId);
        }
		public override Object GetEditorRuntimeAdditionalData(IBEDefinitionSettingsGetEditorRuntimeAdditionalDataContext context)
		{
			GenericBEDefinitionSettings settings = context.BEDefinition.Settings.CastWithValidate<GenericBEDefinitionSettings>("GenericBEDefinitionSettings");
			Dictionary<string, DataRecordField> dataRecordFields = new DataRecordTypeManager().GetDataRecordTypeFields(settings.DataRecordTypeId);

			List<GenericBEAddedValue> genericBEAddedValues = new List<GenericBEAddedValue>();
			if (settings.UploadFields != null)
				foreach (var uploadField in settings.UploadFields)
				{
					var fieldValue = dataRecordFields.GetRecord(uploadField.FieldName);
					if (fieldValue != null)
					{
						genericBEAddedValues.Add(new GenericBEAddedValue
						{

							FieldName = uploadField.FieldName,
							IsRequired = uploadField.IsRequired,
							FieldTitle = fieldValue.Title
						});
					}
				}
			return genericBEAddedValues;
		}
		public GenericBEDefinitionType GenericBEType { get; set; }
        public Guid? VRConnectionId { get; set; }
        public Guid? RemoteGenericBEDefinitionId { get; set; }


        public override string GroupSelectorUIControl { get; set; }

        public bool HideAddButton { get; set; }
        public string SelectorSingularTitle { get; set; }
        public string SelectorPluralTitle { get; set; }
        public GenericBEDefinitionSecurity Security { get; set; }
       // public string FieldPath { get; set; }
        public ModalWidthEnum EditorSize { get; set; }
        public Guid DataRecordTypeId { get; set; }
        public Guid? DataRecordStorageId { get; set; }
        public string TitleFieldName { get; set; }
        public List<GenericBEAction> GenericBEActions{get;set;}
        public GenericBEGridDefinition GridDefinition { get; set; }
        public GenericBEEditorDefinition EditorDefinition { get; set; }
        public GenericBEFilterDefinition FilterDefinition { get; set; }
        public GenericBEOnBeforeInsertHandler OnBeforeInsertHandler { get; set; }
        public GenericBEOnAfterSaveHandler OnAfterSaveHandler { get; set; }
        public GenericBEOnBeforeGetFilteredHandler OnBeforeGetFilteredHandler { get; set; }
        public GenericBEExtendedSettings ExtendedSettings { get; set; }
        public List<GenericBEBulkAction> GenericBEBulkActions { get; set; }

        public bool ShowUpload { get; set; }
        public List<GenericBEUploadField> UploadFields { get; set; }

    }

    public class GenericBEUploadField
    {
        public string FieldName { get; set; }
		public bool IsRequired { get; set; }

    }

    public class GenericBEBulkAction
    {
        public Guid GenericBEBulkActionId { get; set; }
        public string Title { get; set; }
        public GenericBEBulkActionSettings Settings { get; set; }
    }
    public abstract class GenericBEBulkActionSettings
    {
        public abstract Guid ConfigId { get; }
        public abstract string RuntimeEditor { get; }
    }
    public abstract class GenericBEExtendedSettings
    {
        public abstract Guid ConfigId { get; }
        public virtual Object GetInfoByType(IGenericBEExtendedSettingsContext context)
        {
            return null;
        }
    }
    public interface IGenericBEExtendedSettingsContext
    {
        GenericBusinessEntity GenericBusinessEntity { get; }
        GenericBEDefinitionSettings DefinitionSettings { get;  }
        string InfoType { get; }
    }

    public abstract class GenericBEOnBeforeGetFilteredHandler
    {
        public abstract Guid ConfigId { get; }
        public abstract void PrepareQuery(IGenericBEOnBeforeGetFilteredHandlerPrepareQueryContext context);
        public abstract void onBeforeAdd(IGenericBEOnBeforeAddHandlerContext context);
        public abstract void onBeforeUpdate(IGenericBEOnBeforeUpdateHandlerContext context);

    }
    public interface IGenericBEBulkActionRuntimeSettingsContext
    {
        GenericBusinessEntity GenericBusinessEntity { get; set; }
        Guid BEDefinitionId { get; set; }
        string ErrorMessage { get; set; }
        bool IsErrorOccured { get; set; }
        GenericBEBulkActionSettings DefinitionSettings { get; set; }
    }
    public class GenericBEBulkActionRuntimeSettingsContext : IGenericBEBulkActionRuntimeSettingsContext
    {
        public GenericBusinessEntity GenericBusinessEntity { get; set; }
        public Guid BEDefinitionId { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsErrorOccured { get; set; }
        public GenericBEBulkActionSettings DefinitionSettings { get; set; }

    }

    public abstract class GenericBEBulkActionRuntimeSettings
    {
        public abstract void Execute(IGenericBEBulkActionRuntimeSettingsContext context);
    }
    public interface IGenericBEOnBeforeAddHandlerContext
    {
        GenericBusinessEntityToAdd GenericBusinessEntityToAdd { get; }
        Guid? VRConnectionId { get; }
    }

    public interface IGenericBEOnBeforeUpdateHandlerContext
    {
        GenericBusinessEntityToUpdate GenericBusinessEntityToUpdate { get; }
        Guid? VRConnectionId { get;}
    }

    public interface IGenericBEOnBeforeGetFilteredHandlerPrepareQueryContext
    {
        Guid? VRConnectionId { get; }
        GenericBusinessEntityQuery Query { get; set; }
    }

    public class GenericBEOnBeforeGetFilteredHandlerPrepareQueryContext : IGenericBEOnBeforeGetFilteredHandlerPrepareQueryContext
    {
        public Guid? VRConnectionId { get; set; }
        public GenericBusinessEntityQuery Query { get; set; }
    }

    public class GenericBEOnBeforeAddHandlerContext : IGenericBEOnBeforeAddHandlerContext
    {
        public GenericBusinessEntityToAdd GenericBusinessEntityToAdd { get; set; }
        public Guid? VRConnectionId { get; set; }
    }

    public class GenericBEOnBeforeUpdateHandlerContext : IGenericBEOnBeforeUpdateHandlerContext
    {
        public GenericBusinessEntityToUpdate GenericBusinessEntityToUpdate { get; set; }
        public Guid? VRConnectionId { get; set; }
    }
    public abstract class GenericBEOnBeforeInsertHandler
    {
        public abstract Guid ConfigId { get;  }
        public virtual object TryGetInfoByType(IGenericBEOnBeforeInsertHandlerInfoByTypeContext context)
        {
            return null;
        }
        public abstract void Execute(IGenericBEOnBeforeInsertHandlerContext context);
    }
    public interface IGenericBEOnBeforeInsertHandlerInfoByTypeContext
    {
        string InfoType { get; }
        GenericBEDefinitionSettings DefinitionSettings { get; }
    }
    public enum HandlerOperationType { Add = 0,Update = 1}
    public interface IGenericBEOnBeforeInsertHandlerContext
    {
        HandlerOperationType OperationType { get;}
        GenericBusinessEntity GenericBusinessEntity { get; set; }
        GenericBEDefinitionSettings DefinitionSettings { get; }
        GenericBusinessEntity OldGenericBusinessEntity { get; }
        Guid BusinessEntityDefinitionId { get; }
        OutputResult OutputResult { get; }
    }


    public abstract class GenericBEOnAfterSaveHandler
    {
        public abstract Guid ConfigId { get; }
        public List<HandlerOperationType> HandlerOperationTypes { get; set; }
        public virtual object TryGetInfoByType(IGenericBEOnAfterSaveHandlerInfoByTypeContext context)
        {
            return null;
        }
        public abstract void Execute(IGenericBEOnAfterSaveHandlerContext context);
    }
    public interface IGenericBEOnAfterSaveHandlerInfoByTypeContext
    {
        string InfoType { get; }
        GenericBEDefinitionSettings DefinitionSettings { get; }
    }
    public interface IGenericBEOnAfterSaveHandlerContext
    {
        HandlerOperationType OperationType { get; }
        GenericBusinessEntity NewEntity { get;  }
        GenericBusinessEntity OldEntity { get;  }
        GenericBEDefinitionSettings DefinitionSettings { get; }
        Guid BusinessEntityDefinitionId { get; }
    }
    public class GenericBEOnAfterSaveHandlerContext : IGenericBEOnAfterSaveHandlerContext
    {
        public GenericBusinessEntity NewEntity { get; set; }
        public GenericBusinessEntity OldEntity { get; set; }
        public GenericBEDefinitionSettings DefinitionSettings { get; set; }
        public Guid BusinessEntityDefinitionId { get; set; }

        public HandlerOperationType OperationType { get; set; }
    }

    public class GenericBEViewDefinition
    {
        public Guid GenericBEViewDefinitionId { get; set; }
        public string Name { get; set; }
        public GenericBEViewDefinitionSettings Settings { get; set; }
    }
    public abstract class GenericBEViewDefinitionSettings
    {
        public abstract Guid ConfigId { get; }
        public abstract string RuntimeDirective { get; }

        public virtual bool DoesUserHaveAccess(IGenericBEViewDefinitionCheckAccessContext context)
        {
            return true;
        }
    }

    public class GenericBEFilterDefinition
    {
        public GenericBEFilterDefinitionSettings Settings { get; set; }
    }
    public abstract class GenericBEFilterDefinitionSettings
    {
        public abstract Guid ConfigId { get;  }
        public abstract string RuntimeEditor { get; }
    }
   
    public class GenericBEEditorDefinition
    {
        public VRGenericEditorDefinitionSetting Settings { get; set; }
    }
  
    public class GenericBEGridDefinition
    {
        public List<GenericBEGridColumn> ColumnDefinitions { get; set; }
        public List<GenericBEGridAction> GenericBEGridActions { get; set; }
        public List<GenericBEViewDefinition> GenericBEGridViews { get; set; }

    }
    public class GenericBEGridColumn
    {
        public string FieldName { get; set; }
        public string FieldTitle { get; set; }
        public GridColumnSettings GridColumnSettings { get; set; }
    }
    public class GenericBEGridAction
    {
        public Guid GenericBEGridActionId { get; set; }
        public Guid GenericBEActionId { get; set; }
        public string Title { get; set; }
        public bool ReloadGridItem { get; set; }
        public GenericBEActionFilterCondition FilterCondition { get; set; }

    }
    public abstract class GenericBEActionFilterCondition
    {
        public abstract Guid ConfigId { get; }
        public abstract bool IsFilterMatch(IGenericBEActionFilterConditionContext context);
    }
    public interface IGenericBEActionFilterConditionContext
    {
        GenericBEDefinitionSettings DefinitionSettings { get; }
        GenericBusinessEntity Entity { get; }
    }
    public class GenericBEActionFilterConditionContext : IGenericBEActionFilterConditionContext
    {
        public GenericBusinessEntity Entity { get; set; }

        public GenericBEDefinitionSettings DefinitionSettings { get; set; }
    }

    public abstract class GenericBECondition
    {
        public abstract Guid ConfigId { get; }
        public abstract bool IsMatch(IGenericBEConditionContext context);
    }
    public interface IGenericBEConditionContext
    {
        GenericBusinessEntity Entity { get;  }
        GenericBEDefinitionSettings DefinitionSettings { get; }
    }
    public class GenericBEConditionContext : IGenericBEConditionContext
    {
        public GenericBusinessEntity Entity { get; set; }
        public GenericBEDefinitionSettings DefinitionSettings { get; set; }
    }

    public abstract class GenericBESaveCondition
    {
        public abstract Guid ConfigId { get; }
        public abstract bool IsMatch(IGenericBESaveConditionContext context);
    }
    public interface IGenericBESaveConditionContext
    {
        GenericBusinessEntity NewEntity { get; }
        GenericBusinessEntity OldEntity { get; }
        GenericBEDefinitionSettings DefinitionSettings { get; }
    }
    public class GenericBESaveConditionContext : IGenericBESaveConditionContext
    {
        public GenericBusinessEntity NewEntity { get; set; }
        public GenericBusinessEntity OldEntity { get; set; }
        public GenericBEDefinitionSettings DefinitionSettings { get; set; }
    }


    public class GenericBEAction
    {
        public Guid GenericBEActionId { get; set; }
        public string Name { get; set; }
		public GenericBEActionSettings Settings { get; set; }
		public RequiredPermissionSettings RequiredPermission { get; set; }

	}
	public abstract class GenericBEActionSettings
    {
        public virtual string ActionTypeName { get; set; }
		public abstract string ActionKind { get; }
        public abstract Guid ConfigId { get; }
        public virtual bool DoesUserHaveAccess(IGenericBEActionDefinitionCheckAccessContext context)
        {
			return ContextFactory.GetContext().IsAllowed(context.GenericBEAction.RequiredPermission, context.UserId);
		}
	}

    public interface IGenericBEActionDefinitionCheckAccessContext
    {
        Guid BusinessEntityDefinitionId { get; }

        int UserId { get; }

		GenericBEAction GenericBEAction { get; set; }
	}

    public class GenericBEActionDefinitionCheckAccessContext : IGenericBEActionDefinitionCheckAccessContext
    {
        public Guid BusinessEntityDefinitionId { get; set; }

        public int UserId { get; set; }

		public GenericBEAction GenericBEAction { get; set; }

	}

	public interface IGenericBEViewDefinitionCheckAccessContext
    {
        Guid BusinessEntityDefinitionId { get; }

        int UserId { get; }
    }

    public class GenericBEViewDefinitionCheckAccessContext : IGenericBEViewDefinitionCheckAccessContext
    {
        public Guid BusinessEntityDefinitionId { get; set; }

        public int UserId { get; set; }
    }


    public class GenericBEDefinitionSecurity
    {
        public RequiredPermissionSettings ViewRequiredPermission { get; set; }
       
        public RequiredPermissionSettings AddRequiredPermission { get; set; }
       
        public RequiredPermissionSettings EditRequiredPermission { get; set; }

        public RequiredPermissionSettings DeleteRequiredPermission { get; set; }
    }
	public class GenericBEAddedValue
	{
		public string FieldName { get; set; }
		public bool IsRequired { get; set; }
		public string FieldTitle { get; set; }

	}
}
