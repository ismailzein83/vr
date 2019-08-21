using System;

namespace Demo.Module.Entities
{
    public class Product
    {
        public long Id { get; set; }
        public int ManufactoryId { get; set; }
        public string Name { get; set; }
        public ProductSettings Settings { get; set; }
    }

    public abstract class ProductSettings
    {
        public abstract Guid ConfigId { get; }
        public double Price { get; set; }
        public abstract string GetDescription();
    }

    public abstract class SoftwareOperatingSystem
    {
        public abstract Guid ConfigId { get; }
        public string Version { get; set; }
        public abstract string GetDescription();
    }
}