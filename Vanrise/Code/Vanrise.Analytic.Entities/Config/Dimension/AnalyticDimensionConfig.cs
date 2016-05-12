using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public class AnalyticDimensionConfig
    {
        public string IdColumn { get; set; }

        public string NameColumn { get; set; }

        public List<string> JoinConfigNames { get; set; }

        public List<string> GroupByColumns { get; set; }
        public List<string> Parents { get; set; }
        public string RequiredParentDimension { get; set; }
      //  public bool IsRequiredFromParent { get; set; }
        public GenericData.Entities.DataRecordFieldType FieldType { get; set; }

        public string CurrencySQLColumnName { get; set; }
    }
}
