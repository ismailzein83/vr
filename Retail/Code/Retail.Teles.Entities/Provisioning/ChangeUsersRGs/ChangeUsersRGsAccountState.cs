using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Teles.Entities
{
    public class ChangeUsersRGsAccountState:BaseAccountExtendedSettings
    {
        public Dictionary<string, ChURGsActionCh> ChangesByActionType { get; set; }
    }

    public class ChURGsActionCh
    {
        public Dictionary<dynamic, ChURGsUserCh> ChangesByUser { get; set; }
    }

    public class ChURGsUserCh
    {
        public dynamic OriginalRGId { get; set; }
    }
}
