﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Entities;
using Vanrise.Data;

namespace TOne.WhS.SupplierPriceList.Data
{
    public interface ISupplierZonePreviewDataManager : IDataManager, IBulkApplyDataManager<ZoneRatePreview>
    {
        long ProcessInstanceId { set; }

        void ApplyPreviewZonesToDB(object preparedZones); 
        
        IEnumerable<ZoneRatePreviewDetail> GetFilteredZonePreview(SPLPreviewQuery query);
    }
}
