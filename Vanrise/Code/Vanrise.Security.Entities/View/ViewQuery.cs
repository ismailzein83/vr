using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public class ViewQuery
    {
        public Guid? ModuleId { get; set; }
        public List<Guid> ViewTypes { get; set; }
        public string Name { get; set; }
    }
}
