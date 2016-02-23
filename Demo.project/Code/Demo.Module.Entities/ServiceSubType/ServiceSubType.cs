using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities.EntitySynchronization;

namespace Demo.Module.Entities
{
    public abstract class ServiceSubType
    {
        public int ConfigId { get; set; }

        public int SelectedId { get; set; }

    }
}
