namespace CP.SupplierPricelist.Entities
{
    public class GetBeforeIdInput
    {
        public long LessThanID { get; set; }
        public int NbOfRows { get; set; }
        public int UserId { get; set; }
    }
}
