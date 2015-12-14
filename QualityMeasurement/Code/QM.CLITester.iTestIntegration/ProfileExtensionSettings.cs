
namespace QM.CLITester.iTestIntegration
{
    public enum ProfileType { Voice = 1, SMS = 2}
    public class ProfileExtensionSettings : QM.CLITester.Entities.ExtendedProfileSetting
    {
        public ProfileType Type { get; set; }

        public string GatewayIP { get; set; }

        public string GatewayPort { get; set; }

        public string SourceNumber { get; set; }

        public int CallTime { get; set; }

        public int RingTime { get; set; }

        public override void Apply(Entities.Profile profile)
        {
          //  throw new NotImplementedException();
        }
    }
}
