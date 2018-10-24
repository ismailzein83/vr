using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Tools.Entities
{
    public class ColumnsInfoFilter
    {
        public Guid ConnectionId { get; set; }
        public string Table { get; set; }
        public string Schema { get; set; }
    }
}