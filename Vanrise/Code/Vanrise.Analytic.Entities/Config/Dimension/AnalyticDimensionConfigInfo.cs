using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Analytic.Entities
{
    public class AnalyticDimensionConfigInfo
    {
        public Guid AnalyticItemConfigId { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public List<string> Parents { get; set; }
        public string RequiredParentDimension { get; set; }
        public GridColumnAttribute Attribute { get; set; }
    }
}
