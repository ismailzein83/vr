
(function (appControllers) {

    "use strict";

    VRMailMessageTemplateAPIService.$inject = ['BaseAPIService', 'UtilsService', 'SecurityService', 'VRCommon_ModuleConfig'];

    function VRMailMessageTemplateAPIService(BaseAPIService, UtilsService, SecurityService, VRCommon_ModuleConfig) {

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

        function HasAddMailMessageTemplatePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VRCommon_ModuleConfig.moduleName, controllerName, ['AddMailMessageTemplate']));
        }

        function HasEditMailMessageTemplatePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VRCommon_ModuleConfig.moduleName, controllerName, ['UpdateMailMessageTemplate']));
        }

        return ({
            GetFilteredMailMessageTemplates: GetFilteredMailMessageTemplates,
            GetMailMessageTemplate: GetMailMessageTemplate,
            AddMailMessageTemplate: AddMailMessageTemplate,
            UpdateMailMessageTemplate: UpdateMailMessageTemplate,
            HasAddMailMessageTemplatePermission: HasAddMailMessageTemplatePermission,
            HasEditMailMessageTemplatePermission: HasEditMailMessageTemplatePermission
        });
    }

    appControllers.service('VRCommon_VRMailMessageTemplateAPIService', VRMailMessageTemplateAPIService);

})(appControllers);