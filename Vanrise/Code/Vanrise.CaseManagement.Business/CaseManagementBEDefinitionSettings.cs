using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.CaseManagement.Business
{
    public class CaseManagementBEDefinitionSettings : BusinessEntityDefinitionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("18FFFF1B-6680-4EA6-99A8-EEDC6378804A"); }
        }
        public Guid DataRecordTypeId { get; set; }
        public Guid DataRecordStorageId { get; set; }
        public CaseManagementGridDefinition GridDefinition { get; set; }
        public CaseManagementEditorDefinition EditorDefinition { get; set; }
    }
    public class CaseManagementEditorDefinition
    {
        public CaseManagementEditorDefinitionSetting Settings { get; set; }
    }
    public abstract class CaseManagementEditorDefinitionSetting
    {
        public abstract Guid ConfigId { get; }
        public virtual string RuntimeEditor { get; set; }
    }
    public class StaticCaseManagementEditorDefinitionSetting : CaseManagementEditorDefinitionSetting
    {
        public override Guid ConfigId
        {
            get { return new Guid("EC8B54D7-28AC-474F-B40A-D7AC02D89630"); }
        }
        public string DirectiveName { get; set; }
    }
    public class GenericCaseManagementEditorDefinitionSetting : CaseManagementEditorDefinitionSetting
    {
        public override Guid ConfigId
        {
            get { return new Guid("5BE30B11-8EE3-47EB-8269-41BDAFE077E1"); }
        }
        public List<GenericEditorRow> Rows { get; set; }
    }
    public class CaseManagementEditorRow
    {
        public List<CaseManagementEditorColumn> Columns { get; set; }
    }
    public class CaseManagementEditorColumn
    {
        public CaseManagementEditorColumnSettings Settings { get; set; }
    }
    public abstract class CaseManagementEditorColumnSettings
    {
        public abstract Guid ConfigId { get; }
    }
    public class CaseManagementTabsContainerEditorDefinitionSetting : CaseManagementEditorDefinitionSetting
    {
        public override Guid ConfigId
        {
            get { return new Guid("AD2D93E0-0C06-4EBE-B7A9-BF380C256EEE"); }
        }
        public List<CaseManagementTabContainer> TabContainers { get; set; }

    }
    public class CaseManagementTabContainer
    {
        public string TabTitle { get; set; }
        public bool  ShowTab { get; set; }
        public CaseManagementEditorDefinitionSetting TabSettings { get; set; }

    }
    public class CaseManagementGridDefinition
    {
        public List<CaseManagementGridColumn> GridColumns { get; set; }
    }
    public class CaseManagementGridColumn
    {
        public string FieldName { get; set; }
        public string FieldTitle { get; set; }
        public Vanrise.Entities.GridColumnSettings GridColumnSettings { get; set; }
    }
}
