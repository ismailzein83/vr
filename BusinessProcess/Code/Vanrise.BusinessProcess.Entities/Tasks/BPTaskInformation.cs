using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess.Entities
{
    public abstract class BPTaskInformation
    {
        public BPTaskAssignee AssignedTo { get; set; }

        public List<BPTaskAction> Actions { get; set; }
    }

    public class TestBPTaskInformation : BPTaskInformation
    {

    }

    public class TestBPTaskAssignee : BPTaskAssignee
    {
        public override IEnumerable<int> GetUserIds(IBPTaskAssigneeContext context)
        {
            return new List<int>() { 1, 3, 5, 7 };
        }

        public override string GetDescription(IBPTaskAssigneeContext context)
        {
            return "Test Description";
        }
    }
}
