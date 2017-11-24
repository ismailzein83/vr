using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Data.SQL
{
    public class VRFileDataManager : Vanrise.Data.SQL.BaseSQLDataManager, IVRFileDataManager
    {
        #region Constructors / Fields

        static Dictionary<string, string> _mappers;

        static VRFileDataManager()
        {
            _mappers = new Dictionary<string, string>();
            _mappers.Add("FileId", "ID");
        }

        public VRFileDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
        {

        }

        #endregion

        #region Public Methods

        public long AddFile(VRFile file)
        {
            object fileId;
            long badresult = -1;
            Guid? configId = null;
            string settingAsString = null;
            if(file.Settings != null)
            {
                settingAsString = Serializer.Serialize(file.Settings);
                if (file.Settings.ExtendedSettings != null)
                    configId = file.Settings.ExtendedSettings.ConfigId;
            }
            int id = ExecuteNonQuerySP("[common].[sp_File_Insert]", out fileId, file.Name, file.Extension, file.Content, file.ModuleName, file.UserId, file.IsTemp, configId, settingAsString, ToDBNullIfDefault(file.CreatedTime));
            return (id > 0) ? (long)fileId : badresult;
        }

        public VRFileInfo GetFileInfo(long fileId)
        {
            return GetItemSP("[common].[sp_File_GetInfoById]", FileInfoMapper, fileId);
        }

        public VRFile GetFile(long fileId)
        {
            return GetItemSP("[common].[sp_File_GetFileById]", FileMapper, fileId);
        }
        public List<VRFileInfo> GetFilesInfo(IEnumerable<long> fileIds)
        {
            string fileIdsAsString = null;
            if (fileIds != null)
                fileIdsAsString = string.Join<long>(",", fileIds);
            return GetItemsSP("[common].[sp_File_GetByFileIds]", FileInfoMapper, fileIdsAsString);
        }

        public bool SetFileUsed(long fileId)
        {
            return (ExecuteNonQuerySP("[common].[sp_File_SetUsed]", fileId)>0);
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
            return (ExecuteNonQuerySP("[common].[sp_File_SetUsedAndUpdateSettings]", fileId, configId, settingAsString)>0);
        }

        public Vanrise.Entities.BigResult<VRFileInfo> GetFilteredRecentFiles(Vanrise.Entities.DataRetrievalInput<VRFileQuery> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {
                CreateTempTableIfNotExists(input, tempTableName);
            };
            return RetrieveData(input, createTempTableAction, FileInfoMapper, _mappers);
        }

        #endregion

        #region Private Methods

        void CreateTempTableIfNotExists(Vanrise.Entities.DataRetrievalInput<VRFileQuery> input, string tempTableName)
        {
            StringBuilder tempTableQueryBuilder = new StringBuilder
            (
                @"IF NOT OBJECT_ID ('#TEMPTABLENAME#', N'U') IS NOT NULL
                BEGIN 
                    SELECT [ID],
                        [Name],
                        [Extension],
                        [IsUsed],
                        [ModuleName],
                        [UserId],
                        IsTemp, 
                        ConfigID, 
                        Settings,
                        [CreatedTime]
                    INTO #TEMPTABLENAME#
                    FROM [common].[File]
                    #WHERECLAUSE#
                    ORDER BY [CreatedTime] DESC
                END"
            );
            tempTableQueryBuilder.Replace("#TEMPTABLENAME#", tempTableName);
            tempTableQueryBuilder.Replace("#WHERECLAUSE#", (input.Query.ModuleName != null) ? String.Format("WHERE [ModuleName] = '{0}' AND UserID = {1}", input.Query.ModuleName, input.Query.UserId) : null);
            ExecuteNonQueryText(tempTableQueryBuilder.ToString(), null);
        }

        VRFile FileMapper(IDataReader reader)
        {
            var file = new VRFile
            {
                Content = GetReaderValue<byte[]>(reader, "Content")                
            };
            FillFileInfoFromReader(file, reader);
            return file;
        }

        VRFileInfo FileInfoMapper(IDataReader reader)
        {
            var fileInfo =new VRFileInfo();
            FillFileInfoFromReader(fileInfo, reader);
            return fileInfo;
        }

        void FillFileInfoFromReader(VRFileInfo fileInfo, IDataReader reader)
        {
            fileInfo.FileId = GetReaderValue<long>(reader, "ID");
            fileInfo.Name = reader["Name"] as string;
            fileInfo.Extension = reader["Extension"] as string;
            fileInfo.ModuleName = reader["ModuleName"] as string;
            fileInfo.UserId = reader["UserID"] != DBNull.Value ? (int)reader["UserID"] : default(int?);
            fileInfo.CreatedTime = GetReaderValue<DateTime>(reader, "CreatedTime");
            fileInfo.IsTemp = GetReaderValue<bool>(reader, "IsTemp");
            string settingsAsString = reader["Settings"] as string;
            if (settingsAsString != null)
                fileInfo.Settings = Serializer.Deserialize<VRFileSettings>(settingsAsString);
        }

        #endregion

        string _moduleName;
        public string ModuleName
        {
            set { this._moduleName = value; }
        }

        protected override string GetConnectionString()
        {
            if(!String.IsNullOrWhiteSpace(this._moduleName))
            {
                var connectionStringKey = ConfigurationManager.AppSettings[String.Format("VRFile_{0}_DBConnStringKey", this._moduleName)];
                if (!string.IsNullOrEmpty(connectionStringKey))
                {
                    var connectionString = ConfigurationManager.ConnectionStrings[connectionStringKey];
                    if(connectionString == null)
                        throw new NullReferenceException(String.Format("connectionString '{0}'", connectionStringKey));
                    return connectionString.ConnectionString;
                }
            }
            return base.GetConnectionString();
        }

    }
}
