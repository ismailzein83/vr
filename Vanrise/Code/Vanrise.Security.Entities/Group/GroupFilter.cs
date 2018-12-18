using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public class GroupFilter
    {
        public EntityType? EntityType { get; set; }
        public string EntityId { get; set; }
        public bool OnlyStatic { get; set; }
    }
}
