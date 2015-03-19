'use strict'

app.service('ZonesService', function (HttpService, MainService) {

    return ({
        getSalesZones: getSalesZones
    });

    function getSalesZones(filterzone) {

        var getSalesZonesURL = MainService.getBaseURL() + "/api/BusinessEntity/GetSalesZones";
        return HttpService.get(getSalesZonesURL, { nameFilter: filterzone });
    }

});