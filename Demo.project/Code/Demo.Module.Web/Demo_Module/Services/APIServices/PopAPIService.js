(function (appControllers) {

    "use strict";
    popAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_ModuleConfig'];

    function popAPIService(BaseAPIService, UtilsService, Demo_ModuleConfig) {

        function GetFilteredPops(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_ModuleConfig.moduleName, "Pop", "GetFilteredPops"), input);
        }

        function GetPop(popId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_ModuleConfig.moduleName, "Pop", "GetPop"), {
                popId: popId
            });

        }
       
        function UpdatePop(popObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_ModuleConfig.moduleName, "Pop", "UpdatePop"), popObject);
        }
        function AddPop(popObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_ModuleConfig.moduleName, "Pop", "AddPop"), popObject);
        }
        return ({
            GetFilteredPops: GetFilteredPops,
            GetPop: GetPop,
            AddPop:AddPop,
            UpdatePop: UpdatePop
        });
    }

    appControllers.service('Demo_PopAPIService', popAPIService);

})(appControllers);