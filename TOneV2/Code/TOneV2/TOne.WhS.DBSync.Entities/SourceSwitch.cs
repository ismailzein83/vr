
namespace TOne.WhS.DBSync.Entities
{
    public class SourceSwitch : Vanrise.Entities.EntitySynchronization.ISourceItem
    {

        public string SourceId
        {
            get;
            set;
        }

        public string Name { get; set; }
    }
}
