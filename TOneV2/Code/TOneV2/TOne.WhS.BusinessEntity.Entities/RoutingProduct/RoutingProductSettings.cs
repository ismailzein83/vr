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
                            if (!_zoneServicesDict.TryGetValue(zoneId, out temp))
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

        public bool CheckIfZoneServicesAreSame(RoutingProductSettings routingProductSetting)
        {
            if (this.ZoneServices == null && routingProductSetting.ZoneServices == null)
                return true;
            else if (this.ZoneServices == null || routingProductSetting.ZoneServices == null)
                return false;
            return this.ZoneServices.SequenceEqual(routingProductSetting.ZoneServices);
        }

        public bool CheckIfZonesAreSame(RoutingProductSettings routingProductSetting)
        {
            if (this.Zones == null && routingProductSetting.Zones == null)
                return true;
            else if (this.Zones == null || routingProductSetting.Zones == null)
                return false;
            return this.Zones.SequenceEqual(routingProductSetting.Zones);
        }

        public bool CheckIfDefaultServiceIdsAreSame(RoutingProductSettings routingProductSetting)
        {
            if (this.DefaultServiceIds == null && routingProductSetting.DefaultServiceIds == null)
                return true;
            else if (this.DefaultServiceIds == null || routingProductSetting.DefaultServiceIds == null)
                return false;
            return this.DefaultServiceIds.SetEquals(routingProductSetting.DefaultServiceIds);
        }

    }

    public class RoutingProductZone : IEquatable<RoutingProductZone>
    {
        public long ZoneId { get; set; }

        public override bool Equals(Object obj)
        {
            return this.Equals(obj as RoutingProductZone);
        }
        public bool Equals(RoutingProductZone routingProductZone)
        {
            if (routingProductZone == null)
                return false;
            return (this.ZoneId==routingProductZone.ZoneId);
        }
    }

    public class RoutingProductZoneService : IEquatable<RoutingProductZoneService>
    {
        public List<long> ZoneIds { get; set; }

        public HashSet<int> ServiceIds { get; set; }

        public override bool Equals(Object obj)
        {
            return this.Equals(obj as RoutingProductZoneService);
        }
        public bool Equals(RoutingProductZoneService routingProductZoneService)
        {
            if (routingProductZoneService == null)
                return false;
            return (CheckIfZoneIdsAreSame(routingProductZoneService) && CheckIfServiceIdsAreSame(routingProductZoneService));
        }
        public bool CheckIfZoneIdsAreSame(RoutingProductZoneService routingProductZoneService)
        {
            if (this.ZoneIds == null && routingProductZoneService.ZoneIds == null)
                return true;
            else if (this.ZoneIds == null || routingProductZoneService.ZoneIds == null)
                return false;
            return this.ZoneIds.SequenceEqual(routingProductZoneService.ZoneIds);
        }

        public bool CheckIfServiceIdsAreSame(RoutingProductZoneService routingProductZoneService)
        {
            if (this.ServiceIds == null && routingProductZoneService.ServiceIds == null)
                return true;
            else if (this.ServiceIds == null || routingProductZoneService.ServiceIds == null)
                return false;
            return this.ServiceIds.SetEquals(routingProductZoneService.ServiceIds);
        }

    }

    public class RoutingProductSupplier
    {
        public int SupplierId { get; set; }
    }
}
