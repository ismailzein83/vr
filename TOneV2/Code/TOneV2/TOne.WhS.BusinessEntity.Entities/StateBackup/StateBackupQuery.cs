using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class StateBackupQuery
    {
        public Guid? BackupTypeFilterConfigId { get; set; }

        public object BackupTypeFilterObject { get; set; }

        public DateTime? From { get; set; }

        public DateTime? To { get; set; }
    }
}
