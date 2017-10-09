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
            int id = ExecuteNonQuerySP("[common].[sp_File_Insert]", out fileId, file.Name, file.Extension, file.Content, file.ModuleName, file.UserId, file.CreatedTime);
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
        public bool UpdateFileUsed(long fileId, bool isUsed)
        {
            int recordesEffected = ExecuteNonQuerySP("[common].[sp_File_GetFileById]", fileId, isUsed);
            return (recordesEffected > 0);
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

        VRFileInfo FileInfoMapper(IDataReader reader)
        {
            return new VRFileInfo
            {
                FileId = GetReaderValue<long>(reader, "ID"),
                Name = reader["Name"] as string,
                Extension = reader["Extension"] as string,
                IsUsed = GetReaderValue<bool>(reader, "IsUsed"),
                ModuleName = reader["ModuleName"] as string,
                UserId = reader["UserID"] != DBNull.Value ? (int)reader["UserID"] : default(int?),
                CreatedTime = GetReaderValue<DateTime>(reader, "CreatedTime"),
            };
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
