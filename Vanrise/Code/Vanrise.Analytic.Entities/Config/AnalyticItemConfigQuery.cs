using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public class AnalyticItemConfigQuery
    {
        public Guid TableId { get; set; }
        public AnalyticItemType ItemType { get; set; }
    }
}
