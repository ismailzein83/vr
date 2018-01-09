using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOM.Main.Entities
{
    public class CreateSOMRequestInput
    {
        public string EntityId { get; set; }

        public SOMRequestSettings Settings { get; set; }
    }
}
