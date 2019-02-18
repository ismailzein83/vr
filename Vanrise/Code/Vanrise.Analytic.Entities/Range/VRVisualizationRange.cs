using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Analytic.Entities
{
    public class VRVisualizationRange
    {
        public decimal? From { get; set; }
        public decimal? To { get; set; }
        public string Name { get; set; }
        public StyleFormatingSettings Color { get; set; }
    }
}
