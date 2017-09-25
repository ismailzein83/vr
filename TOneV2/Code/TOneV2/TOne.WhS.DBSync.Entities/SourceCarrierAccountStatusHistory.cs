using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities.EntitySynchronization;

namespace TOne.WhS.DBSync.Entities
{
    public class SourceCarrierAccountStatusHistory : ISourceItem
    {
        public int CarrierAccountId { get; set; }

        public ActivationStatus ActivationStatus { get; set; }

        public string SourceId
        {
            get { return null; }
        }
    }
}
