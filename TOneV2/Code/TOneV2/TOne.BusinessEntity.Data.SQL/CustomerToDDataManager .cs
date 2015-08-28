using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;
using TOne.Data.SQL;

namespace TOne.BusinessEntity.Data.SQL
{
    public class CustomerToDDataManager : ToDConsiderationDataManager ,ICustomerTODDataManager
    {
       
        public Vanrise.Entities.BigResult<CustomerTODConsiderationInfo> GetCustomerToDConsiderationByCriteria(Vanrise.Entities.DataRetrievalInput<TODQuery> input )
        {


            return RetrieveData(input, (tempTableName) =>
            {
                string zoneIds = null;
                if (input.Query.ZoneIds != null && input.Query.ZoneIds.Count() > 0)
                    zoneIds = string.Join<int>(",", input.Query.ZoneIds);
                ExecuteNonQuerySP("BEntity.sp_CustomersToDConsideration_CreateTempForFiltered", tempTableName, zoneIds, input.Query.CustomerId, input.Query.EffectiveOn);

            }, (reader) => CustomerTODConsiderationInfoMapper(reader, input.Query.EffectiveOn));
        }


        private CustomerTODConsiderationInfo CustomerTODConsiderationInfoMapper(IDataReader reader, DateTime effectiveOn)
        {
            CustomerTODConsiderationInfo tod = base.ToDConsiderationInfoMapper<CustomerTODConsiderationInfo>(reader, effectiveOn);
            tod.CustomerNameSuffix = reader["CustomerNameSuffix"] as string;
            return tod;
        }
    }
}
