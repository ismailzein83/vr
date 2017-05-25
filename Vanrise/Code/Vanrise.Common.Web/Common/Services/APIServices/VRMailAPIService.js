(function (appControllers) {

    "use strict";
    VRMailAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig'];

    function VRMailAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig) {
        var controller = 'VRMail';

        function SendTestEmail(emailSettingDetail) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controller, "SendTestEmail"), emailSettingDetail);
        }
        function SendEmail(emailSetting) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controller, "SendEmail"), emailSetting);
        }
        function GetFileName(fileId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controller, "GetFileName"), { fileId: fileId });
        }
        function GetSalePriceListFile(fileId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controller, "GetSalePriceListFile"), { fileId: fileId }, {
                returnAllResponseParameters: true,
                responseTypeAsBufferArray: true
            });
        }
        return ({
            SendTestEmail: SendTestEmail,
            SendEmail: SendEmail,
            GetFileName: GetFileName,
            GetSalePriceListFile: GetSalePriceListFile
        });
    }

    appControllers.service('VRCommon_VRMailAPIService', VRMailAPIService);

})(appControllers);