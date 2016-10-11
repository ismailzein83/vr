using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.DBSync.Entities
{
    public class RuleMigrationContext
    {
        public bool GetEffectiveOnly { get; set; }
        public int CurrencyId { get; set; }
        public MigrationContext MigrationContext { get; set; }
    }
}
