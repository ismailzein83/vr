using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities.EntitySynchronization;

namespace Demo.Module.Entities
{
    public abstract class GroupType
    {
        public int ConfigId { get; set; }

        public List<int> SelectedIds { get; set; }

    }
}
