using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class CountryTOneV2SQLReader : SourceCountryReader
    {
        public string ConnectionString { get; set; }
        public override bool UseSourceItemId
        {
            get { return true; }
        }

        public override IEnumerable<SourceCountry> GetChangedItems(ref object updatedHandle)
        {
            throw new NotImplementedException();
        }
    }
}
