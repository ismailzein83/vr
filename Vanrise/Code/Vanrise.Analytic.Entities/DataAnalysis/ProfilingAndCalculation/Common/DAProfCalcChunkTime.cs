using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public enum DAProfCalcChunkTimeEnum
    {
        [DAProfCalcChunkTimeAttribute(60)]
        OneHour = 0,
        [DAProfCalcChunkTimeAttribute(120)]
        TwoHours = 1,
        [DAProfCalcChunkTimeAttribute(180)]
        ThreeHours = 2,
        [DAProfCalcChunkTimeAttribute(360)]
        SixHours = 3,
        [DAProfCalcChunkTimeAttribute(720)]
        TwelveHours = 4,
    }

    public class DAProfCalcChunkTimeAttribute : Attribute
    {
        public DAProfCalcChunkTimeAttribute(int value)
        {
            this.Value = value;
        }
        public int Value { get; set; }
    }
}
