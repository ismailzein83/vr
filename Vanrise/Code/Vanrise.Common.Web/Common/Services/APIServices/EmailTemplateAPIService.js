(function (appControllers) {

    "use strict";
    emailTemplateAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig','SecurityService'];

    function emailTemplateAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig, SecurityService) {
        var controller = 'EmailTemplate';
        function GetFilteredEmailTemplates(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controller, "GetFilteredEmailTemplates"), input);
        }

        function UpdateEmailTemplate(emailTemplate) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controller, "UpdateEmailTemplate"), emailTemplate);
        }

        function GetEmailTemplate(emailTemplateId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controller, "GetEmailTemplate"), {
                emailTemplateId: emailTemplateId
            });
        }

        function HasUpdateEmailTemplatePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_Sec_ModuleConfig.moduleName, controllerName, ['UpdateEmailTemplate']));
        }


        return ({
            GetFilteredEmailTemplates: GetFilteredEmailTemplates,
            UpdateEmailTemplate: UpdateEmailTemplate,
            GetEmailTemplate: GetEmailTemplate,
            HasUpdateEmailTemplatePermission: HasUpdateEmailTemplatePermission
        });
    }

    appControllers.service('VRCommon_EmailTemplateAPIService', emailTemplateAPIService);

})(appControllers);