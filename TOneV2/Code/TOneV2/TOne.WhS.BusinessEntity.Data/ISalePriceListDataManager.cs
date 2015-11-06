﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data
{
   public interface ISalePriceListDataManager:IDataManager
    {
       List<SalePriceList> GetPriceLists();
       bool ArGetSalePriceListsUpdated(ref object updateHandle);
    }
}
