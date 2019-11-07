using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Entities;
using Vanrise.Data.Postgres;

namespace TOne.WhS.RouteSync.Cataleya.Entities
{
    public class CataleyaConvertedRoute : ConvertedRouteWithCode, INpgBulkCopy
    {
        public int CarrierID { get; set; }
        public bool IsPercentage { get; set; }
        public string Options { get; set; }

        public override string GetCustomer()
        {
            return CarrierID.ToString();
        }

        public string ConvertToString()
        {
            return string.Format("{1}{0}{2}{0}{3}", "\t", CarrierID, IsPercentage, Options);
        }

        public override string GetRouteOptionsIdentifier()
        {
            return Options;
        }
    }
}