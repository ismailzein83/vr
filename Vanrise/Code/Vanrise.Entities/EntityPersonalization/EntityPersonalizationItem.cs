using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class EntityPersonalizationItem
    {
        public EntityPersonalizationItemSetting Setting { get; set; }
    }

    public abstract class EntityPersonalizationItemSetting
    {
        public abstract string Title { get; }
    }
}
