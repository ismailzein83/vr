using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.Adapters.DBReceiveAdapter.Arguments
{
    public class DBAdapterRangesState : BaseAdapterState
    {
        public List<DBAdapterRangeState> Ranges { get; set; }
    }

    public class DBAdapterRangeState
    {
        public Guid RangeId { get; set; }

        public Object RangeStart { get; set; }

        public Object RangeEnd { get; set; }

        public Object LastImportedId { get; set; }
    }
}
