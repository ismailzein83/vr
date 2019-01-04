using System;

namespace TOne.WhS.Routing.Entities
{
    public class BaseRuleCriteriaPriority
    {
        public static Guid s_CodeRuleBehavior = new Guid("EE9CF252-6DDA-4939-9D8A-8B5896EE48B5");
        public static Guid s_DealRuleBehavior = new Guid("568AA291-1746-4511-B53E-561AAC1B5B1F");
        public static Guid s_ZoneRuleBehavior = new Guid("B584B526-75A4-4611-93AB-883AD4933F7F");
        public static Guid s_CountryRuleBehavior = new Guid("47172F1E-7540-4D38-92EA-F32B060B7AC3");
        public static Guid s_CustomerRuleBehavior = new Guid("DE33E029-991E-4D72-B0E9-01EBC6D8AE73");
        public static Guid s_RoutingProductRuleBehavior = new Guid("0A634C6B-391B-4A6B-AC12-BE4590CE293F");
        public static Guid s_SupplierZoneRuleBehavior = new Guid("311DBDFD-4DCA-437A-A07F-216DD47A5DCD");
        public static Guid s_SupplierRuleBehavior = new Guid("437979DD-03D1-445E-98C7-75F0E9E15C6A");

        public Guid Id { get; set; }

        public string Name { get; set; }
    }

    public class RouteRuleCriteriaPriority : BaseRuleCriteriaPriority
    {

    }

    public class RouteOptionRuleCriteriaPriority : BaseRuleCriteriaPriority
    {

    }
}