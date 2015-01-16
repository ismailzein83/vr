using System.Collections.Generic;
using System.Data;

namespace TABS.Components.Routing
{
    public class Route
    {
        public int ID;
        public CarrierAccount Customer;
        public string Code;
        public float OurRate;
        public List<RouteOption> Options;

        public static void ReadRoutes(IDataReader routeReader, List<Route> routeList)
        {
            while (routeReader.Read())
            {
                Route route = new Route();
                route.Options = new List<RouteOption>();
                int index = -1;
                index++; route.ID = routeReader.GetInt32(index);
                index++; route.Customer = CarrierAccount.All[routeReader.GetString(index)];
                index++; route.Code = routeReader.GetString(index);
                index++; route.OurRate = routeReader.GetFloat(index);
                routeList.Add(route);
            }
        }

        public static void ReadOptions(IDataReader routeOptionsReader, Dictionary<int, Route> routes)
        {
            while (routeOptionsReader.Read())
            {
                int index = 0;
                int routeID = routeOptionsReader.GetInt32(index);
                Route route = null;
                if (routes.TryGetValue(routeID, out route))
                {
                    RouteOption outOption = new RouteOption();
                    index++; outOption.Supplier = CarrierAccount.All[routeOptionsReader.GetString(index)];
                    // Customer cannot be own supplier, to prevent looping
                    if (outOption.Supplier.Equals(route.Customer)) continue;
                    index++; if (!routeOptionsReader.IsDBNull(index)) outOption.Priority = routeOptionsReader.GetByte(index);
                    index++; if (!routeOptionsReader.IsDBNull(index)) outOption.NumberOfTries = routeOptionsReader.GetByte(index);
                    index++; if (!routeOptionsReader.IsDBNull(index)) outOption.Rate = routeOptionsReader.GetFloat(index);
                    if (outOption.NumberOfTries == 0) outOption.NumberOfTries = 1;
                    else outOption.IsValid = true;
                    index++;
                    if (!routeOptionsReader.IsDBNull(index))
                        outOption.Percentage = routeOptionsReader.GetByte(index);
                    else
                        outOption.Percentage = 0;
                    route.Options.Add(outOption);
                }
            }
        }

        public override bool Equals(object obj)
        {
            Route other = obj as Route;
            if (
                    other == null
                    ||
                    !other.Code.Equals(this.Code)
                    ||
                    !other.Customer.Equals(this.Customer))
                return false;
            else
                return true;
        }

        public override int GetHashCode()
        {
            return ID;
        }
    }
}