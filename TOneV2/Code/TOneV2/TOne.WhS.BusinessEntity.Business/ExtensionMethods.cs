using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public static class ExtensionMethods
    {
        public static bool IsEffective(this IBusinessEntity entity, DateTime? date, bool futureEntities)
        {
            if (date.HasValue)
            {
                if (entity.BED <= date.Value && (!entity.EED.HasValue || entity.EED.Value > date.Value))
                    return true;
                else
                    return false;
            }
            else
                if (futureEntities)
                    if (!entity.EED.HasValue || entity.BED > DateTime.Now)
                        return true;

            return false;
        }

        public static bool IsEffective(this IBusinessEntity entity, DateTime? date)
        {
            return IsEffective(entity, date, false);
        }
    }
}
