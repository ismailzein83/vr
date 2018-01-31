using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public class RDBGroupBy
    {
        public List<RDBSelectColumn> Columns { get; set; }

        public List<RDBSelectColumn> AggregateColumns { get; set; }

        public BaseRDBCondition HavingCondition { get; set; }
    }
}
