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

namespace Vanrise.Web.Controllers
{
    public class VRFileController : Vanrise.Web.Base.BaseAPIController
    {
        
        [HttpPost]
        public FileUploadResult UploadFile()
        {

            HttpResponseMessage result = null;

            var httpRequest = HttpContext.Current.Request;

            if (httpRequest.Files.Count > 0 )
            {

                HttpPostedFile postedFile = httpRequest.Files[0];
                string[] nameastab = postedFile.FileName.Split('.');
                VRFile file = new VRFile()
                {
                    Content = ReadToEnd(postedFile.InputStream) ,
                    Name = postedFile.FileName ,
                    Extension = nameastab[nameastab.Length -1],
                    CreatedTime = DateTime.Now,
                    FileUniqueId = Guid.NewGuid()
                };
                VRFileManager manager = new VRFileManager();
                long id = manager.AddFile(file);

                
                result = Request.CreateResponse(HttpStatusCode.Created );
                return new FileUploadResult
                {
                    FileId = id,
                    FileUniqueId = file.FileUniqueId
                };

            }

            else
            {

                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));


            }
        }
        [HttpGet]
        public HttpResponseMessage DownloadFile(long fileId) {

            VRFileManager manager = new VRFileManager();
            VRFile file = manager.GetFile(fileId);
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
                FileName = String.Format(file.Name) 
            };
            return response;

        }

        [HttpGet]
        public string PreviewImage(long fileId)
        {

            VRFileManager manager = new VRFileManager();
            VRFile file = manager.GetFile(fileId);
          //  HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);

            if ( file != null && ( file.Extension == "jpg" || file.Extension == "png" || file.Extension == "jpeg" || file.Extension == "bmp" || file.Extension == "gif"))
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

        [HttpGet]
        public VRFileInfo GetFileInfo(long fileId)
        {

            VRFileManager manager = new VRFileManager();
            return manager.GetFileInfo(fileId);

        }

        [HttpGet]
        public VRFileInfo GetFileInfoByUniqueId(Guid fileUniqueId)
        {
            VRFileManager manager = new VRFileManager();
            return manager.GetFileInfoByUniqueId(fileUniqueId);
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
