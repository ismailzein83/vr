using System;
using Demo.Module.Entities;

namespace Demo.Module.MainExtension.Animal
{
    public enum ReptileNourrissementEnum { Egg, Rabbit }
    public class Reptile : ZooAnimal
    {
        public override Guid ConfigId { get { return new Guid("56C1C100-9340-4263-92FC-674A014F4C87"); } }
        public ReptileNourrissementEnum Nourrisement { get; set; }
    }
}
