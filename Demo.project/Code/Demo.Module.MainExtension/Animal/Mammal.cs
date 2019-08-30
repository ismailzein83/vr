using System;
using Demo.Module.Entities;

namespace Demo.Module.MainExtension.Animal
{
    public class Mammal: ZooAnimal
    {
        public override Guid ConfigId { get { return new Guid("436E9B04-6BE6-412B-ADFF-9909145DF963"); } }
        public decimal HighestJump { get; set; }
    }
}
