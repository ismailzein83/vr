using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Data
{
    public interface IVRFileDataManager : IDataManager
    {
        string ModuleName { set; }

        long AddFile(VRFile file);
        VRFileInfo GetFileInfo(long fileId);
        List<VRFileInfo> GetFilesInfo(IEnumerable<long> fileIds);
        VRFile GetFile(long fileId);

        bool SetFileUsed(long fileId);

        bool SetFileUsedAndUpdateSettings(long fileId, VRFileSettings fileSettings);

        Vanrise.Entities.BigResult<VRFileInfo> GetFilteredRecentFiles(Vanrise.Entities.DataRetrievalInput<VRFileQuery> input);
    }
}
