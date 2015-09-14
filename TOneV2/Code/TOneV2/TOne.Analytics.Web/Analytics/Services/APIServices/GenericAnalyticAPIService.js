'use strict'
var serviceObj = function (BaseAPIService) {
    return ({
        GetFiltered: GetFiltered
    });

    function GetFiltered(input) {
        console.log(input);
        return BaseAPIService.post("/api/GenericAnalytic/GetFiltered", input);
    }
}
serviceObj.$inject = ['BaseAPIService'];
appControllers.service('GenericAnalyticAPIService', serviceObj);