'use strict'
var serviceObj = function (BaseAPIService) {
    return ({
        GetCarrierMasks: GetCarrierMasks
    });

    function GetCarrierMasks(input) {
        return BaseAPIService.post("/api/CarrierMask/GetCarrierMasks", input);
    }
}
serviceObj.$inject = ['BaseAPIService'];
appControllers.service('CarrierMaskAPIService', serviceObj);