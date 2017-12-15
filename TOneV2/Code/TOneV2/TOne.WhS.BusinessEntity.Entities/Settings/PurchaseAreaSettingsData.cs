namespace TOne.WhS.BusinessEntity.Entities
{
    public class PurchaseAreaSettingsData : Vanrise.Entities.SettingData
    {
        public int EffectiveDateDayOffset { get; set; }
        public int RetroactiveDayOffset { get; set; }
        public decimal MaximumRate { get; set; }
        public long MaximumCodeRange { get; set; }
    }
}
