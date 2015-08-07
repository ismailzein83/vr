using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;
using Vanrise.Entities;

namespace Vanrise.Common
{
    public class VRFileManager
    {
        private readonly IVRFileDataManager _datamanager;

        public VRFileManager()
        {
            _datamanager =  CommonDataManagerFactory.GetDataManager<IVRFileDataManager>();
        }
        public long AddFile(VRFile file)
        {
            return _datamanager.AddFile(file);
        }

        public bool SetFileUsed(long fileId)
        {
            return _datamanager.UpdateFileUsed(fileId , true);
        }

        public bool SetFileUnUsed(long fileId)
        {
            return _datamanager.UpdateFileUsed(fileId, false);
        }
        public VRFile GetFile(long fileId)
        {
            return _datamanager.GetFile(fileId);
        }

        public VRFileInfo GetFileInfo(long fileId)
        {
            return  _datamanager.GetFileInfo(fileId);
        }
    }
}
