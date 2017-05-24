(function (appControllers) {

    "use strict";
    VRMailAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig'];

    function VRMailAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig) {
        var controller = 'VRMail';

        function SendTestEmail(emailSettingDetail) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controller, "SendTestEmail"), emailSettingDetail);
        }
        function SendEmail(emailSettingDetail) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controller, "SendEmail"), emailSettingDetail);
        }
        function GetFileName(fileId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controller, "GetFileName"), { fileId: fileId });
        }
        return ({
            SendTestEmail: SendTestEmail,
            SendEmail: SendEmail,
            GetFileName: GetFileName
        });
    }

    appControllers.service('VRCommon_VRMailAPIService', VRMailAPIService);

})(appControllers);