using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public abstract class GroupSettings
    {
        public int ConfigId { get; set; }

        public abstract bool IsMember(IGroupSettingsContext context);
    }
}
