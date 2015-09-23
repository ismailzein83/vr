using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CodePreparation.Data
{
   public interface ICodePreparationDataManager:IDataManager
    {
       bool UploadSaleZonesList(int SaleZonePackageId, int fileId);
    }
}
