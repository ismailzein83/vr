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
                if (entity.BeginEffectiveDate <= date.Value && (!entity.EndEffectiveDate.HasValue || entity.EndEffectiveDate.Value > date.Value))
                    return true;
                else
                    return false;
            }
            else
                if (futureEntities)
                    if (!entity.EndEffectiveDate.HasValue || entity.BeginEffectiveDate > DateTime.Now)
                        return true;

            return false;
        }
    }
}
