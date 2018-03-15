using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public enum DAProfCalcChunkTimeEnum
    {
        [DAProfCalcChunkTimeAttribute(15)]
        FifteenMinutes = 0,
        [DAProfCalcChunkTimeAttribute(30)]
        ThirtyMinutes = 1,
        [DAProfCalcChunkTimeAttribute(60)]
        OneHour = 2,
        [DAProfCalcChunkTimeAttribute(120)]
        TwoHours = 3,
        [DAProfCalcChunkTimeAttribute(180)]
        ThreeHours = 4,
        [DAProfCalcChunkTimeAttribute(360)]
        SixHours = 5,
        [DAProfCalcChunkTimeAttribute(720)]
        TwelveHours = 6,
    }

    public class DAProfCalcChunkTimeAttribute : Attribute
    {
        public DAProfCalcChunkTimeAttribute(int value)
        {
            this.Value = value;
        }
        public int Value { get; set; }
    }

    public enum DAProfCalcTimeUnit
    {
        [DAProfCalcTimeUnitAttribute(1440, "Day", "Days")]
        Days = 0,
        [DAProfCalcTimeUnitAttribute(60, "Hour", "Hours")]
        Hours = 1,
        [DAProfCalcTimeUnitAttribute(1, "Minute", "Minutes")]
        Minutes = 2
    }

    public class DAProfCalcTimeUnitAttribute : Attribute
    {
        public DAProfCalcTimeUnitAttribute(int value, string singularDescription, string pluralDescription)
        {
            this.Value = value;
            this.SingularDescription = singularDescription;
            this.PluralDescription = pluralDescription;
        }
        public int Value { get; set; }

        public string SingularDescription { get; set; }
        public string PluralDescription { get; set; }
    }
}
