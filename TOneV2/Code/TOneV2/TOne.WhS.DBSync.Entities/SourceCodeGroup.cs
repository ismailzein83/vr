﻿
using Vanrise.Entities.EntitySynchronization;
namespace TOne.WhS.DBSync.Entities
{
    public class SourceCodeGroup : ISourceItem
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
