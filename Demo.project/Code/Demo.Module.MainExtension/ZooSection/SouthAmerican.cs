using Demo.Module.Entities;

namespace Demo.Module.MainExtension.ZooSection
{
    class SouthAmerican: ZooSectionType
    {
        public int NbOfAnimals { get; set; }

        public override string GetAnimalsNumber()
        {
            return $"The number of animals in this South American Zoo Section is: {NbOfAnimals}";
        }
    }
}
