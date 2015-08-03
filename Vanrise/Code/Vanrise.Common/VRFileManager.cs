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
        public void AddFile(VRFile file)
        {
            IVRFileDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRFileDataManager>();
        }

        public bool SetFileUsed(long fileId)
        {
            return true;
        }

        public VRFile GetFile(long fileId)
        {
            return null;
        }

        public VRFileInfo GetFileInfo(long fileId)
        {
            return null;
        }
    }
}
