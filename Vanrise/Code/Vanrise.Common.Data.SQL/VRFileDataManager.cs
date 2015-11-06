using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Data.SQL
{
    public class VRFileDataManager : Vanrise.Data.SQL.BaseSQLDataManager, IVRFileDataManager
    {
        public VRFileDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
        {

        }
        public long AddFile(VRFile file)
        {
            object fileId;
            long badresult = -1;
            int id = ExecuteNonQuerySP("[common].[sp_File_Insert]", out fileId, file.Name, file.Extension , file.Content ,file.CreatedTime);
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

        public bool UpdateFileUsed(long fileId, bool isUsed)
        {
            int recordesEffected = ExecuteNonQuerySP("[common].[sp_File_GetFileById]", fileId, isUsed);
            return (recordesEffected > 0) ;
        }
        private VRFileInfo FileInfoMapper(IDataReader reader)
        {
            return new VRFileInfo
            {
                Name = reader["Name"] as string,
                FileId = GetReaderValue<long>(reader, "ID"),
                CreatedTime = GetReaderValue<DateTime>(reader, "CreatedTime"),
                Extension = reader["Extension"] as string,
                IsUsed = GetReaderValue<bool>(reader, "IsUsed")

            };
        }
        private VRFile FileMapper(IDataReader reader)
        {
            return new VRFile
            {
                Name = reader["Name"] as string,
                FileId = GetReaderValue<long>(reader, "ID"),
                CreatedTime = GetReaderValue<DateTime>(reader, "CreatedTime"),
                Extension = reader["Extension"] as string,
                IsUsed = GetReaderValue<bool>(reader, "IsUsed"),
                Content = GetReaderValue<byte[]>(reader, "Content")
            };
        }

    }
}
