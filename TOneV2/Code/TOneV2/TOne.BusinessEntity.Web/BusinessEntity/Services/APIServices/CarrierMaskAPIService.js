'use strict'
var serviceObj = function (BaseAPIService) {
    return ({
        GetCarrierMasks: GetCarrierMasks
    });

    function GetCarrierMasks() {
        return BaseAPIService.get("/api/CarrierMask/GetCarrierMasks");
    }
}
serviceObj.$inject = ['BaseAPIService'];
appControllers.service('CarrierMaskAPIService', serviceObj);