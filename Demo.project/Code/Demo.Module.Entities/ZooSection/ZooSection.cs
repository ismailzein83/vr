using System;
using System.Collections.Generic;

namespace Demo.Module.Entities
{
    public enum ZooSectionPositionEnum { Top, Bottom, Left, Right, Middle }

    public class ZooSection
    {
        public long ZooSectionId { get; set; }
        public string Name { get; set; }
        public long ZooId { get; set; }
        public ZooSectionType Type { get; set; }
    }

    public abstract class ZooSectionType
    {
        public abstract Guid ConfigId { get; }
        public bool HasRiver { get; set; }
        public int NbOfVisitors { get; set; }
        public List<ZooSectionPositionEnum> Positions { get; set; }
        public abstract int GetAnimalsNumber(IZooSectionTypeGetAnimalsNumberContext context);
    }

    public interface IZooSectionTypeGetAnimalsNumberContext
    {
        ZooSectionType Type { get; }
    }

    public abstract class ZooAnimal
    {
        public string Name { get; set; }
        public decimal Weight { get; set; }
    }
}
