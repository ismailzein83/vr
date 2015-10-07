using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BI.Entities
{
   public class ChartDirectiveSetting:BIWidgetSetting
    {
        public string OperationType { get; set; }
        public List<String> EntityType { get; set; }
        public List<string> MeasureTypes { get; set; }
        public string TopMeasure { get; set; }
        public string DefinitionType { get; set; }
        public string Title { get; set; }
        public bool IsPieChart { get; set; }
        public int TopRecords { get; set; }
        public override List<string> GetMeasures()
        {
            return MeasureTypes;
        }
    }
}
