﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities.EntitySynchronization;

namespace QM.BusinessEntity.Entities
{
    public abstract class SourceZoneReader : Vanrise.Entities.EntitySynchronization.ISourceItemReader<SourceZone>
    {
        public int ConfigId { get; set; }

        public abstract bool UseSourceItemId
        {
            get;
        }

        public abstract IEnumerable<SourceZone> GetChangedItems(ref object updatedHandle);

        public Vanrise.Entities.SourceCountryReader CountryReader
        {
            get;
            set;
        }
    }
}
