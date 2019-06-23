using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.NIM.Entities
{
    public class FDBPort
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public long Area { get; set; }

        public long Site { get; set; }

        public long FDB { get; set; }

        public Guid Status { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedTime { get; set; }

        public int LastModifiedBy { get; set; }

        public DateTime LastModifiedTime { get; set; }
    }
}