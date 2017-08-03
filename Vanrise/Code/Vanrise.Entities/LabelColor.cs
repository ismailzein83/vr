using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Vanrise.Entities
{
    public enum LabelColor
    {
        [LabelColor(ColorString = "#5cb85c")]
        Success = 0,
        [LabelColor(ColorString = "#f0ad4e")]
        Warning = 1,
        [LabelColor(ColorString = "#FFD400")]
        WarningLevel1 = 2,
        [LabelColor(ColorString = "#FFAA00")]
        WarningLevel2 = 3,
        [LabelColor(ColorString = "#d9534f")]
        Error = 4,
        [LabelColor(ColorString = "#5bc0de")]
        Info = 5,
        [LabelColor(ColorString = "#337ab7")]
        Primary = 6,
        [LabelColor(ColorString = "#777")]
        Default = 7,
        [LabelColor(ColorString = "#5bc0de")]
        New = 8,
        [LabelColor(ColorString = "#f0ad4e")]
        Processing = 9,
        [LabelColor(ColorString = "#d9534f")]
        Failed = 10,
        [LabelColor(ColorString = "#5cb85c")]
        Processed = 11,
        [LabelColor(ColorString = "#ff0000")]
        DangerFont = 12,
    }

    public class LabelColorAttribute : Attribute
    {
        public string ColorString { get; set; }
    }
}
