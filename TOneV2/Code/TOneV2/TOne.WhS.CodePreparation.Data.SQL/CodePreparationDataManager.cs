using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;

namespace TOne.WhS.CodePreparation.Data.SQL
{
    public class CodePreparationDataManager : BaseTOneDataManager, ICodePreparationDataManager
    {
        public CodePreparationDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

        public bool UploadSaleZonesList(int SellingNumberPlanId, int fileId)
        {
            throw new NotImplementedException();
        }
    }
}
