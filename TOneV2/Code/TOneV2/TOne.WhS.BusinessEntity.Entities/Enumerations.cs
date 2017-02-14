using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public enum RateTypeEnum : byte
    {
        Normal = 0,
        OffPeak = 1,
        Weekend = 2
    }

    public enum PeriodTypeEnum
    {
        Days = 0,
        Hours = 1,
        Minutes = 2
    }
}
