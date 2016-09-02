using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public enum OperatorDeclaredInfoTrafficDirection { In = 1, Out = 2}
    public class OperatorDeclaredInfo
    {
        public int OperatorDeclaredInfoId { get; set; }

        public OperatorDeclaredInfoSettings Settings { get; set; }
    }

    public class OperatorDeclaredInfoSettings
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public string Notes { get; set; }

        public int? AttachmentFileId { get; set; }

        public List<OperatorDeclaredInfoItem> Items { get; set; }
    }

    public class OperatorDeclaredInfoItem
    {
        public int ServiceTypeId { get; set; }

        public OperatorDeclaredInfoTrafficDirection?  TrafficDirection { get; set; }

        public Decimal Volume { get; set; }

        public Decimal Amount { get; set; }
    }
}
