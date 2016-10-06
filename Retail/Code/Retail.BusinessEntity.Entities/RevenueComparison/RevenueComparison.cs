using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class RevenueComparison
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public decimal SystemVolume { get; set; }
        public decimal DeclaredVolume { get; set; }
        public decimal DifferenceVolume { get; set; }
        public decimal SystemAmount { get; set; }
        public decimal DeclaredAmount { get; set; }
        public decimal DifferenceAmount { get; set; }
        public OperatorDeclaredInfoTrafficDirection? EventDirection { get; set; }
        public Guid ServiceTypeID { get; set; }
    }
}
