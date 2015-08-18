'use strict'
var serviceObj = function (BaseAPIService) {
    return ({
        GetConnectionByCarrierType: GetConnectionByCarrierType,
    });
    function GetConnectionByCarrierType(type) {
        return BaseAPIService.get("/api/CarrierAccountConnection/GetConnectionByCarrierType",
            {
                type: type
            });
    }
}
serviceObj.$inject = ['BaseAPIService'];
appControllers.service('CarrierAccountConnectionAPIService', serviceObj);