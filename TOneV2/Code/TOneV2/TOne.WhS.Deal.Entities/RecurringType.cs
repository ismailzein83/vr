using System.ComponentModel;

namespace TOne.WhS.Deal.Entities
{
    public enum RecurringType
    {
        [Description("Daily")]
        Daily = 0,

        [Description("Monthly")]
        Monthly = 1,
    }

    public abstract class DealTimePeriod  : Vanrise.Entities.VRTimePeriod
    {

    }

    public class DealTimePeriodConfig : Vanrise.Entities.ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "WhS_Deal_DealTimePeriodConfig";

        public string Editor { get; set; }
    }
}