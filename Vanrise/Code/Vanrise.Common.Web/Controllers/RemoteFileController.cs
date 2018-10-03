using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Common.Web.Controllers
{
    [JSONWithType]
    [RoutePrefix(Constants.ROUTE_PREFIX + "RemoteFile")]
    public class VRCommon_RemoteFileController : BaseAPIController
    {
        VRRemoteFileManager _remoteFileManager = new VRRemoteFileManager();

        [Route("UploadRemoteFile")]
        [HttpPost]
        public FileUploadResult UploadRemoteFile()
        {
            var httpRequest = HttpContext.Current.Request;

            HttpPostedFile postedFile = httpRequest.Files[0];
            string[] nameastab = postedFile.FileName.Split('.');
            Guid connectionId = Guid.Parse(httpRequest.Headers["Connection-Id"]);
            string moduleName = httpRequest.Headers["Module-Name"];
            bool isTempFile = httpRequest.Headers["Temp-File"] == "true";

            VRFile file = new VRFile()
            {
                Content = ReadToEnd(postedFile.InputStream),
                Name = postedFile.FileName,
                Extension = nameastab[nameastab.Length - 1],
                ModuleName = moduleName,
                IsTemp = isTempFile,
                FileUniqueId = Guid.NewGuid()
            };
            return _remoteFileManager.UploadRemoteFile(file, connectionId);
        }

        [Route("DownloadRemoteFile")]
        [HttpGet]
        public HttpResponseMessage DownloadRemoteFile(Guid connectionId, long fileId, string moduleName = null)
        {
            return _remoteFileManager.DownloadRemoteFile(connectionId, fileId, moduleName);
        }

        [Route("GetRemoteFileInfo")]
        [HttpGet]
        public VRFileInfo GetRemoteFileInfo(Guid connectionId, long fileId, string moduleName = null)
        {
            return _remoteFileManager.GetRemoteFileInfo(connectionId, fileId, moduleName);
        }

        private static byte[] ReadToEnd(System.IO.Stream stream)
        {
            long originalPosition = 0;

            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }
        }
    }
}