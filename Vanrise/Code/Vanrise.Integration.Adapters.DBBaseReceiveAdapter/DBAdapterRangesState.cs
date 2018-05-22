using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.Adapters.DBBaseReceiveAdapter
{
    /// <summary>
    /// this class is not used anymore for adapters. but it should NOT be DELETED because existing serialized states might exist in the customer database(s) before refactoring
    /// </summary>
    public class DbAdapterRangesState : BaseAdapterState
    {
        public List<DbAdapterRangeState> Ranges { get; set; }
    }

    public class DbAdapterRangeState
    {
        public Guid RangeId { get; set; }

        public Object RangeStart { get; set; }

        public Object RangeEnd { get; set; }

        public Object LastImportedId { get; set; }

        public int? LockedByProcessId { get; set; }
    }
}
