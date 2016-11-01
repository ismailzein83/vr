using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class StateBackup
    {
        public Guid BackupTypeConfigId { get; set; }

        public long StateBackupId { get; set; }

        public StateBackupType Info { get; set; }

        public DateTime BackupDate { get; set; }

        public DateTime? RestoreDate { get; set; }

        public int BackupByUserId { get; set; }

        public int? RestoredByByUserId { get; set; }
    }
}
