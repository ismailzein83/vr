using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public abstract class SourceCountryReader : EntitySynchronization.ISourceItemReader<SourceCountry>
    {
        public abstract Guid ConfigId { get; }

        public abstract bool UseSourceItemId
        {
            get;
        }

        public abstract IEnumerable<SourceCountry> GetChangedItems(ref object updatedHandle);
    }
}
