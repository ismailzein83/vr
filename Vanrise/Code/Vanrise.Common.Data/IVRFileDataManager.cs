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
       long AddFile(VRFile file);
       VRFileInfo GetFileInfo(long fileId);
       VRFile GetFile(long fileId);
       bool UpdateFileUsed(long fileId, bool isUsed);
    }
}
