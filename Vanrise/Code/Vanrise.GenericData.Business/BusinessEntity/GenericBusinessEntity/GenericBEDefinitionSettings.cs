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
        public override string GroupSelectorUIControl { get; set; }

        public bool HideAddButton { get; set; }
        public string SelectorSingularTitle { get; set; }
        public string SelectorPluralTitle { get; set; }
        public GenericBEDefinitionSecurity Security { get; set; }
       // public string FieldPath { get; set; }
        public ModalWidthEnum EditorSize { get; set; }
        public Guid DataRecordTypeId { get; set; }
        public Guid DataRecordStorageId { get; set; }
        public string TitleFieldName { get; set; }
        public List<GenericBEAction> GenericBEActions{get;set;}
        public GenericBEGridDefinition GridDefinition { get; set; }
        public GenericBEEditorDefinition EditorDefinition { get; set; }
        public GenericBEFilterDefinition FilterDefinition { get; set; }
        public GenericBEOnBeforeInsertHandler OnBeforeInsertHandler { get; set; }
        public GenericBEOnAfterSaveHandler OnAfterSaveHandler { get; set; }
        public GenericBEExtendedSettings ExtendedSettings { get; set; }
 
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
    }


    public abstract class GenericBEOnAfterSaveHandler
    {
        public abstract Guid ConfigId { get; }
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
    }
    public abstract class GenericBEActionSettings
    {
        public virtual string ActionTypeName { get; set; }
        public abstract Guid ConfigId { get; }
        public virtual bool DoesUserHaveAccess(IGenericBEActionDefinitionCheckAccessContext context)
        {
            return true;
        }
    }


    public interface IGenericBEActionDefinitionCheckAccessContext
    {
        Guid BusinessEntityDefinitionId { get; }

        int UserId { get; }
    }

    public class GenericBEActionDefinitionCheckAccessContext : IGenericBEActionDefinitionCheckAccessContext
    {
        public Guid BusinessEntityDefinitionId { get; set; }

        public int UserId { get; set; }
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
    }

}
