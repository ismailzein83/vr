using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Entities;
using Vanrise.Common;
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
            Guid? configId = null;
            string settingAsString = null;
            if (file.Settings != null)
            {
                settingAsString = Serializer.Serialize(file.Settings);
                if (file.Settings.ExtendedSettings != null)
                    configId = file.Settings.ExtendedSettings.ConfigId;
            }
            int id = ExecuteNonQuerySP("[common].[sp_File_Insert]", out fileId, file.Name, file.Extension, file.Content, file.ModuleName, file.UserId, file.IsTemp, configId, settingAsString, ToDBNullIfDefault(file.CreatedTime));
            return (id > 0) ? (long)fileId : badresult;
        }
        public bool SetFileUsedAndUpdateSettings(long fileId, VRFileSettings fileSettings)
        {
            Guid? configId = null;
            string settingAsString = null;
            if (fileSettings != null)
            {
                settingAsString = Serializer.Serialize(fileSettings);
                if (fileSettings.ExtendedSettings != null)
                    configId = fileSettings.ExtendedSettings.ConfigId;
            }
            return ExecuteNonQueryText(string.Format(query_SetUsedAndUpdateSettings, _UseTempTables ? "File_Temp" : "File"), (cmd) =>
            {
                cmd.Parameters.Add(new SqlParameter("@Id", fileId));

                if (configId.HasValue)
                    cmd.Parameters.Add(new SqlParameter("@ConfigID", configId));
                else
                    cmd.Parameters.Add(new SqlParameter("@ConfigID", DBNull.Value));

                if (!string.IsNullOrEmpty(settingAsString))
                    cmd.Parameters.Add(new SqlParameter("@Settings", settingAsString));
                else
                    cmd.Parameters.Add(new SqlParameter("@Settings", DBNull.Value));

            }) > 0;
        }
        public long AddFileToTemp(VRFile file)
        {
            object fileId;
            long badresult = -1;
            Guid? configId = null;
            string settingAsString = null;
            if (file.Settings != null)
            {
                settingAsString = Serializer.Serialize(file.Settings);
                if (file.Settings.ExtendedSettings != null)
                    configId = file.Settings.ExtendedSettings.ConfigId;
            }
            int id = ExecuteNonQuerySP("[common].[sp_File_Insert_Temp]", out fileId, file.Name, file.Extension, file.Content, file.ModuleName, file.UserId, file.IsTemp, configId, settingAsString, ToDBNullIfDefault(file.CreatedTime));
            return (id > 0) ? (long)fileId : badresult;
        }

        public void InsertFiles(List<VRFile> files)
        {
            StringBuilder query = new StringBuilder();

            foreach (var file in files)
            {
                ExecuteNonQueryText(string.Format(query_InsertFileWithIdentityOff, MigrationUtils.GetTableName(_Schema, _TableName, _UseTempTables)), (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@ID", file.FileId));
                    cmd.Parameters.Add(new SqlParameter("@Name", file.Name));
                    cmd.Parameters.Add(new SqlParameter("@Extension", file.Extension));
                    cmd.Parameters.Add(new SqlParameter("@Content", file.Content));

                    if (string.IsNullOrEmpty(file.ModuleName))
                        cmd.Parameters.Add(new SqlParameter("@ModuleName", DBNull.Value));
                    else
                        cmd.Parameters.Add(new SqlParameter("@ModuleName", file.ModuleName));

                    if (file.UserId.HasValue)
                        cmd.Parameters.Add(new SqlParameter("@UserID", file.UserId));
                    else
                        cmd.Parameters.Add(new SqlParameter("@UserID", DBNull.Value));

                    cmd.Parameters.Add(new SqlParameter("@IsTemp", file.IsTemp));
                    Guid? configId = null;
                    string settingAsString = null;
                    if (file.Settings != null)
                    {
                        settingAsString = Serializer.Serialize(file.Settings);
                        if (file.Settings.ExtendedSettings != null)
                            configId = file.Settings.ExtendedSettings.ConfigId;
                    }
                    cmd.Parameters.Add(new SqlParameter("@ConfigID", configId.HasValue ? (Object)configId.Value : DBNull.Value));
                    cmd.Parameters.Add(new SqlParameter("@Settings", settingAsString != null ? (Object)settingAsString : DBNull.Value));
                    cmd.Parameters.Add(new SqlParameter("@CreatedTime", file.CreatedTime));
                });
            }
        }

        const string query_InsertFileWithIdentityOff = @"set identity_insert {0} on;
	Insert into common.[File_Temp] (ID, [Name], [Extension], [Content], [ModuleName],[UserID], IsTemp, ConfigID, Settings, [CreatedTime])
	values(@ID, @Name, @Extension, @Content, @ModuleName,@UserID, @IsTemp, @ConfigID, @Settings, @CreatedTime)


set identity_insert [common].[file_Temp] off;";

        const string query_SetUsedAndUpdateSettings = @"UPDATE	[common].{0}
		SET		[IsTemp] = 0,
				ConfigID = @ConfigID,
				Settings = @Settings
		WHERE	ID = @Id";

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
