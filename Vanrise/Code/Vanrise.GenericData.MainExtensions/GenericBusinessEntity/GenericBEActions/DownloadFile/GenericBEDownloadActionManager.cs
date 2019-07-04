using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.Common;
using Vanrise.GenericData.MainExtensions.GenericBusinessEntity.GenericBEActions;
using Vanrise.Common.Business;
using Vanrise.Entities;
using System.IO;
using System.Text.RegularExpressions;

namespace Vanrise.GenericData.MainExtensions
{
    public class GenericBEDownloadActionManager
    {
        GenericBusinessEntityDefinitionManager _genericBEDefinitionManager = new GenericBusinessEntityDefinitionManager();
        GenericBusinessEntityManager _genericBEManager = new GenericBusinessEntityManager();
        public DownloadGenericBEFileOutPut DownloadGenericBEFile(Object genericBusinessEntityId, Guid businessEntityDefinitionId, DownloadFileGenericBEAction genericBEAction,bool isRemoteCall)
        {
            var beDefinitionSettings = _genericBEDefinitionManager.GetGenericBEDefinitionSettings(businessEntityDefinitionId);
            var beEntity = _genericBEManager.GetGenericBusinessEntity(genericBusinessEntityId, businessEntityDefinitionId);
            genericBEAction.FileIdFieldName.ThrowIfNull("FileIdFieldName");

            if (beDefinitionSettings.DataRecordStorageId.HasValue)
            {
                if (beEntity != null && beEntity.FieldValues != null)
                {
                    var fileID = beEntity.FieldValues.GetRecord(genericBEAction.FileIdFieldName);
                    if (fileID != null)
                    {
                        VRFileManager fileManager = new VRFileManager();
                        var file = fileManager.GetFile(Convert.ToInt64(fileID.ToString()));
                        if (file != null)
                        {
                            var fileOutput = new DownloadGenericBEFileOutPut
                            {
                                Content = file.Content,
                                FileName = file.Name,
                            };
                            if (!isRemoteCall && genericBEAction.OpenNewWindow)
                            {
                                fileOutput.FilePath = SaveFileAndGetPhysicalPath(fileOutput.FileName, fileOutput.Content);
                            }
                            return fileOutput;
                        }

                    }
                }
            }
            else
            {

                var input = new DownloadGenericBEFileInput
                {
                    BusinessEntityDefinitionId = beDefinitionSettings.RemoteGenericBEDefinitionId.Value,
                    GenericBEAction = genericBEAction,
                    GenericBusinessEntityId = genericBusinessEntityId,
                    IsRemoteCall = true,
                };

                VRInterAppRestConnection connectionSettings = GetVRInterAppRestConnection(beDefinitionSettings.VRConnectionId.Value);
                var fileOutput = connectionSettings.Post<DownloadGenericBEFileInput, DownloadGenericBEFileOutPut>("/api/VR_GenericData/GenericBEDownloadAction/DownloadGenericBEFile", input, true);

                if (genericBEAction.OpenNewWindow)
                {
                    
                    fileOutput.FilePath = SaveFileAndGetPhysicalPath(fileOutput.FileName, fileOutput.Content);
                }
                return fileOutput;
            }
            return null;
        }

        private string SaveFileAndGetPhysicalPath(string fileName, byte[] content)
        {
            Regex reg = new Regex("[*'()[\",&#^@]");
            fileName = reg.Replace(fileName, "_");
            var returnedFilePath = string.Format("/TempFiles/{0}_{1}", Guid.NewGuid(), fileName);
            var physicalFilePath = VRWebContext.MapVirtualToPhysicalPath(string.Format("~{0}", returnedFilePath));
            if (!File.Exists(physicalFilePath))
            {
                var directoryName = Path.GetDirectoryName(physicalFilePath);

                if (!Directory.Exists(directoryName))
                    Directory.CreateDirectory(directoryName);

                if (!File.Exists(physicalFilePath))
                    File.WriteAllBytes(physicalFilePath, content);
            }
            return returnedFilePath;
        }
        public VRInterAppRestConnection GetVRInterAppRestConnection(Guid connectionId)
        {
            VRConnectionManager connectionManager = new VRConnectionManager();
            var vrConnection = connectionManager.GetVRConnection<VRInterAppRestConnection>(connectionId);
            return vrConnection.Settings as VRInterAppRestConnection;
        }
    }
}
