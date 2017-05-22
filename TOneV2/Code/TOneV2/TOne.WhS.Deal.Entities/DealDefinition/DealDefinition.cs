namespace TOne.WhS.Deal.Entities
{
    public class DealDefinition
    {
        public int DealId { get; set; }

        public string Name { get; set; }

        public DealSettings Settings { get; set; }
    }
}