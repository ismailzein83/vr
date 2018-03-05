using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace Vanrise.Common.MainExtensions.Country
{
    public class SelectiveCountryCriteriaGroup : CountryCriteriaGroupSettings
    {
        public override Guid ConfigId { get { return new Guid("4EC42931-14E9-4807-9104-10985446D26B"); } }
        public List<int> CountryIds { get; set; }

        public override IEnumerable<int> GetCountryIds(ICountryCriteriaGroupContext context)
        {
            return this.CountryIds;
        }

        public override string GetDescription(ICountryCriteriaGroupContext context)
        {
            if (this.CountryIds != null)
            {
                CountryManager manager = new CountryManager();
                return manager.GetDescription(CountryIds);
            }
            else
                return null;
        }
    }
}
