using Demo.Module.Entities;
using System.Collections.Generic;

namespace Demo.Module.MainExtension.ZooSection
{
    public class African: ZooSectionType
    {
        public List<ZooAnimal> Animals { get; set; }

        public override string GetAnimalsNumber()
        {
            return $"The number of animals in this African Zoo Section is: {Animals.Count}";
        }
    }
}
