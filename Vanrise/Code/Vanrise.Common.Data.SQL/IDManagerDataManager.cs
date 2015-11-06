using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Common.Data.SQL
{
    public class IDManagerDataManager : Vanrise.Data.SQL.BaseSQLDataManager, IIDManagerDataManager
    {
        public IDManagerDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
        {

        }

        public void ReserveIDRange(int typeId, int nbOfIds, out long startingId)
        {
            startingId = (long)ExecuteScalarSP("[common].[sp_IDManager_ReserveIDRange]", typeId, nbOfIds);
        }
    }
}
