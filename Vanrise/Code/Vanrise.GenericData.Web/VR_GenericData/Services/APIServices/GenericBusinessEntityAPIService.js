﻿ (function (appControllers) {

    'use strict';

    GenericBusinessEntityAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_GenericData_ModuleConfig'];

    function GenericBusinessEntityAPIService(BaseAPIService, UtilsService, VR_GenericData_ModuleConfig) {
        var controllerName = 'GenericBusinessEntity';

        return {
            GetFilteredGenericBusinessEntities: GetFilteredGenericBusinessEntities,
            GetGenericBusinessEntityDetail: GetGenericBusinessEntityDetail,
            AddGenericBusinessEntity: AddGenericBusinessEntity,
            GetGenericBusinessEntityEditorRuntime:GetGenericBusinessEntityEditorRuntime,
            DoesUserHaveAddAccess:DoesUserHaveAddAccess,
            UpdateGenericBusinessEntity: UpdateGenericBusinessEntity,
            DoesUserHaveEditAccess: DoesUserHaveEditAccess,
            GetGenericBusinessEntityInfo: GetGenericBusinessEntityInfo,
			DeleteGenericBusinessEntity: DeleteGenericBusinessEntity,
			UploadGenericBusinessEntities: UploadGenericBusinessEntities,
			DownloadGenericBusinessEntityTemplate: DownloadGenericBusinessEntityTemplate,
            DownloadBusinessEntityLog: DownloadBusinessEntityLog,
            ExecuteGenericBEBulkActions: ExecuteGenericBEBulkActions,
            GetGenericBETitleFieldValue: GetGenericBETitleFieldValue,
            DoesUserHaveViewAccess: DoesUserHaveViewAccess,
            //GetGenericEditorColumnsInfo: GetGenericEditorColumnsInfo
        };
        //function GetGenericEditorColumnsInfo(input) {
        //    return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, 'GetGenericEditorColumnsInfo'), input);
        //}

        function GetGenericBETitleFieldValue(businessEntityDefinitionId, genericBusinessEntityId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, 'GetGenericBETitleFieldValue'), {
                businessEntityDefinitionId: businessEntityDefinitionId,
                genericBusinessEntityId: genericBusinessEntityId
            });
        }

        function GetFilteredGenericBusinessEntities(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, 'GetFilteredGenericBusinessEntities'), input);
        }

        function GetGenericBusinessEntity(genericBusinessEntityId, businessEntityDefinitionId, historyId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, 'GetGenericBusinessEntity'), {
                genericBusinessEntityId: genericBusinessEntityId,
                businessEntityDefinitionId: businessEntityDefinitionId,
                historyId: historyId
            });
        }
        function GetGenericBusinessEntityEditorRuntime(businessEntityDefinitionId, genericBusinessEntityId, historyId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, 'GetGenericBusinessEntityEditorRuntime'), {
                businessEntityDefinitionId: businessEntityDefinitionId,
                    genericBusinessEntityId: genericBusinessEntityId,
        historyId: historyId
            });
        }
        function GetGenericBusinessEntityDetail(genericBusinessEntityId, businessEntityDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, 'GetGenericBusinessEntityDetail'), {
                businessEntityDefinitionId: businessEntityDefinitionId,
                genericBusinessEntityId: genericBusinessEntityId
            });
        }
        function GetGenericBusinessEntityInfo(businessEntityDefinitionId,serializedFilter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, 'GetGenericBusinessEntityInfo'), {
                businessEntityDefinitionId:businessEntityDefinitionId,
                serializedFilter: serializedFilter
            });
        }
        function AddGenericBusinessEntity(genericBusinessEntity) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, 'AddGenericBusinessEntity'), genericBusinessEntity);
        }
        function DoesUserHaveAddAccess(businessEntityDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, 'DoesUserHaveAddAccess'), {
                businessEntityDefinitionId: businessEntityDefinitionId
            });
        }

        function DoesUserHaveViewAccess(businessEntityDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, 'DoesUserHaveViewAccess'), {
                businessEntityDefinitionId: businessEntityDefinitionId
            });
        }
        function DeleteGenericBusinessEntity(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, 'DeleteGenericBusinessEntity'), input);
        }

        function UpdateGenericBusinessEntity(genericBusinessEntity) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, 'UpdateGenericBusinessEntity'), genericBusinessEntity);
        }
        function DoesUserHaveEditAccess(businessEntityDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, 'DoesUserHaveEditAccess'), {
                businessEntityDefinitionId: businessEntityDefinitionId
            }, {
                useCache: true
            });
		}

		function UploadGenericBusinessEntities(businessEntityDefinitionId,fileId) {
			return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, "UploadGenericBusinessEntities"), {
				businessEntityDefinitionId: businessEntityDefinitionId,
				fileId: fileId
			});
		}

		function DownloadGenericBusinessEntityTemplate(businessEntityDefinitionId) {
			return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, "DownloadGenericBusinessEntityTemplate"), { businessEntityDefinitionId: businessEntityDefinitionId },
				{
				returnAllResponseParameters: true,
				responseTypeAsBufferArray: true
			});
		} 

		function DownloadBusinessEntityLog(fileId) {
			return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, "DownloadBusinessEntityLog"), { fileId: fileId },
				{
					returnAllResponseParameters: true,
					responseTypeAsBufferArray: true
				});
        }
        function ExecuteGenericBEBulkActions(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, "ExecuteGenericBEBulkActions"), input);
        }
    }

    appControllers.service('VR_GenericData_GenericBusinessEntityAPIService', GenericBusinessEntityAPIService);

})(appControllers);