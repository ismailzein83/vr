using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOM.Main.Entities
{
    public class PromotionPackage
    {
        public string CustomerId { get; set; }
        public string PromotionPackageId { get; set; }
        public string PromotionPackageSeq { get; set; }
        public DateTime AssignDate { get; set; }
        public List<PromotionAssignmentState> States { get; set; }
    }

    public class PromotionAssignmentState
    {
        public int StateSeq { get; set; }
        public string StateStatus { get; set; }
        public DateTime ValidDate { get; set; }
    }
}
