(function (appControllers) {
    "use strict";
    bidsASKsAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_Module_ModuleConfig', 'SecurityService'];
    function bidsASKsAPIService(BaseAPIService, UtilsService, Demo_Module_ModuleConfig, SecurityService) {

        var controller = "BIDsASKs";

        function GetFilteredBIDs(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetFilteredBIDs"), input);
        };


        function GetFilteredASKs(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetFilteredASKs"), input);
        }

        return {
            GetFilteredBIDs: GetFilteredBIDs,
            GetFilteredASKs: GetFilteredASKs
        };
    };
    appControllers.service("Demo_Module_BIDsASKsAPIService", bidsASKsAPIService);

})(appControllers);