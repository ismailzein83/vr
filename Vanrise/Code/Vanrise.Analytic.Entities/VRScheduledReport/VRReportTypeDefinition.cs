using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public class VRReportTypeDefinition : Vanrise.Entities.VRComponentType<VRReportTypeDefinitionSettings>
    {
    }
    public class VRReportTypeDefinitionSettings: Vanrise.Entities.VRComponentTypeSettings
    {
        public override Guid VRComponentTypeConfigId
        {
            get { return new Guid("D5A05B29-0329-4E28-8010-D55C01F1BFB3"); }
        }
        public Guid DataStorageId { get; set; }
        public List<VRReportTypeFilterField> FilterFields { get; set; }
        public List<VRReportTypeAttachement> Attachements { get; set; }
    }
    public class VRReportTypeFilterField
    {
        public string FieldName { get; set; }
        public bool IsRequired { get; set; }
    }
    public class VRReportTypeAttachement
    {
        public Guid VRReportTypeAttachementId { get; set; }
        public string Name { get; set; }
        public List<VRReportTypeAttachementField> Fields { get; set; }
        public VRAutomatedReportFileGenerator Attachement { get; set; }
    }
    public class VRReportTypeAttachementField
    {
        public string FieldName { get; set; }
    }

}
