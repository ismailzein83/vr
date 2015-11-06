using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public abstract class StatisticBatch<T> where T : StatisticItem
    {
        public abstract DateTime BatchKey { get; }

        public List<T> Items { get; set; }
    }
}
