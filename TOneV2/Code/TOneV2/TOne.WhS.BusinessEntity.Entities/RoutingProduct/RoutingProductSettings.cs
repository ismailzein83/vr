using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public enum RoutingProductZoneRelationType { AllZones, SpecificZones }
    public enum RoutingProductSupplierRelationType { AllSuppliers, SpecificSuppliers }

    public class RoutingProductSettings
    {
        object obj = new object();
        Dictionary<long, HashSet<int>> _zoneServicesDict;

        public HashSet<int> DefaultServiceIds { get; set; }

        public RoutingProductZoneRelationType ZoneRelationType { get; set; }

        public List<RoutingProductZone> Zones { get; set; }

        public List<RoutingProductZoneService> ZoneServices { get; set; }

        public RoutingProductSupplierRelationType SupplierRelationType { get; set; }

        public List<RoutingProductSupplier> Suppliers { get; set; }


        public HashSet<int> GetZoneServices(long saleZoneId)
        {
            if (_zoneServicesDict == null && ZoneServices != null && ZoneServices.Count > 0)
            {
                lock (obj)
                {
                    _zoneServicesDict = new Dictionary<long, HashSet<int>>();
                    HashSet<int> temp;

                    foreach (var routingProductZoneService in ZoneServices)
                    {
                        foreach (var zoneId in routingProductZoneService.ZoneIds)
                        {
                            if (!_zoneServicesDict.TryGetValue(saleZoneId, out temp))
                                _zoneServicesDict.Add(zoneId, routingProductZoneService.ServiceIds);
                        }
                    }
                }
            }

            HashSet<int> zoneServices;
            if (_zoneServicesDict == null || !_zoneServicesDict.TryGetValue(saleZoneId, out zoneServices))
                zoneServices = DefaultServiceIds;

            if (zoneServices == null)
                throw new NullReferenceException("zoneServices");

            return zoneServices;
        }
    }

    public class RoutingProductZone
    {
        public long ZoneId { get; set; }
    }

    public class RoutingProductZoneService
    {
        public List<long> ZoneIds { get; set; }

        public HashSet<int> ServiceIds { get; set; }
    }

    public class RoutingProductSupplier
    {
        public int SupplierId { get; set; }
    }
}
