﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data
{
    public interface IAccountManagerCarrierDataManager : IDataManager
    {
        IEnumerable<AssignedCarrier> GetAssignedCarriers();

        bool AreAccountManagerUpdated(ref object updateHandle);
        bool AssignCarriers(UpdatedAccountManagerCarrier[] updatedCarriers);
    }
}
