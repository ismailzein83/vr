using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.GenericData.Entities
{
    public class GenericBEDefinitionSettings : BusinessEntityDefinitionSettings
    {
        public static Guid s_configId = new Guid("6F3FBD7B-275A-4D92-8E06-AD7F7B04C7D6");
        public override Guid ConfigId { get { return s_configId; } }

        public override string DefinitionEditor 
        { 
            get { return "vr-genericdata-genericbusinessentity-editor"; } 
        }
        public override string IdType
        {
            get { return "System.Int64"; }
        }
        public override string SelectorUIControl
        {
            get { return "vr-genericdata-genericbusinessentity-selector"; }
        }
        public override string ManagerFQTN
        {
            get { return "Vanrise.GenericData.Business.GenericBusinessEntityManager, Vanrise.GenericData.Business"; }
        }
        public override string GroupSelectorUIControl { get; set; }


    //    public GenericRuleDefinitionSecurity Security { get; set; }
       // public string FieldPath { get; set; }

        public Guid DataRecordTypeId { get; set; }
        public Guid DataRecordStorageId { get; set; }
        public string TitleFieldName { get; set; }
        public List<GenericBEAction> GenericBEActions{get;set;}
        public GenericBEGridDefinition GridDefinition { get; set; }
        public GenericBEEditorDefinition EditorDefinition { get; set; }
        public GenericBEFilterDefinition FilterDefinition { get; set; }
        public GenericBEOnBeforeInsertHandler OnBeforeInsertHandler { get; set; }
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
    public interface IGenericBEOnBeforeInsertHandlerContext
    {
        GenericBusinessEntity GenericBusinessEntity { get; set; }
        GenericBEDefinitionSettings DefinitionSettings { get; }
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
        public GenericBEGridActionFilterCondition FilterCondition { get; set; }

    }
    public abstract class GenericBEGridActionFilterCondition
    {
        public abstract Guid ConfigId { get; }
        public abstract bool IsFilterMatch(IGenericBEGridActionFilterConditionContext context);
    }
    public interface IGenericBEGridActionFilterConditionContext
    {
        GenericBusinessEntity Entity { get;  }
        GenericBEDefinitionSettings DefinitionSettings { get; }
    }
    public class GenericBEGridActionFilterConditionContext : IGenericBEGridActionFilterConditionContext
    {
        public GenericBusinessEntity Entity { get; set; }
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
    }
}
