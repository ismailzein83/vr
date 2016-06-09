using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Entities;
using Vanrise.Data;

namespace TOne.WhS.SupplierPriceList.Data
{
    public interface ISupplierCountryPreviewDataManager : IDataManager
    {
        long ProcessInstanceId { set; }
        Vanrise.Entities.BigResult<Entities.CountryPreview> GetCountryPreviewFilteredFromTemp(Vanrise.Entities.DataRetrievalInput<Entities.SPLPreviewQuery> input);
    }
}
