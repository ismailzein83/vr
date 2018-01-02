using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.CaseManagement.Entities
{
    public class VRCase
    {
        public VRCaseValues CaseManagementValues { get; set; }
        
        //public long CaseManagementId { get; set; }
        //public Guid CaseManagementDefinitionId { get; set; }
        //public string PartnerId { get; set; }
        //public string SerialNumber { get; set; }
        //public Guid StatusId { get; set; }
        //public string Title { get; set; }
        //public string Description { get; set; }
        //public long ProblemId { get; set; }
        //public Guid CategoryId { get; set; }
        //public Guid PriorityId { get; set; }
        //public List<int> GroupIds { get; set; }
        //public int OwnerId { get; set; }
        //public CaseManagementSettings Settings { get; set; }
    }
    public class VRCaseValues : Dictionary<string, VRCaseValue>
    {

    }
    public class VRCaseValue
    {
        public object Value { get; set; }
    }
}
