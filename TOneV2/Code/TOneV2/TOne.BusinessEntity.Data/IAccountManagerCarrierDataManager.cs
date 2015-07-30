﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;

namespace TOne.BusinessEntity.Data
{
    public interface IAccountManagerCarrierDataManager : IDataManager
    {
        Vanrise.Entities.BigResult<AccountManagerCarrier> GetCarriers(Vanrise.Entities.DataRetrievalInput<AccountManagerCarrierQuery> input);
    }
}
