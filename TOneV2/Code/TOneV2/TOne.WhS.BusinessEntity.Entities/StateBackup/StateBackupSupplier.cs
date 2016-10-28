using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class StateBackupSupplier : StateBackupType
    {
        public override Guid ConfigId
        {
            get { return new Guid("9f57b618-0890-4cf6-b2e9-ce2bb6d925a7"); }
        }

        public int SupplierId { get; set; }
    }
}
