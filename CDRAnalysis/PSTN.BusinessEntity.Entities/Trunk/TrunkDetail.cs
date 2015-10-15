
namespace PSTN.BusinessEntity.Entities
{
    public class TrunkDetail
    {
        public int TrunkId { get; set; }

        public string Name { get; set; }

        public string Symbol { get; set; }

        public int SwitchId { get; set; }

        public string SwitchName { get; set; }

        public TrunkType Type { get; set; }

        public TrunkDirection Direction { get; set; }

        public int? LinkedToTrunkId { get; set; }

        public string LinkedToTrunkName { get; set; }
    }
}
