
(function (appControllers) {

    "use strict";

    VRMailMessageTemplateAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig'];

    function VRMailMessageTemplateAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig) {

        var controllerName = "VRMailMessageTemplate";


        function GetFilteredMailMessageTemplates(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'GetFilteredMailMessageTemplates'), input);
        }

        function GetMailMessageTemplate(vrMailMessageTemplateId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'GetMailMessageTemplate'), {
                VRMailMessageTemplateId: vrMailMessageTemplateId
            });
        }

        function AddMailMessageTemplate(vrMailMessageTemplateItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'AddMailMessageTemplate'), vrMailMessageTemplateItem);
        }

        function UpdateMailMessageTemplate(vrMailMessageTemplateItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'UpdateMailMessageTemplate'), vrMailMessageTemplateItem);
        }

        return ({
            GetFilteredMailMessageTemplates: GetFilteredMailMessageTemplates,
            GetMailMessageTemplate: GetMailMessageTemplate,
            AddMailMessageTemplate: AddMailMessageTemplate,
            UpdateMailMessageTemplate: UpdateMailMessageTemplate
        });
    }

    appControllers.service('VRCommon_VRMailMessageTemplateAPIService', VRMailMessageTemplateAPIService);

})(appControllers);