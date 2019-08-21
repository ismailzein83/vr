using System;
using Demo.Module.Entities;
using System.Collections.Generic;

namespace Demo.Module.MainExtension.Product
{
    public enum Material { Plastic, Glass, Steel, Metal, Fiber, Aluminum, Ceramic}
    public class Hardware : ProductSettings
    {
        public override Guid ConfigId { get { return new Guid("9507931D-A93E-49BC-A93C-3CE39452EC6D"); } }
        public double Weight { get; set; }
        public double Volume { get; set; }
        public List<Material> Materials { get; set; }
        public override string GetDescription()
        {
            return $"Hardware of weight: {Weight.ToString()} kg and volume: { Volume.ToString() } m3 made using { Materials.Count } materials - costs: { Price.ToString() } $";
        }
    }
}
