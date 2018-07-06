(function (appControllers) {
    "use strict";
    masterAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_Module_ModuleConfig', 'SecurityService'];
    function masterAPIService(BaseAPIService, UtilsService, Demo_Module_ModuleConfig, SecurityService) {

        var controller = "Master";

        function GetFilteredMasters(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetFilteredMasters"), input);
        }
        function GetMasterById(masterId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetMasterById"),
                { masterId: masterId
                });
        }
        function UpdateMaster(master) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "UpdateMaster"), master);
        }
        function AddMaster(master) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "AddMaster"), master);
        };
        function GetMastersInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetMastersInfo"), { filter: filter });
        };

        return {
            GetFilteredMasters: GetFilteredMasters,
            GetMasterById: GetMasterById,
            UpdateMaster: UpdateMaster,
            AddMaster: AddMaster,
            GetMastersInfo: GetMastersInfo
        };
    };
    appControllers.service("Demo_Module_MasterAPIService", masterAPIService);

})(appControllers);