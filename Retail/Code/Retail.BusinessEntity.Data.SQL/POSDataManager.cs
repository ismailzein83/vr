using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retail.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Data.SQL;

namespace Retail.BusinessEntity.Data.SQL
{
    public class POSDataManager : BaseSQLDataManager, IPOSDataManager
    {
        #region Constructors
        public POSDataManager()
            : base(GetConnectionStringName("Retail_BE_DBConnStringKey", "RetailDBConnString"))
        {

        }
        #endregion

        #region Public Methods
        public IEnumerable<PointOfSale> GetPointOfSales()
        {
            return GetItemsSP("Retail.sp_POS_GetAll", PosMapper);
        }

        public bool Insert(PointOfSale pos, out long insertedId)
        {
            string serializedSettings = pos.Settings != null ? Serializer.Serialize(pos.Settings) : null;
            object agentId;
            int affectedRecords = ExecuteNonQuerySP("Retail.sp_POS_Insert", out agentId, pos.Name, pos.Type, serializedSettings, pos.SourceId);

            if (affectedRecords > 0)
            {
                insertedId = (int)agentId;
                return true;
            }
            insertedId = -1;
            return false;
        }

        public bool Update(PointOfSale pos)
        {
            string serializedSettings = pos.Settings != null ? Serializer.Serialize(pos.Settings) : null;
            int affectedRecords = ExecuteNonQuerySP("Retail.sp_POS_Update", pos.Id, pos.Name, pos.Type, serializedSettings, pos.SourceId);
            return (affectedRecords > 0);
        }

        public bool ArePOSsUpdated(ref object updateHandle)
        {
            return IsDataUpdated("Retail.POS", ref updateHandle);
        }

        #endregion

        #region Mapper
        PointOfSale PosMapper(IDataReader reader)
        {
            return new PointOfSale
            {
                Id = (long)reader["ID"],
                Name = reader["Name"] as string,
                Type = reader["Type"] as string,
                Settings = Serializer.Deserialize<POSSetting>(reader["Settings"] as string),
                SourceId = reader["SourceID"] as string
            };
        }

        #endregion
    }
}
