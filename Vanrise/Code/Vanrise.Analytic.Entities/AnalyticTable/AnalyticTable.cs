using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public class AnalyticTable
    {
        public Guid AnalyticTableId { get; set; }

        public string Name { get; set; }

        public AnalyticTableSettings Settings { get; set; }
    }
}
