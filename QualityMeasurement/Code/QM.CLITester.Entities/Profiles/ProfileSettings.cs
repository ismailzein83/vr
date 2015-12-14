using System.Collections.Generic;

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
