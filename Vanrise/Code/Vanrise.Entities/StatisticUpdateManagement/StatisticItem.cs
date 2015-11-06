using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public abstract class StatisticItem
    {
        public long StatisticItemId { get; set; }

        public abstract string ItemKey { get; }
    }
}
