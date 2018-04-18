
(function (appControllers) {

    "use strict";

    SMSMessageTypeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'SecurityService', 'VRCommon_ModuleConfig'];

    function SMSMessageTypeAPIService(BaseAPIService, UtilsService, SecurityService, VRCommon_ModuleConfig) {

        var controllerName = "SMSMessageType";

        function GetSMSMessageType(smsMessageTypeId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'GetSMSMessageType'), {
                SMSMessageTypeId: smsMessageTypeId
            });
        }

        function GetSMSMessageTypesInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetSMSMessageTypesInfo"), {
                filter: filter
            });
        }

        function GetSMSHandlerSettings() {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetSMSHandlerSettings"), {
                
            });
        }

        return ({
            GetSMSMessageType: GetSMSMessageType,
            GetSMSMessageTypesInfo: GetSMSMessageTypesInfo,
            GetSMSHandlerSettings: GetSMSHandlerSettings
        });
    }

    appControllers.service('VRCommon_SMSMessageTypeAPIService', SMSMessageTypeAPIService);

})(appControllers);