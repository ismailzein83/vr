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

namespace Vanrise.GenericData.MainExtensions
{
    public class GenericBEDownloadActionManager
    {
        GenericBusinessEntityDefinitionManager _genericBEDefinitionManager = new GenericBusinessEntityDefinitionManager();
        GenericBusinessEntityManager _genericBEManager = new GenericBusinessEntityManager();
        public DownloadGenericBEFileOutPut DownloadGenericBEFile(Object genericBusinessEntityId, Guid businessEntityDefinitionId, GenericBEAction genericBEAction)
        {
            var beDefinitionSettings = _genericBEDefinitionManager.GetGenericBEDefinitionSettings(businessEntityDefinitionId);
            if (beDefinitionSettings.DataRecordStorageId.HasValue)
            {
                var beEntity = _genericBEManager.GetGenericBusinessEntity(genericBusinessEntityId, businessEntityDefinitionId);
                genericBEAction.ThrowIfNull("genericBEAction");
                var action = genericBEAction.Settings.CastWithValidate<DownloadFileGenericBEAction>("DownloadFileGenericBEAction");
                action.FileIdFieldName.ThrowIfNull("FileIdFieldName");
                if (beEntity != null && beEntity.FieldValues != null)
                {
                    var fileID = beEntity.FieldValues.GetRecord(action.FileIdFieldName);
                    if(fileID != null)
                    {
                        VRFileManager fileManager = new VRFileManager();
                        var file = fileManager.GetFile(Convert.ToInt64(fileID.ToString()));
                        if(file != null)
                        {
                            return new DownloadGenericBEFileOutPut
                            {
                                Content = file.Content,
                                FileName = file.Name
                            };
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
                    IsRemoteCall = true
                };
                VRInterAppRestConnection connectionSettings = GetVRInterAppRestConnection(beDefinitionSettings.VRConnectionId.Value);
                return connectionSettings.Post<DownloadGenericBEFileInput, DownloadGenericBEFileOutPut>("/api/VR_GenericData/GenericBEDownloadAction/DownloadGenericBEFile", input,true);
            }
            return null;
        }
        public VRInterAppRestConnection GetVRInterAppRestConnection(Guid connectionId)
        {
            VRConnectionManager connectionManager = new VRConnectionManager();
            var vrConnection = connectionManager.GetVRConnection<VRInterAppRestConnection>(connectionId);
            return vrConnection.Settings as VRInterAppRestConnection;
        }
    }
}
