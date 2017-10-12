using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;
using Vanrise.Entities;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class FileDBSyncDataManager : BaseSQLDataManager, IDBSyncDataManager
    {
        string _TableName = "File";
        string _Schema = "Common";
        bool _UseTempTables;
        public FileDBSyncDataManager(bool useTempTables) :
            base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
        {
            _UseTempTables = useTempTables;
        }

        public long ApplyFile(VRFile file)
        {
            return _UseTempTables ? AddFileToTemp(file) : AddFile(file);
        }

        public long AddFile(VRFile file)
        {
            object fileId;
            long badresult = -1;
            int id = ExecuteNonQuerySP("[common].[sp_File_Insert]", out fileId, file.Name, file.Extension, file.Content, file.ModuleName, file.UserId, file.CreatedTime);
            return (id > 0) ? (long)fileId : badresult;
        }

        public long AddFileToTemp(VRFile file)
        {
            object fileId;
            long badresult = -1;
            int id = ExecuteNonQuerySP("[common].[sp_File_Insert_Temp]", out fileId, file.Name, file.Extension, file.Content, file.ModuleName, file.UserId, file.CreatedTime);
            return (id > 0) ? (long)fileId : badresult;
        }

        public void InsertFiles(List<VRFile> files)
        {
            StringBuilder query = new StringBuilder();

            foreach (var file in files)
            {
                ExecuteNonQuerySP("[common].[sp_File_InsertWithIdentityOff]", _UseTempTables, 
                                                                                file.FileId, 
                                                                                file.Name, 
                                                                                file.Extension, 
                                                                                file.Content, 
                                                                                file.IsUsed, 
                                                                                file.ModuleName, 
                                                                                file.UserId, 
                                                                                file.CreatedTime);
            }
        }

        public string GetConnection()
        {
            return base.GetConnectionString();
        }

        public string GetTableName()
        {
            return _TableName;
        }

        public string GetSchema()
        {
            return _Schema;
        }
    }
}
