﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.MainExtensions.GenericBusinessEntity.GenericBEActions;
using Vanrise.Web.Base;

namespace Vanrise.GenericData.MainExtensions
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "GenericBEDownloadAction")]
    public class GenericBEDownloadActionController : BaseAPIController
    {
        GenericBEDownloadActionManager _manager = new GenericBEDownloadActionManager();
        [HttpPost]
        [Route("DownloadGenericBEFile")]
        public Object DownloadGenericBEFile(DownloadGenericBEFileInput input)
        {
            var file = _manager.DownloadGenericBEFile(input.GenericBusinessEntityId, input.BusinessEntityDefinitionId, input.GenericBEAction,input.IsRemoteCall);
            if (input.IsRemoteCall)
            {
                return file;
            }
            if (input.GenericBEAction.OpenNewWindow)
                return file.FilePath;

            return GetExcelResponse(file.Content, file.FileName);
        }
    }

    public class DownloadGenericBEFileInput
    {
        public Object GenericBusinessEntityId { get; set; }
        public DownloadFileGenericBEAction GenericBEAction { get; set; }
        public Guid BusinessEntityDefinitionId { get; set; }
        public bool IsRemoteCall { get; set; }
    }
    public class DownloadGenericBEFileOutPut
    {
        public byte[] Content { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }
}
