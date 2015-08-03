using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Vanrise.Common;
using Vanrise.Entities;

namespace Vanrise.Web.Controllers
{
    public class VRFileController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpPost]
        public Task<FileUploadResult> UploadFile()
        {
            HttpRequestMessage request = this.Request;
            if (!request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.UnsupportedMediaType));
            }
            VRFileManager fileManager = new VRFileManager();
            //string root = HttpContext.Current.Server.MapPath("~/Upload");
            //if (!Directory.Exists(root))
            //    Directory.CreateDirectory(root);
            //var provider = new MultipartFormDataStreamProvider(root);

            //var task = request.Content..ReadAsMultipartAsync(provider).
            //    ContinueWith<FileUploadResult>(o =>
            //    {
            //        //FileInfo finfo = new FileInfo(provider.FileData.First().LocalFileName);

            //        //string guid = Guid.NewGuid().ToString();

            //        //File.Move(finfo.FullName, Path.Combine(root, guid + "_" + provider.FileData.First().Headers.ContentDisposition.FileName.Replace("\"", "")));

            //        return new FileUploadResult()
            //        {
            //            FileId = 22
            //        };
            //    }
            //);
            return null;
        }
    }

    
}
