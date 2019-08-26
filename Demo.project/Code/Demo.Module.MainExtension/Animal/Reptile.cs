using Demo.Module.Entities;

namespace Demo.Module.MainExtension.Animal
{
    public enum NourrisementEnum { Egg, Rabbit }
    public class Reptile : ZooAnimal
    {
        public NourrisementEnum Nourrisement { get; set; }
    }
}
