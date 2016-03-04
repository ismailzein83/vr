using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class GenericBEDefinitionSettings:BusinessEntityDefinitionSettings
    {
        [JsonIgnore]
        public override string GroupSelectorUIControl { get; set; }
        [JsonIgnore]
        public override string DefinitionEditor { get { return "/Client/Modules/VR_GenericData/Views/GenericBusinessEntity/GenericBEEditor.html"; } }
        [JsonIgnore]
        public override string SelectorUIControl
        {
            get;
            set;
        }
        [JsonIgnore]
        public override string ManagerFQTN
        {
            get { return "Vanrise.GenericData.Entities.GenericBEDefinitionSettings, Vanrise.GenericData.Entities"; }
        }
        [JsonIgnore]
        public override string IdType
        {
            get { return "System.Int64";}
        }

        public int DataRecordTypeId { get; set; }
        public GenericEditor EditorDesign { get; set; }
        public GenericManagement ManagementDesign { get; set; }
        
    }
}
