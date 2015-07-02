
PriceListAPIService.$inject = ['BaseAPIService'];
function PriceListAPIService(BaseAPIService) {
    'use strict';
    return({
        GetPriceList: GetPriceList
    });
    function GetPriceList() {
        return BaseAPIService.get("/api/PriceList/GetPriceList");
    }
}

appControllers.service('PriceListAPIService', PriceListAPIService);