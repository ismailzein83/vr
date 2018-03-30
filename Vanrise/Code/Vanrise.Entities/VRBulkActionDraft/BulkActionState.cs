using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class BulkActionState
    {
        public Guid BulkActionDraftIdentifier { get; set; }
        public bool IsAllSelected { get; set; }
        public bool ReflectedToDB { get; set; }
    }
    public class BulkActionItem
    {
        public string ItemId { get; set; }
    }
}
