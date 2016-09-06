(function (appControllers) {

    "use strict";
    popAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_BE_ModuleConfig'];

    function popAPIService(BaseAPIService, UtilsService, Retail_BE_ModuleConfig) {

        function GetFilteredPops(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, "Pop", "GetFilteredPops"), input);
        }

        function GetPop(popId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, "Pop", "GetPop"), {
                popId: popId
            });

        }
       
        function UpdatePop(popObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, "Pop", "UpdatePop"), popObject);
        }
        function AddPop(popObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, "Pop", "AddPop"), popObject);
        }
        return ({
            GetFilteredPops: GetFilteredPops,
            GetPop: GetPop,
            AddPop:AddPop,
            UpdatePop: UpdatePop
        });
    }

    appControllers.service('Retail_BE_PopAPIService', popAPIService);

})(appControllers);