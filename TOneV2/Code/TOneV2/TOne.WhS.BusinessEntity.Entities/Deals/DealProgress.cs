using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class DealProgress
    {
        public int DealProgressId { get; set; }
        public DateTime ProgressDate { get; set; }
        public bool  IsSelling { get; set; }
        public decimal EstimatedDuration { get; set; }
        public decimal ReachedDuration { get; set; }
        public decimal EstimatedAmount { get; set; }
        public decimal ReachedAmount { get; set; }
    }
    public class DealProgressDetail
    {
        public DealProgress Entity { get; set; }
        public decimal Completion { get; set; }
        public bool Indication { get; set; }
    }

    public class DealProgressQuery
    {
        public DateTime FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public bool IsSelling { get; set; }
      
    }
}