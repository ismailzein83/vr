'use strict'
var serviceObj = function (BaseAPIService) {
    return ({
        GetFilteredCarrierSummaryStats: GetFilteredCarrierSummaryStats
    });

    function GetFilteredCarrierSummaryStats(input) {
        return BaseAPIService.post("/api/CarrierSummaryStats/GetFilteredCarrierSummaryStats", input);
    }
}
serviceObj.$inject = ['BaseAPIService'];
appControllers.service('CarrierSummaryStatsAPIService', serviceObj);