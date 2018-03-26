using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{
    public abstract class CollegeInfoType
    {
        public abstract Guid ConfigId { get; }

        public abstract string getDescription();
    }

}
