using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class VRComponentType
    {
        public Guid VRComponentTypeId { get; set; }

        public string Name { get; set; }

        public VRComponentTypeSettings Settings { get; set; }
    }

    public class VRComponentType<T> where T : VRComponentTypeSettings
    {
        public Guid VRComponentTypeId { get; set; }

        public string Name { get; set; }

        public T Settings { get; set; }
    }
}
