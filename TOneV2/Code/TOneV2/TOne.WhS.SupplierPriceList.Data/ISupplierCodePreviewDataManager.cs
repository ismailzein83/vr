﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Entities;
using Vanrise.Data;

namespace TOne.WhS.SupplierPriceList.Data
{
    public interface ISupplierCodePreviewDataManager : IDataManager, IBulkApplyDataManager<CodePreview>
    {
        long ProcessInstanceId { set; }

        void ApplyPreviewCodesToDB(object preparedCodes); 
        
        Vanrise.Entities.BigResult<Entities.CodePreview> GetCodePreviewFilteredFromTemp(Vanrise.Entities.DataRetrievalInput<Entities.SPLPreviewQuery> input);
    }
}
