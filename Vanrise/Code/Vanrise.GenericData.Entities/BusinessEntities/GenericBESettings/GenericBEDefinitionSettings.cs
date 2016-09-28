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
        public override string GroupSelectorUIControl { get; set; }
        public override string DefinitionEditor { get { return "/Client/Modules/VR_GenericData/Views/GenericBusinessEntity/Definition/GenericBEEditorDefintion.html"; } }
        public override string SelectorUIControl
        {
            get { return "vr-genericdata-genericbusinessentity-selector"; }
        }
        public override string ManagerFQTN
        {
            get { return "Vanrise.GenericData.Business.GenericBusinessEntityManager, Vanrise.GenericData.Business"; }
        }
        public override string IdType
        {
            get { return "System.Int64";}
        }

        public Guid DataRecordTypeId { get; set; }
        public string FieldPath { get; set; }
        public GenericEditor EditorDesign { get; set; }
        public GenericManagement ManagementDesign { get; set; }
        
    }
}
