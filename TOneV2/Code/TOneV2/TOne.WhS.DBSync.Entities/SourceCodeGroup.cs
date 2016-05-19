
namespace TOne.WhS.DBSync.Entities
{
    public class SourceCodeGroup : Vanrise.Entities.EntitySynchronization.ISourceItem
    {

        public string SourceId
        {
            get;
            set;
        }

        public string Name { get; set; }

        public string Code { get; set; }
    }
}
