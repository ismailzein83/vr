(function (appControllers) {
    "use strict";
    switchReleaseCauseAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_BE_ModuleConfig", "SecurityService"];
    function switchReleaseCauseAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig, SecurityService) {
        var controllerName = "SwitchReleaseCause";
        function GetFilteredSwitchReleaseCauses(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetFilteredSwitchReleaseCauses"), input);
        }
        function AddSwitchReleaseCause(switchReleaseCause)
        {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "AddSwitchReleaseCause"), switchReleaseCause);
        }
        function GetSwitchReleaseCause(switchReleaseCauseId)
        {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetSwitchReleaseCause"), { switchReleaseCauseId: switchReleaseCauseId });
        }
        function UpdateSwitchReleaseCause(switchReleaseCause) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "UpdateSwitchReleaseCause"), switchReleaseCause);
        }
        function GetReleaseCausesByCode(code, switchIds) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetReleaseCausesByCode"), {
                code: code,
                switchIds: switchIds
            });
        }
        function DownloadSwitchReleaseCausesTemplate() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "DownloadSwitchReleaseCausesTemplate"),
                {},
                {
                    returnAllResponseParameters: true,
                    responseTypeAsBufferArray: true
                }
            );
        }
        function UploadSwitchReleaseCauses(fileId,switchId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "UploadSwitchReleaseCauses"), { fileId: fileId, switchId: switchId });
        }
        function DownloadSwitchReleaseCauseLog(fileID) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "DownloadSwitchReleaseCauseLog"), { fileID: fileID }, {
                returnAllResponseParameters: true,
                responseTypeAsBufferArray: true
            });
        }
        return ({
            GetFilteredSwitchReleaseCauses: GetFilteredSwitchReleaseCauses,
            AddSwitchReleaseCause: AddSwitchReleaseCause,
            GetSwitchReleaseCause: GetSwitchReleaseCause,
            UpdateSwitchReleaseCause: UpdateSwitchReleaseCause,
            GetReleaseCausesByCode: GetReleaseCausesByCode,
            DownloadSwitchReleaseCausesTemplate: DownloadSwitchReleaseCausesTemplate,
            UploadSwitchReleaseCauses: UploadSwitchReleaseCauses,
            DownloadSwitchReleaseCauseLog: DownloadSwitchReleaseCauseLog
        });
    }
    appControllers.service("WhS_BE_SwitchReleaseCauseAPIService", switchReleaseCauseAPIService);
})(appControllers);