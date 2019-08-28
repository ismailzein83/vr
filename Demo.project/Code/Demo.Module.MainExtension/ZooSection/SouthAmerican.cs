using Demo.Module.Entities;
using System;

namespace Demo.Module.MainExtension.ZooSection
{
    public class SouthAmerican : ZooSectionType
    {
        public override Guid ConfigId { get { return new Guid("8A91B056-91A7-46B0-97FB-3F03CE51F6A6"); } }
        public int NbOfAnimals { get; set; }

        public override int GetAnimalsNumber(IZooSectionTypeGetAnimalsNumberContext context)
        {
            return NbOfAnimals;
        }
    }
}
