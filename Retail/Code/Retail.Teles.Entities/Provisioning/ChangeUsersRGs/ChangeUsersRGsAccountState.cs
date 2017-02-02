using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Teles.Entities
{
    public class ChangeUsersRGsAccountState
    {
        public Dictionary<string, ChangeUsersRGsAccountStateActionTypeChanges> ChangesByActionType { get; set; }
    }

    public class ChangeUsersRGsAccountStateActionTypeChanges
    {
        public Dictionary<long, ChangeUsersRGsAccountStateUserChange> ChangesByUser { get; set; }
    }

    public class ChangeUsersRGsAccountStateUserChange
    {
        public long SiteId { get; set; }

        public long OriginalRGId { get; set; }
    }
}
