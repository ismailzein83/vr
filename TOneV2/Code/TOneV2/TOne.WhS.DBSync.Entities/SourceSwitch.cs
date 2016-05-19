
using Vanrise.Entities.EntitySynchronization;
namespace TOne.WhS.DBSync.Entities
{
    public class SourceSwitch : ISourceItem
    {

        public string SourceId
        {
            get;
            set;
        }

        public string Name { get; set; }
    }
}
