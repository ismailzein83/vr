using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
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
            var file = new VRFile
            {
                Content = GetReaderValue<byte[]>(reader, "Content")
            };
            file.FileId = GetReaderValue<long>(reader, "ID");
            file.Name = reader["Name"] as string;
            file.Extension = reader["Extension"] as string;
            file.ModuleName = reader["ModuleName"] as string;
            file.UserId = reader["UserID"] != DBNull.Value ? (int)reader["UserID"] : default(int?);
            file.CreatedTime = GetReaderValue<DateTime>(reader, "CreatedTime");
            file.IsTemp = GetReaderValue<bool>(reader, "IsTemp");
            string settingsAsString = reader["Settings"] as string;
            if (settingsAsString != null)
                file.Settings = Serializer.Deserialize<VRFileSettings>(settingsAsString);
            return file;
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
