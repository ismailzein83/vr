
(function (appControllers) {

    "use strict";

    SMSMessageTemplateAPIService.$inject = ['BaseAPIService', 'UtilsService', 'SecurityService', 'VRCommon_ModuleConfig'];

    function SMSMessageTemplateAPIService(BaseAPIService, UtilsService, SecurityService, VRCommon_ModuleConfig) {

        var controllerName = "SMSMessageTemplate";


        function GetFilteredSMSMessageTemplates(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'GetFilteredSMSMessageTemplates'), input);
        }

        function GetSMSMessageTemplate(smsMessageTemplateId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'GetSMSMessageTemplate'), {
                SMSMessageTemplateId: smsMessageTemplateId
            });
        }

        function GetSMSMessageTemplatesInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetSMSMessageTemplatesInfo"), {
                filter: filter
            });
        }

        function AddSMSMessageTemplate(smsMessageTemplateItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'AddSMSMessageTemplate'), smsMessageTemplateItem);
        }

        function UpdateSMSMessageTemplate(smsMessageTemplateItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'UpdateSMSMessageTemplate'), smsMessageTemplateItem);
        }

        function HasAddSMSMessageTemplatePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VRCommon_ModuleConfig.moduleName, controllerName, ['AddSMSMessageTemplate']));
        }

        function HasEditSMSMessageTemplatePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VRCommon_ModuleConfig.moduleName, controllerName, ['UpdateSMSMessageTemplate']));
        }

        return ({
            GetFilteredSMSMessageTemplates: GetFilteredSMSMessageTemplates,
            GetSMSMessageTemplate: GetSMSMessageTemplate,
            GetSMSMessageTemplatesInfo: GetSMSMessageTemplatesInfo,
            AddSMSMessageTemplate: AddSMSMessageTemplate,
            UpdateSMSMessageTemplate: UpdateSMSMessageTemplate,
            HasAddSMSMessageTemplatePermission: HasAddSMSMessageTemplatePermission,
            HasEditSMSMessageTemplatePermission: HasEditSMSMessageTemplatePermission
        });
    }

    appControllers.service('VRCommon_SMSMessageTemplateAPIService', SMSMessageTemplateAPIService);

})(appControllers);