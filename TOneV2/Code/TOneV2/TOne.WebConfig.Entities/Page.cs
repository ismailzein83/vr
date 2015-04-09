using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Entities;

namespace TOne.WebConfig.Entities
{
    public class Page : GenericEntity
    {
        public int ModuleId { get; set; }

        public string Name { get; set; }

        public string URL { get; set; }

        public override int? OwnerId
        {
            get
            {
                return this.ModuleId;
            }
        }
    }
}
