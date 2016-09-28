using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Vanrise.Entities;

namespace QM.CLITester.iTestIntegration
{
    public class CountryITestReader : Vanrise.Entities.SourceCountryReader
    {
        public override bool UseSourceItemId
        {
            get { return false; }
        }


        public override IEnumerable<Vanrise.Entities.SourceCountry> GetChangedItems(ref object updatedHandle)
        {
            ITestZoneManager zoneManager = new ITestZoneManager();
            var itestZones = zoneManager.GetAllZones();
            if (itestZones != null)
            {
                List<Vanrise.Entities.SourceCountry> countries = new List<SourceCountry>();
                HashSet<string> addedCountryIds = new HashSet<string>();
                foreach (var z in itestZones.Values)
                {
                    if (!addedCountryIds.Contains(z.CountryId))
                    {
                        var country = new SourceCountry
                        {
                            SourceId = z.CountryId,
                            Name = z.CountryName
                        };
                        countries.Add(country);
                        addedCountryIds.Add(country.SourceId);
                    }
                }
                return countries;
            }
            else
                return null;

        }

        public override Guid ConfigId
        {
             get{return new Guid("BB371F00-B036-4745-BDBC-65637F253054");}
        }
    }
}
