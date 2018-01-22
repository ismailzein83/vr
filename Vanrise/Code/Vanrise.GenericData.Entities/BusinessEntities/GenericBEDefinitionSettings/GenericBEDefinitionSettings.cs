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
      //  public GenericEditor EditorDesign { get; set; }
    //    public GenericManagement ManagementDesign { get; set; }


        public Guid DataRecordTypeId { get; set; }
        public Guid DataRecordStorageId { get; set; }
        public string TitleFieldName { get; set; }
        public List<GenericBEAction> GenericBEActions{get;set;}
        public GenericBEGridDefinition GridDefinition { get; set; }
        public GenericBEEditorDefinition EditorDefinition { get; set; }
        public GenericBEFilterDefinition FilterDefinition { get; set; }

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
