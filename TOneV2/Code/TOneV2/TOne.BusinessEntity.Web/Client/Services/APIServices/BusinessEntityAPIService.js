'use strict'
var serviceObj = function (BaseAPIService) {

    return ({
        GetCarriers: GetCarriers,
        GetCodeGroups: GetCodeGroups,
        GetSwitches: GetSwitches,
        insertCarrierTest: insertCarrierTest
    });

    function GetCarriers(carrierType) {
        return BaseAPIService.get("/api/BusinessEntity/GetCarriers",
            {
                carrierType: carrierType
            });
    }
    function insertCarrierTest(CarrierAccountID, Name) {
        return BaseAPIService.post("/api/BusinessEntity/insertCarrierTest",
            {
                CarrierAccountID: CarrierAccountID,
                Name: Name
            });
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
}

serviceObj.$inject = ['BaseAPIService'];
appControllers.service('BusinessEntityAPIService', serviceObj);
