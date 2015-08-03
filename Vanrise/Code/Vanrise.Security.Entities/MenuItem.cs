using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Vanrise.Security.Entities
{
    public class MenuItem
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }

        public string Location { get; set; }

        public List<MenuItem> Childs { get; set; }

        public string Icon { get; set; }
        public ViewType Type { get; set; }
        public bool AllowDynamic { get; set; }
        public int Rank { get; set; }
    }
}
