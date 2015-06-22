var BusinessProcessAPIService = function (BaseAPIService) {

    "use strict";

    function GetDefinitions() {
        return BaseAPIService.get("/api/BusinessProcess/GetDefinitions");
    }

    function GetStatusList() {
        return BaseAPIService.get("/api/BusinessProcess/GetStatusList");
    }

    function GetFilteredBProcess(param) {
        return BaseAPIService.post("/api/BusinessProcess/GetFilteredBProcess", param );
    }

    return ({
        GetDefinitions: GetDefinitions,
        GetStatusList: GetStatusList,
        GetFilteredBProcess: GetFilteredBProcess
    });

}
BusinessProcessAPIService.$inject = ['BaseAPIService'];
appControllers.service('BusinessProcessAPIService', BusinessProcessAPIService);