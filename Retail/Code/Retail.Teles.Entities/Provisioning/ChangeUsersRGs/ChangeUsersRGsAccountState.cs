using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Teles.Entities
{
    public class ChangeUsersRGsAccountState
    {
        public Dictionary<string, ChURGsActionCh> ChangesByActionType { get; set; }
    }

    public class ChURGsActionCh
    {
        public Dictionary<long, ChURGsUserCh> ChangesByUser { get; set; }
    }

    public class ChURGsUserCh
    {
        public long OriginalRGId { get; set; }
    }
}
