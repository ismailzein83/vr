using System.ComponentModel;

namespace Vanrise.Fzero.CDRImport.Entities
{
    public enum SubscriberType
    {
        [Description("INROAMER")]
        INROAMER = 0,

        [Description("OUTROAMER")]
        OUTROAMER = 1,

        [Description("POSTPAID")]
        POSTPAID = 2,

        [Description("PREPAID")]
        PREPAID = 3,

        [Description("PREROAMER")]
        PREROAMER = 4
    };
}
