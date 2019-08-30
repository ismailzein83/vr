using Demo.Module.Entities;
using System;
using System.Collections.Generic;

namespace Demo.Module.MainExtension.ZooSection
{
    public class African : ZooSectionType
    {
        public override Guid ConfigId { get { return new Guid("9CCB94CF-D8A2-47ED-B78B-DDC932544846"); } }
        public List<ZooAnimal> Animals { get; set; }

        public override int GetAnimalsNumber(IZooSectionTypeGetAnimalsNumberContext context)
        {
            return Animals != null ? Animals.Count : 0;
        }
    }
}
