using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Teles.Entities
{
    public enum ChURGsActionChStatus { Active = 0, Blocked = 1  }
    public class ChangeUsersRGsAccountState:BaseAccountExtendedSettings
    {
        public Dictionary<string, ChURGsActionCh> ChangesByActionType { get; set; }
    }

    public class ChURGsActionCh
    {
        public Dictionary<string, ChURGsUserCh> ChangesByUser { get; set; }
        public ChURGsActionChStatus Status { get; set; }
    }

    public class ChURGsUserCh
    {
        public dynamic OriginalRGId { get; set; }
        public dynamic ChangedRGId { get; set; }
        public string SiteId { get; set; }
    }
}
