'use strict'
var serviceObj = function (BaseAPIService) {

    return ({
        GetCodeGroups: GetCodeGroups,
        GetSwitches: GetSwitches,
        GetSalesZones: GetSalesZones,
        GetEntityNodes: GetEntityNodes
    });


    function GetEntityNodes() {
        return BaseAPIService.get("/api/BusinessEntity/GetEntityNodes");
    }

    function GetCodeGroups() {
        return BaseAPIService.get("/api/BusinessEntity/GetCodeGroups",
            {
            });
    }

    function GetSwitches() {
        return BaseAPIService.get("/api/BusinessEntity/GetSwitches",
            {
            });
    }
    function GetSalesZones(nameFilter) {
        return BaseAPIService.get("/api/BusinessEntity/GetSalesZones",
            {
                nameFilter: nameFilter
            });
    }
}

serviceObj.$inject = ['BaseAPIService'];
appControllers.service('BusinessEntityAPIService_temp', serviceObj);
