using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public interface IVanriseType
    {
        string UniqueTypeName { get; }
    }

    public class VanriseType : IVanriseType
    {
        public VanriseType(string uniqueTypeName)
        {
            this.UniqueTypeName = uniqueTypeName;
        }

        public string UniqueTypeName { get; private set; }
    }
}
