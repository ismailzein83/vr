using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CDRProcessing.Entities
{
    public abstract class BaseTrafficStatistic : Vanrise.Entities.StatisticManagement.IStatisticItem
    {
        public long StatisticItemId { get; set; }

        public DateTime BatchStart { get; set; }

        public DateTime BatchEnd { get; set; }

        public int CustomerId { get; set; }

        public int SupplierId { get; set; }

        public int Attempts { get; set; }

        public long TotalDurationInSeconds { get; set; }

        public static string GetStatisticItemKey(int customerId, int supplierId)
        {
            return string.Format("{0}_{1}", customerId, supplierId);
        }
    }
}
