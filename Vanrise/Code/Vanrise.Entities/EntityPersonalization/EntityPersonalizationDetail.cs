using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class EntityPersonalizationDetail
    {
        public string EntityUniqueName { get; set; }
        public EntityPersonalizationExtendedSetting ExtendedSetting { get; set; }
        public bool IsGlobal { get; set; }
    }
    public class EntityPersonalizationData
    {
        public List<EntityPersonalizationDetail> UserEntityPersonalizations { get; set; }

        public List<EntityPersonalizationDetail> GlobalEntityPersonalizations { get; set; }

    }

}
