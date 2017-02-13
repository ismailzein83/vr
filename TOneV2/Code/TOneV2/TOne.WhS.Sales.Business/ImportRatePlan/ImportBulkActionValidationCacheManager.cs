using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Business
{
    public class ImportBulkActionValidationCacheManager : Vanrise.Caching.BaseCacheManager
    {
        protected override bool IsTimeExpirable
        {
            get
            {
                return true;
            }
        }
    }
}
