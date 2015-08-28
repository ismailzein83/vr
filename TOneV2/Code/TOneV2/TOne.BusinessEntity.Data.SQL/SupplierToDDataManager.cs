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
    public class SupplierToDDataManager : ToDConsiderationDataManager , ISupplierTODDataManager
    {
       
        public Vanrise.Entities.BigResult<SupplierTODConsiderationInfo> GetSupplierToDConsiderationByCriteria(Vanrise.Entities.DataRetrievalInput<TODQuery> input , List<string> suppliersAMUids )
        {


            return RetrieveData(input, (tempTableName) =>
            {
                string zoneIds = null;
                string suppliersIds = null;
                if (input.Query.ZoneIds.Count() > 0)
                    zoneIds = string.Join<int>(",", input.Query.ZoneIds);
                if (suppliersAMUids.Count() > 0)
                    suppliersIds = string.Join<string>(",", suppliersAMUids);
                ExecuteNonQuerySP("BEntity.sp_SupplierToDConsideration_CreateTempForFiltered", tempTableName, zoneIds, suppliersIds, input.Query.SupplierId, input.Query.EffectiveOn);

            }, (reader) => SupplierToDConsiderationInfoMapper(reader, input.Query.EffectiveOn));
        }

        private SupplierTODConsiderationInfo SupplierToDConsiderationInfoMapper(IDataReader reader, DateTime effectiveOn)
        {
            SupplierTODConsiderationInfo tod = base.ToDConsiderationInfoMapper<SupplierTODConsiderationInfo>(reader, effectiveOn);
            tod.UserName = reader["UserName"] as string;
            tod.WeekDayName = reader["WeekDayName"] as string;
            tod.CustomerName = reader["CustomerName"] as string;
            return tod;
        }
    }
}
