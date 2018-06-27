using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class DBReplication
    {
        public Guid DBReplicationId { get; set; }

        public string Name { get; set; }

        public Guid DBReplicationDefinitionId { get; set; }

        public DBReplicationSettings Settings { get; set; }
    }
}