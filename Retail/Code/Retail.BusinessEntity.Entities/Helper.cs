using System;
using System.Collections.Generic;
using System.Text;

namespace Retail.BusinessEntity.Entities
{
    public static class Helper
    {
        public static string SerializePackageCombinations(Dictionary<int, List<Guid>> packageItemsByPackageId)
        {
            if (packageItemsByPackageId == null || packageItemsByPackageId.Count == 0)
                return null;

            List<string> serializedPackageItemsByPackageId = new List<string>();

            foreach (var packageItemKvp in packageItemsByPackageId)
            {
                int packageId = packageItemKvp.Key;
                List<Guid> packageItems = packageItemKvp.Value;

                StringBuilder sb = new StringBuilder();
                sb.Append(packageId + ":");
                sb.Append(string.Join("|", packageItems));
                serializedPackageItemsByPackageId.Add(sb.ToString());
            }

            return string.Join("&", serializedPackageItemsByPackageId);
        }

        public static Dictionary<int, List<Guid>> DeserializePackageCombinations(string packageCombinations)
        {
            if (string.IsNullOrEmpty(packageCombinations))
                return null;

            Dictionary<int, List<Guid>> packageCombinationsByPackageCombinationId = new Dictionary<int, List<Guid>>();

            var serializedPackageItemsByPackageId = packageCombinations.Split('&');
            foreach (var packageItemsByPackageId in serializedPackageItemsByPackageId)
            {
                var packageItemsByPackageIdAsArray = packageItemsByPackageId.Split(':');
                var packageId = Convert.ToInt32(packageItemsByPackageIdAsArray[0]);
                var packageItemIdsAsArray = packageItemsByPackageIdAsArray[1].Split('|');

                List<Guid> packageItemIds = new List<Guid>();
                foreach (var packageItemId in packageItemIdsAsArray)
                    packageItemIds.Add(Guid.Parse(packageItemId));

                packageCombinationsByPackageCombinationId.Add(packageId, packageItemIds);
            }
            return packageCombinationsByPackageCombinationId;
        }
    }
}