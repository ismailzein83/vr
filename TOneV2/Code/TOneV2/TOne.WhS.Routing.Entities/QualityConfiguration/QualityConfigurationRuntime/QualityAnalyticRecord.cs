using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;

namespace TOne.WhS.Routing.Entities
{
    public class QualityAnalyticRecord
    {
        public AnalyticRecord AnalyticRecord { get; set; }

        public Decimal Quality { get; set; }
    }
}