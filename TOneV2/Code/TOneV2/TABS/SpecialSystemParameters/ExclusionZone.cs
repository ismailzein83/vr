using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace TABS.SpecialSystemParameters
{
    public class ExclusionZone
    {
        /// <summary>
        /// the corresponding Customer
        /// </summary>
        public CarrierAccount CarrierAccount { get; set; }

        /// <summary>
        /// String comma seperated Zoneid 
        /// </summary>
        public string ExcludedZonesData { get; set; }


        /// <summary>
        /// Get a list of Zones from a string comma seperated Zoneid 
        /// </summary>
        public List<Zone> ExcludedZones
        {
            get
            {
                List<Zone> result = new List<Zone>();
                if (!string.IsNullOrEmpty(ExcludedZonesData))
                {
                    var data = ExcludedZonesData.Split(',');
                    foreach (Zone zone in TABS.Zone.OwnZones.Values)
                    {
                        if (data.Any(s => s == zone.ZoneID.ToString()))
                            result.Add(zone);
                    }
                }
                return result;
            }
            set
            {
                 ExcludedZonesData = value.Select(z => z.ZoneID.ToString()).DefaultIfEmpty("").Aggregate((s1, s2) => s1 + ',' + s2);
            }
        }

        /// <summary>
        /// get all excluded zones corresponding to each customer
        /// </summary>
        /// <returns></returns>
        public static List<ExclusionZone> GetExclusionZones()
        {
            string parameter = SystemConfiguration.KnownParameters[KnownSystemParameter.CustomerExcludedZones].LongTextValue;

            List<ExclusionZone> result = new List<ExclusionZone>();
            XDocument document = XDocument.Parse(parameter, LoadOptions.None);

            result = document.Elements().Elements()
                .Select(e =>
                    new ExclusionZone
                    {
                        CarrierAccount = TABS.CarrierAccount.All[e.Element("CarrierAccountID").Value],
                        ExcludedZonesData = e.Element("ExcludedZonesData").Value
                    }).ToList();

            return result;
        }


        /// <summary>
        /// get all excluded zones corresponding to a specific customer
        /// </summary>
        /// <returns></returns>
        public static List<Zone> GetExclusionZones(CarrierAccount customer)
        {
            try { return GetExclusionZones().Where(e => e.CarrierAccount.Equals(customer)).Single().ExcludedZones; }
            catch { return new List<Zone>(); }
        }

        /// <summary>
        /// save or update the system parametre that represent excluded zones for each customer
        /// </summary>
        /// <param name="ExclusionZones"></param>
        /// <returns></returns>
        public static Exception Save(List<ExclusionZone> ExclusionZones)
        {
            SystemParameter parameter = SystemConfiguration.KnownParameters[KnownSystemParameter.CustomerExcludedZones];

            parameter.LongTextValue = new XElement("Customers",
                   ExclusionZones.Select(r =>
                       new XElement("Customer", new XElement("CarrierAccountID", r.CarrierAccount.CarrierAccountID),
                       new XElement("ExcludedZonesData", r.ExcludedZonesData)))).ToString();


            Exception ex;
            ObjectAssembler.SaveOrUpdate(parameter, out ex);
            return ex;
        }

        public static string DefaultXml { get { return @"<Customers></Customers>"; } }
    }
}
