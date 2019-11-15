namespace TOne.WhS.RouteSync.Cataleya.Entities
{
    public class CarrierAccountMapping
    {
        public int Version { get; set; }
        public int CarrierId { get; set; }
        public string RouteTableName { get; set; }
        public int ZoneID { get; set; }

        //public CarrierAccountStatus Status { get; set; }
    }

    //public enum CarrierAccountStatus
    //{
    //    Active = 1,
    //    Blocked = 2
    //}
}