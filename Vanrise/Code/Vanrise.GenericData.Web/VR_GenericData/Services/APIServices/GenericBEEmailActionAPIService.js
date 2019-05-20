(function (appControllers) {

    "use strict";
    genericBEEmailActionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_GenericData_ModuleConfig', 'SecurityService'];

    function genericBEEmailActionAPIService(BaseAPIService, UtilsService, VR_GenericData_ModuleConfig, SecurityService) {

        var controllerName = 'GenericBEEmailAction';

        function SendEmail(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, "SendEmail"), input);
        }

        function GetEmailTemplate(genericBusinessEntityId, businessEntityDefinitionId, genericBEMailTemplateId, genericBEActionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, 'GetEmailTemplate'), {
                genericBusinessEntityId: genericBusinessEntityId,
                businessEntityDefinitionId: businessEntityDefinitionId,
                genericBEMailTemplateId: genericBEMailTemplateId,
                genericBEActionId: genericBEActionId
            });
        }
        function GetMailTemplateIdByInfoType(genericBusinessEntityId, businessEntityDefinitionId, infoType) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, 'GetMailTemplateIdByInfoType'), {
                genericBusinessEntityId: genericBusinessEntityId,
                businessEntityDefinitionId: businessEntityDefinitionId,
                infoType: infoType,
            });
        }
        return ({
            SendEmail: SendEmail,
            GetEmailTemplate: GetEmailTemplate,
            GetMailTemplateIdByInfoType: GetMailTemplateIdByInfoType
        });
    }

    appControllers.service('VR_GenericData_GenericBEEmailActionAPIService', genericBEEmailActionAPIService);

})(appControllers);