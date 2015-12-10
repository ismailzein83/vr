using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.CLITester.Entities
{
    public class Profile
    {
        public int ProfileId { get; set; }

        public string Name { get; set; }

        public string SourceId { get; set; }

        public ProfileSettings Settings { get; set; }
    }
}
