using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Entities;

namespace TOne.WebConfig.Entities
{
    public class Module : GenericEntity
    {
        public string Name { get; set; }

        public int? ParentModuleId { get; set; }

        public int? DefaultPageId { get; set; }

        public override int? ParentId
        {
            get
            {
                return this.ParentModuleId;
            }
        }
    }
}
