using System;

namespace TOne.WhS.RouteSync.Cataleya
{
    public static class Helper
    {
        public static string BuildRouteTableName(int carrierId, int version)
        {
            return $"RL_{carrierId}_{version}";
        }
    }
}