﻿using System;
using Vanrise.Entities.EntitySynchronization;

namespace TOne.WhS.DBSync.Entities
{
    public class SourcePriceList : ISourceItem
    {

        public string SourceId
        {
            get;
            set;
        }

        public string SupplierId { get; set; }
        public string CustomerId { get; set; }
        public string CurrencyId { get; set; }
        public byte[] SourceFileBytes { get; set; }
        public string SourceFileName { get; set; }
    }
}
