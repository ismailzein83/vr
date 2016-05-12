
namespace TOne.WhS.DBSync.Entities
{
    public class SourceCurrency : Vanrise.Entities.EntitySynchronization.ISourceItem
    {

        public string SourceId
        {
            get;
            set;
        }

        public string Name { get; set; }

        public string Symbol { get; set; }
    }
}
