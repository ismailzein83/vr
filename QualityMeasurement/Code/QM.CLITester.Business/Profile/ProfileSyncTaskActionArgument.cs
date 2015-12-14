using QM.CLITester.Entities;

namespace QM.CLITester.Business
{
    public class ProfileSyncTaskActionArgument : Vanrise.Runtime.Entities.BaseTaskActionArgument
    {
        public SourceProfileReader SourceProfileReader { get; set; }
    }
}
