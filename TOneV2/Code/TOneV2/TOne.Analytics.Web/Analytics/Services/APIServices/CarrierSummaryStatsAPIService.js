'use strict'
var serviceObj = function (BaseAPIService) {
    return ({
        GetFilteredCarrierSummaryStats: GetFilteredCarrierSummaryStats,
        GetFiltered: GetFiltered
    });

    function GetFilteredCarrierSummaryStats(input) {
        return BaseAPIService.post("/api/CarrierSummaryStats/GetFilteredCarrierSummaryStats", input);
    }

    function GetFiltered(input) {
        return BaseAPIService.post("/api/CarrierSummaryStats/GetFiltered", input);
    }
}
serviceObj.$inject = ['BaseAPIService'];
appControllers.service('CarrierSummaryStatsAPIService', serviceObj);