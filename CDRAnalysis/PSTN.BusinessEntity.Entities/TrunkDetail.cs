
namespace PSTN.BusinessEntity.Entities
{
    public class TrunkDetail
    {
        public Trunk Entity { get; set; }
        
        public string SwitchName { get; set; }

        public string LinkedToTrunkName { get; set; }
    }
}
