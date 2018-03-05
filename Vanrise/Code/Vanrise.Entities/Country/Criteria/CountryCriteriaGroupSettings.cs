using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public abstract class CountryCriteriaGroupSettings
    {
        public abstract Guid ConfigId { get; }

        public abstract IEnumerable<int> GetCountryIds(ICountryCriteriaGroupContext context);

        public abstract string GetDescription(ICountryCriteriaGroupContext context);
    }
}
