using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public enum AnalyticItemType { Dimension = 1, Measure = 2, Join = 3, Aggregate = 4, MeasureExternalSource = 5 }
    public class AnalyticItemConfig
    {
        public Guid AnalyticItemConfigId { get; set; }

        public Guid TableId { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }

        public AnalyticItemType ItemType { get; set; }
     
    }
    public class AnalyticItemConfig<T> : AnalyticItemConfig
    {
        public T Config { get; set; }
    }
}
