using System.Collections.Generic;

namespace Demo.Module.Entities
{
    public enum ZooSectionPositionEnum { Top, Bottom, Left, Right, Middle }

    public class ZooSection
    {
        public long ZooSectionId { get; set; }
        public string Name { get; set; }
        public long ZooId { get; set; }
        public List<ZooSectionPositionEnum> Position { get; set; }
        public ZooSectionType Type { get; set; }
    }

    public abstract class ZooSectionType
    {
        public int HasRiver { get; set; }
        public int NbOfVisitors { get; set; }
        public abstract string GetAnimalsNumber();
    }

    public abstract class ZooAnimal
    {
        public string Name { get; set; }
        public decimal Weight { get; set; }
    }
}
