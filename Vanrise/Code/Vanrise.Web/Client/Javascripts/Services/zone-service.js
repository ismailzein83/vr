'use strict';

app.service('ZonesService', function (HttpService, MainService) {

    return ({
        getSalesZones: getSalesZones,
        getZoneList: getZoneList
    });

    function getSalesZones(filterzone) {

        var getSalesZonesURL = MainService.getBaseURL() + "/api/BusinessEntity/GetSalesZones";
        return HttpService.get(getSalesZonesURL, { nameFilter: filterzone });
    }

    function getZoneList(ZonesIds) {
        var getSalesZonesURL = MainService.getBaseURL() + "/api/BusinessEntity/GetZoneList";
        return HttpService.get(getSalesZonesURL, { ZonesIds: ZonesIds });
    }

});