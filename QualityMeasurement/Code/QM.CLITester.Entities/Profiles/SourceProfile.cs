using Vanrise.Entities.EntitySynchronization;

namespace QM.CLITester.Entities
{
    public class SourceProfile : ISourceItem
    {

        public string SourceId
        {
            get;
            set;
        }

        public string Name { get; set; }
    }
}
