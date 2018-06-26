using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Common.Web
{
    [JSONWithType]
    [RoutePrefix(Constants.ROUTE_PREFIX + "File")]
    public class VRCommon_FileController : BaseAPIController
    {
        [Route("UploadFile")]
        [HttpPost]
        public FileUploadResult UploadFile()
        {
            HttpResponseMessage result = null;
            var httpRequest = HttpContext.Current.Request;

            if (httpRequest.Files.Count > 0)
            {
                HttpPostedFile postedFile = httpRequest.Files[0];
                string[] nameastab = postedFile.FileName.Split('.');

                string moduleName = httpRequest.Headers["Module-Name"];
                bool isTempFile = httpRequest.Headers["Temp-File"] == "true";

                VRFile file = new VRFile()
                {
                    Content = ReadToEnd(postedFile.InputStream),
                    Name = postedFile.FileName,
                    Extension = nameastab[nameastab.Length - 1],
                    ModuleName = moduleName,
                    IsTemp = isTempFile
                };

                // VRFileManager sets the UserId property via SecurityContext

                VRFileManager manager = new VRFileManager(moduleName);
                long id = manager.AddFile(file);


                result = Request.CreateResponse(HttpStatusCode.Created);
                return new FileUploadResult
                {
                    FileId = id,
                    Name = postedFile.FileName
                };
            }
            else
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
            }
        }

        [Route("DownloadFile")]
        [HttpGet]
        public HttpResponseMessage DownloadFile(long fileId, string moduleName = null)
        {
            VRFileManager manager = new VRFileManager(moduleName);
            VRFile file = manager.GetFile(fileId);
            if (file == null)
            {
                return null;
            }
            else if (!manager.DoesUserHaveViewAccessToFile(file))
            {
                return GetUnauthorizedResponse();
            }
            else
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
        }

        [Route("PreviewImage")]
        [HttpGet]
        public object PreviewImage(long fileId)
        {
            VRFileManager manager = new VRFileManager();
            VRFile file = manager.GetFile(fileId);
            //  HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            if (file == null)
            {
                return null;
            }
            else if (!manager.DoesUserHaveViewAccessToFile(file))
            {
                return GetUnauthorizedResponse();
            }
            else
            {
                if (file.Extension == "jpg" || file.Extension == "png" || file.Extension == "jpeg" || file.Extension == "bmp" || file.Extension == "gif")
                {
                    byte[] bytes = file.Content;

                    string base64String = "data:image/" + file.Extension + ";base64," + Convert.ToBase64String(bytes);
                    return base64String;


                }
                else
                {
                    return null;
                }
            }
        }

        [Route("GetFileInfo")]
        [HttpGet]
        public VRFileInfo GetFileInfo(long fileId, string moduleName = null)
        {
            VRFileManager manager = new VRFileManager(moduleName);
            VRFileInfo fileInfo = manager.GetFileInfo(fileId);
            if (fileInfo == null)
            {
                return null;
            }
            else
            {
                return fileInfo;
            }
        }

        [Route("GetFileName")]
        [HttpGet]
        public String GetFileName(long fileId)
        {

            VRFileInfo fileInfo = GetFileInfo(fileId);
            if (fileInfo == null)
            {
                return null;
            }
            else
            {
                return fileInfo.Name;// +"." + fileInfo.Extension;
            }
        }

        [HttpPost]
        [Route("GetFilteredRecentFiles")]
        public object GetFilteredRecentFiles(Vanrise.Entities.DataRetrievalInput<VRFileQuery> input)
        {
            //no need for security because it is returning the files of the logged in User
            VRFileManager manager = new VRFileManager(input.Query.ModuleName);
            return GetWebResponse(input, manager.GetFilteredRecentFiles(input));
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
