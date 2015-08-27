﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;

namespace TOne.BusinessEntity.Data
{
    public interface IBaseTODDataManager : IDataManager
    {
        Vanrise.Entities.BigResult<T> GetTODinfos<T>(Vanrise.Entities.DataRetrievalInput<TODQuery> input) where T : BaseTODConsiderationInfo;
    }
}
