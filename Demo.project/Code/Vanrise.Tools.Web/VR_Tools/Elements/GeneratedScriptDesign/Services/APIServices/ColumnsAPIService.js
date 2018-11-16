(function (appControllers) {
    "use strict";
    columnsAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Tools_ModuleConfig', 'SecurityService'];
    function columnsAPIService(BaseAPIService, UtilsService, VR_Tools_ModuleConfig, SecurityService) {

        var controller = "Columns";

        function GetColumnsInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Tools_ModuleConfig.moduleName, controller, "GetColumnsInfo"), { filter: filter });
        };
        function GetGeneratedScriptItemTableSettingsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Tools_ModuleConfig.moduleName, controller, "GetGeneratedScriptItemTableSettingsConfigs"));
        };
        function GetExceptionMessage(generatedScriptItemTable) {
            console.log(generatedScriptItemTable)
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Tools_ModuleConfig.moduleName, controller, "GetExceptionMessage"), generatedScriptItemTable);
        };
        return {
            GetColumnsInfo: GetColumnsInfo,
            GetGeneratedScriptItemTableSettingsConfigs: GetGeneratedScriptItemTableSettingsConfigs,
            GetExceptionMessage: GetExceptionMessage
        };
    };
    appControllers.service("VR_Tools_ColumnsAPIService", columnsAPIService);

})(appControllers);