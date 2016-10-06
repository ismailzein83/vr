using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Analytic.Entities
{
    public class AnalyticMeasureConfigInfo
    {
        public Guid AnalyticItemConfigId { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public GridColumnAttribute Attribute { get; set; }
        public GenericData.Entities.DataRecordFieldType FieldType { get; set; }
    }
}
