namespace BPMExtended.Main.SOMAPI
{
    public class ServiceDefinition
    {
        //public int Id { get; set; }
        //public string PublicId { get; set; }
        //public string Title { get; set; }
        //public string Description { get; set; }
        //public string PackageId { get; set; }
        //public string ServiceResource { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string PackageId { get; set; }
        public bool NeedsProvisioning { get; set; }
        public bool IsNetwork { get; set; }
    }
}
