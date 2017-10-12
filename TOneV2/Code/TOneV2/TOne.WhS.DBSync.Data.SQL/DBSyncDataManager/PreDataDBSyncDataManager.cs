using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Entities;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class PreDataDBSyncDataManager : BaseSQLDataManager, IDBSyncDataManager
    {

        public PreDataDBSyncDataManager() :
            base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
        {

        }

        public List<VRFile> GetExistingFiles(List<long> fileIds)
        {
            return GetItemsText(string.Format(query_GetFiles, string.Join(",", fileIds)), VRFileMapper, null);
        }

        VRFile VRFileMapper(IDataReader reader)
        {
            return new VRFile
            {
                FileId = GetReaderValue<long>(reader, "ID"),
                Name = reader["Name"] as string,
                Extension = reader["Extension"] as string,
                Content = GetReaderValue<byte[]>(reader, "Content"),
                IsUsed = GetReaderValue<bool>(reader, "IsUsed"),
                ModuleName = reader["ModuleName"] as string,
                UserId = reader["UserID"] != DBNull.Value ? (int)reader["UserID"] : default(int?),
                CreatedTime = GetReaderValue<DateTime>(reader, "CreatedTime"),
            };
        }

        public string GetConnection()
        {
            return base.GetConnectionString();
        }

        public string GetSchema()
        {
            return "";
        }

        const string query_GetFiles = @"Select * from [common].[File] where ID in ({0})";
    }
}
