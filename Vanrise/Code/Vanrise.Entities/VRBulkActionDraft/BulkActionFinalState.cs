using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class BulkActionFinalState
    {
        public bool IsAllSelected { get; set; }
        public List<BulkActionItem> TargetItems { get; set; }
        public Guid BulkActionDraftIdentifier { get; set; }
    }
}
