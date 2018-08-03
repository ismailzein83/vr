using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class VRFileManager
    {
        #region Constructors / Fields

        private readonly IVRFileDataManager _datamanager;

        public VRFileManager(string moduleName)
        {
            _datamanager = CommonDataManagerFactory.GetDataManager<IVRFileDataManager>();
            _datamanager.ModuleName = moduleName;
        }

        public VRFileManager()
            : this(null)
        {

        }

        #endregion

        #region Public Methods

        public long AddFile(VRFile file)
        {
            int? userId;
            if (Vanrise.Security.Entities.ContextFactory.GetContext().TryGetLoggedInUserId(out userId))
                file.UserId = userId;
            return _datamanager.AddFile(file);
        }
        

        public bool SetFileUsed(long fileId)
        {
            return _datamanager.SetFileUsed(fileId);
        }

        public bool SetFileUsedAndUpdateSettings(long fileId, VRFileSettings fileSettings)
        {
            return _datamanager.SetFileUsedAndUpdateSettings(fileId, fileSettings);
        }
        public bool SetFileUsedAndUpdateSettings(Guid fileUniqueId, VRFileSettings fileSettings)
        {
            return _datamanager.SetFileUsedAndUpdateSettings(fileUniqueId, fileSettings);
        }

        public VRFile GetFile(long fileId)
        {
            return _datamanager.GetFile(fileId);
        }

        public VRFile GetFileByUniqueId(Guid fileUniqueId)
        {
            return _datamanager.GetFileByUniqueId(fileUniqueId);
        }

        public bool DoesUserHaveViewAccessToFile(VRFileInfo file)
        {
            if(file.Settings != null && file.Settings.ExtendedSettings != null)
            {
                var context = new VRFileDoesUserHaveViewAccessContext { UserId = Vanrise.Security.Entities.ContextFactory.GetContext().GetLoggedInUserId() };
                return file.Settings.ExtendedSettings.DoesUserHaveViewAccess(context);
            }
            else
            {
                return true;
            }
        }

        public VRFileInfo GetFileInfo(long fileId)
        {
            return _datamanager.GetFileInfo(fileId);
        }
        public VRFileInfo GetFileInfoByUniqueId(Guid fileUniqueId)
        {
            return _datamanager.GetFileInfoByUniqueId(fileUniqueId);
        }
        public List<VRFileInfo> GetFilesInfo(IEnumerable<long> fileIds)
        {
            return _datamanager.GetFilesInfo(fileIds);
        }

        public IDataRetrievalResult<VRFileInfo> GetFilteredRecentFiles(DataRetrievalInput<VRFileQuery> input)
        {
            input.Query.UserId = Vanrise.Security.Entities.ContextFactory.GetContext().GetLoggedInUserId();
            return DataRetrievalManager.Instance.ProcessResult(input, _datamanager.GetFilteredRecentFiles(input));
        }

        #endregion

        #region Private Classes

        private class VRFileDoesUserHaveViewAccessContext : IVRFileDoesUserHaveViewAccessContext
        {
            public int UserId
            {
                get;
                set;
            }
        }


        #endregion
    }
}
