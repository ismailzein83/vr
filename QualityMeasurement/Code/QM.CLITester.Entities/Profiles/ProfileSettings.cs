using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.CLITester.Entities
{
    public class ProfileSettings
    {
        public List<ExtendedProfileSetting> ExtendedSettings { get; set; }
    }

    public abstract class ExtendedProfileSetting
    {
        public abstract void Apply(Profile profile);
    }
}
