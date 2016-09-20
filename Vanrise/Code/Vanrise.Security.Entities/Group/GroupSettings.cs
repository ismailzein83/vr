using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public abstract class GroupSettings
    {
        public abstract Guid ConfigId { get; }

        public abstract bool IsMember(IGroupSettingsContext context);
    }
}
