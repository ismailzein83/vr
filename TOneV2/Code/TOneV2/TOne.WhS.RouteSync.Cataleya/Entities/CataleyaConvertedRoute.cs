using System;
using TOne.WhS.RouteSync.Entities;
using Vanrise.Data.Postgres;

namespace TOne.WhS.RouteSync.Cataleya.Entities
{
    public class CataleyaConvertedRoute : ConvertedRouteWithCode, INpgBulkCopy
    {
        public int CarrierID { get; set; }
        public string IsPercentage { get; set; }
        public string Options { get; set; }
        public string Statistics { get; set; }

        public override string GetCustomer()
        {
            return CarrierID.ToString();
        }

        public override string GetRouteOptionsIdentifier()
        {
            return Options;
        }

        public string ConvertToString()
        {
            return string.Format("{1}{0}{2}{0}{3}{0}{4}", "\t", Code, IsPercentage, Options, Statistics);
        }
    }
}