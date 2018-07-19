using System.ComponentModel;

namespace TOne.WhS.Deal.Entities
{
    public enum SubstituteRateType
    {
        [Description("Fixed Rate")]
        FixedRate = 0,

        [Description("Normal Rate")]
        NormalRate = 1,

        [Description("Deal Rate")]
        DealRate = 2
    }

}
