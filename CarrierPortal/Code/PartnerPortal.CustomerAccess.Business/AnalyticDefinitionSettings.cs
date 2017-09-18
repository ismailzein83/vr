using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace PartnerPortal.CustomerAccess.Business
{
    public class AnalyticDefinitionSettings : VRTileExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("06CD79C8-B1C0-4A33-A757-A36EBD96EA5B"); }
        }

        public override string RuntimeEditor
        {
            get { return "partnerportal-customeraccess-analytictileruntimesettings"; }
        }
        public List<AnalyticQuery> Queries { get; set; }
        public List<Guid> OrderedMeasureIds { get; set; }
        public Guid? ViewId { get; set; }
    }
    public class AnalyticQuery
    {
        public string Name { get; set; }
        public string UserDimensionName { get; set; }
        public Guid VRConnectionId { get; set; }
        public Guid TableId { get; set; }
        public List<MeasureItem> Measures { get; set; }
        public VRTimePeriod TimePeriod { get; set; }
    }
    public class MeasureItem
    {
        public Guid MeasureItemId { get; set; }
        public string MeasureName { get; set; }
        public string MeasureTitle { get; set; }
    }
}
