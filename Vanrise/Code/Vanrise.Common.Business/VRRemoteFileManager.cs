using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class VRRemoteFileManager
    {
        VRConnectionManager _connectionManager = new VRConnectionManager();

        public FileUploadResult UploadRemoteFile(VRFile file, Guid connectionId)
        {
            var vrConnection = _connectionManager.GetVRConnection<VRInterAppRestConnection>(connectionId);
            VRInterAppRestConnection connectionSettings = vrConnection.Settings as VRInterAppRestConnection;
            return connectionSettings.Post<VRFile, FileUploadResult>("/api/VRCommon/File/UploadVRFile", file);
        }
        
        public VRFile GetRemoteFile(Guid connectionId, long fileId, string moduleName = null)
        {
            var vrConnection = _connectionManager.GetVRConnection<VRInterAppRestConnection>(connectionId);
            VRInterAppRestConnection interAppConnection = vrConnection.Settings.CastWithValidate<VRInterAppRestConnection>("vrConnection");
            return interAppConnection.Get<VRFile>(string.Format("/api/VRCommon/File/GetRemoteFile?fileId={0}&moduleName={1}", fileId, moduleName));
        }

        public VRFileInfo GetRemoteFileInfo(Guid connectionId, long fileId, string moduleName = null)
        {
            var vrConnection = _connectionManager.GetVRConnection<VRInterAppRestConnection>(connectionId);
            VRInterAppRestConnection connectionSettings = vrConnection.Settings as VRInterAppRestConnection;
            return connectionSettings.Get<VRFileInfo>(string.Format("/api/VRCommon/File/GetRemoteFileInfo?fileId={0}&moduleName={1}", fileId, moduleName));
        }
    }
}
