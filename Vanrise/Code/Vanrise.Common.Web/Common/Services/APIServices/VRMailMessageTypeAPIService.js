
(function (appControllers) {

    "use strict";

    VRMailMessageTypeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig'];

    function VRMailMessageTypeAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig) {

        var controllerName = "VRMailMessageType";


        function GetFilteredMailMessageTypes(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'GetFilteredMailMessageTypes'), input);
        }

        function GetMailMessageType(vrMailMessageTypeId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'GetMailMessageType'), {
                VRMailMessageTypeId: vrMailMessageTypeId
            });
        }

        function AddMailMessageType(vrMailMessageTypeItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'AddMailMessageType'), vrMailMessageTypeItem);
        }

        function UpdateMailMessageType(vrMailMessageTypeItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'UpdateMailMessageType'), vrMailMessageTypeItem);
        }

        function GetMailMessageTypesInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetMailMessageTypesInfo"), {
                filter: filter
            });
        }

        return ({
            GetFilteredMailMessageTypes: GetFilteredMailMessageTypes,
            GetMailMessageType: GetMailMessageType,
            AddMailMessageType: AddMailMessageType,
            UpdateMailMessageType: UpdateMailMessageType,
            GetMailMessageTypesInfo: GetMailMessageTypesInfo
        });
    }

    appControllers.service('VRCommon_VRMailMessageTypeAPIService', VRMailMessageTypeAPIService);

})(appControllers);