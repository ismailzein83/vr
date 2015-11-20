using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities;

namespace TOne.WhS.CodePreparation.Data
{
   public interface ICodePreparationDataManager:IDataManager
    {

       void InsertCodePreparationObject(Dictionary<string, Zone> saleZones, int sellingNumberPlanId);
    }
}
