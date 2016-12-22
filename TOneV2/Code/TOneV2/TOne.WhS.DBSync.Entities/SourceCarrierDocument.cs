using System;
using Vanrise.Entities.EntitySynchronization;
namespace TOne.WhS.DBSync.Entities
{

  
    public class SourceCarrierDocument : ISourceItem
    {

        public string SourceId
        {
            get;
            set;
        }

        public short ProfileId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public  byte[] Document { get; set; }
        public DateTime Created { get; set; }

    }
}

