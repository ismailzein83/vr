using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.CaseManagement.Business
{
    public class VRCaseBEDefinitionSettings : BusinessEntityDefinitionSettings
    {
        public static Guid s_configId = new Guid("18FFFF1B-6680-4EA6-99A8-EEDC6378804A");
        public override Guid ConfigId
        {
            get { return s_configId; }
        }
        public Guid DataRecordTypeId { get; set; }
        public Guid DataRecordStorageId { get; set; }
        public VRCaseGridDefinition GridDefinition { get; set; }
        public VRCaseEditorDefinition EditorDefinition { get; set; }
        public override string IdType { get { return "System.Int64"; } }
        public override string ManagerFQTN
        {
            get { return "Vanrise.CaseManagement.Business.VRCaseDefinitionManager, Vanrise.CaseManagement.Business"; }
        }
    }
    public class VRCaseEditorDefinition
    {
        public VRGenericEditorDefinitionSetting Settings { get; set; }
    }
   
  
    public class VRCaseGridDefinition
    {
        public List<VRCaseGridColumn> GridColumns { get; set; }
    }
    public class VRCaseGridColumn
    {
        public string FieldName { get; set; }
        public string FieldTitle { get; set; }
        public Vanrise.Entities.GridColumnSettings GridColumnSettings { get; set; }
    }
}
