using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
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

        public HttpResponseMessage DownloadRemoteFile(Guid connectionId, long fileId, string moduleName = null)
        {
            var vrConnection = _connectionManager.GetVRConnection<VRInterAppRestConnection>(connectionId);
            VRInterAppRestConnection connectionSettings = vrConnection.Settings as VRInterAppRestConnection;
            var file = connectionSettings.Get<VRFile>(string.Format("/api/VRCommon/File/GetRemoteFile?fileId={0}&moduleName={1}", fileId, moduleName));
            if (file != null)
            {
                byte[] bytes = file.Content;
                MemoryStream memStreamRate = new System.IO.MemoryStream();
                memStreamRate.Write(bytes, 0, bytes.Length);
                memStreamRate.Seek(0, System.IO.SeekOrigin.Begin);

                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                memStreamRate.Position = 0;
                response.Content = new StreamContent(memStreamRate);

                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = HttpUtility.UrlPathEncode(file.Name)
                };
                return response;
            }
            return null;
        }

        public VRFileInfo GetRemoteFileInfo(Guid connectionId, long fileId, string moduleName = null)
        {
            var vrConnection = _connectionManager.GetVRConnection<VRInterAppRestConnection>(connectionId);
            VRInterAppRestConnection connectionSettings = vrConnection.Settings as VRInterAppRestConnection;
            return connectionSettings.Get<VRFileInfo>(string.Format("/api/VRCommon/File/GetRemoteFileInfo?fileId={0}&moduleName={1}", fileId, moduleName));
        }
    }
}
