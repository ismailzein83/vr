(function (appControllers) {

    "use strict";
    VRMailAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig'];

    function VRMailAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig) {
        var controller = 'VRMail';

        function SendTestEmail(emailSettingDetail) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controller, "SendTestEmail"), emailSettingDetail);
        }
        return ({
            SendTestEmail: SendTestEmail
        });
    }

    appControllers.service('VRCommon_VRMailAPIService', VRMailAPIService);

})(appControllers);