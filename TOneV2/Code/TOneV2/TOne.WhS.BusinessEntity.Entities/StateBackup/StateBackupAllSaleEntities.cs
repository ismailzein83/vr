using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class StateBackupAllSaleEntities : StateBackupType
    {
        public override Guid ConfigId
        {
            get { return new Guid("72d531f3-63b9-4a89-85f8-ab667e2759ad"); }
        }

        public int SellingNumberPlanId { get; set; }
    }
}
