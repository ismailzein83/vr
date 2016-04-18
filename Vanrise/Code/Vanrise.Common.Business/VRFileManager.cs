using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;
using Vanrise.Entities;
using Vanrise.Security.Business;

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
            file.UserId = SecurityContext.Current.GetLoggedInUserId();
            return _datamanager.AddFile(file);
        }

        public bool SetFileUsed(long fileId)
        {
            return _datamanager.UpdateFileUsed(fileId, true);
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
            return _datamanager.GetFileInfo(fileId);
        }

        public IDataRetrievalResult<VRFileInfo> GetFilteredRecentFiles(DataRetrievalInput<VRFileQuery> input)
        {
            input.Query.UserId = SecurityContext.Current.GetLoggedInUserId();
            return DataRetrievalManager.Instance.ProcessResult(input, _datamanager.GetFilteredRecentFiles(input));
        }

        public decimal? GetMaxUncompressedFileSizeInMegaBytes()
        {
            string size = ConfigurationManager.AppSettings["VRFile_Max_Uncompressed_MB_Size"];
            if (size != null)
                return Convert.ToDecimal(size);
            else
                return null;
        }

        #endregion
    }
}
