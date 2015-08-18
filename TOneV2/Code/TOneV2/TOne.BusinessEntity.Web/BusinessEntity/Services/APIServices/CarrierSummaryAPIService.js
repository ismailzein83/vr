'use strict'
var serviceObj = function (BaseAPIService) {
    return ({
        GetFilteredCarrierMasks: GetFilteredCarrierMasks,
        GetCarrierMask: GetCarrierMask,
        UpdateCarrierMask: UpdateCarrierMask,
        AddCarrierMask: AddCarrierMask
    });

    function GetFilteredCarrierMasks(input) {
        return BaseAPIService.post("/api/CarrierMask/GetFilteredCarrierMasks", input);
    }
    function GetCarrierMask(carrierMaskId) {
        return BaseAPIService.get("/api/CarrierMask/GetCarrierMask",
            {
                carrierMaskId: carrierMaskId
            });
    }
    function UpdateCarrierMask(carrierMask) {
        return BaseAPIService.post("/api/CarrierMask/UpdateCarrierMask", carrierMask);
    }
    function AddCarrierMask(carrierMask) {
        return BaseAPIService.post("/api/CarrierMask/AddCarrierMask", carrierMask);
    }
}
serviceObj.$inject = ['BaseAPIService'];
appControllers.service('CarrierSummaryAPIService', serviceObj);