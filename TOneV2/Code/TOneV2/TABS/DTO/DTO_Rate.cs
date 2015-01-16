using System;

namespace TABS.DTO
{
    public class DTO_Rate : Components.FlaggedServicesEntity, IComparable<DTO_Rate>
    {
        public double? _Normal;
        public double? _OffPeak;
        public double? _Weekend;
        public double? _Duration;
        public double? _ASR;
        public double? _ACD;
        public double? _TQI;


        public double? Normal { get { return _Normal; } set { _Normal = value; } }
        public double? OffPeak { get { return _OffPeak; } set { _OffPeak = value; } }
        public double? Weekend { get { return _Weekend; } set { _Weekend = value; } }

        public double? ASR { get { return _ASR; } set { _ASR = value; } }
        public double? TQI { get { return ASR == null || ACD == null ? null : ASR * ACD * 10; } }
        public double? ACD { get { return _ACD; } set { _ACD = value; } }
        public double? Duration { get { return _Duration; } set { _Duration = value; } }

        public DateTime? EndEffectiveDate { get; set; }

        public override string ToString() { return Normal.ToString(); }

        public override string Identifier { get { return "DTO_Rate " + this.Normal; } }

        #region IComparable<DTO_Rate> Members

        public int CompareTo(DTO_Rate other)
        {
            return this.Normal.Value.CompareTo(other.Normal.Value);
        }

        #endregion
    }
}