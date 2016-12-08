using System;

namespace Vanrise.Reprocess.Entities
{
    public enum ReprocessChunkTimeEnum
    {
        [ReprocessChunkTimeAttribute(5)]
        FiveMinutes = 0,
        [ReprocessChunkTimeAttribute(10)]
        TenMinutes = 1,
        [ReprocessChunkTimeAttribute(15)]
        FifteenMinutes = 2,
        [ReprocessChunkTimeAttribute(30)]
        ThirtyMinutes = 3,
        [ReprocessChunkTimeAttribute(60)]
        OneHour = 4,
        [ReprocessChunkTimeAttribute(120)]
        TwoHours = 5,
        [ReprocessChunkTimeAttribute(180)]
        ThreeHours = 6,
        [ReprocessChunkTimeAttribute(360)]
        SixHours = 7
    }

    public class ReprocessChunkTimeAttribute : Attribute
    {
        public ReprocessChunkTimeAttribute(int value)
        {
            this.Value = value;
        }
        public int Value { get; set; }
    }
}
