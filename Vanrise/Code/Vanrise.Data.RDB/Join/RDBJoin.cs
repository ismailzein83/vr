using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public class RDBJoin
    {
        public IRDBTableQuerySource Table { get; set; }

        public RDBJoinType JoinType { get; set; }

        public BaseRDBCondition Condition { get; set; }
    }
}
