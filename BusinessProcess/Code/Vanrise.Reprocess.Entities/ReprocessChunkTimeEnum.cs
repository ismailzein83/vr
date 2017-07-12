using System;

namespace Vanrise.Reprocess.Entities
{
    public enum ReprocessChunkTimeEnum
    {
        [ReprocessChunkTimeAttribute(30)]
        ThirtyMinutes = 0,
        [ReprocessChunkTimeAttribute(60)]
        OneHour = 1,
        [ReprocessChunkTimeAttribute(120)]
        TwoHours = 2,
        [ReprocessChunkTimeAttribute(180)]
        ThreeHours = 3,
        [ReprocessChunkTimeAttribute(360)]
        SixHours = 4,
        [ReprocessChunkTimeAttribute(720)]
        TwelveHours = 5,
        [ReprocessChunkTimeAttribute(1440)]
        OneDay = 6
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
