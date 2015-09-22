
namespace PSTN.BusinessEntity.Entities
{
    public class SwitchTrunkDetail
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string Symbol { get; set; }

        public int SwitchID { get; set; }

        public string SwitchName { get; set; }

        public SwitchTrunkType Type { get; set; }

        public SwitchTrunkDirection Direction { get; set; }
    }
}
