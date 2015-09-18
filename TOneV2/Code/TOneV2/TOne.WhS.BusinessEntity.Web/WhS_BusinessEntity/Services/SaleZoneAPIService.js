
app.service('SaleZoneAPIService', function (BaseAPIService) {

    return ({

        GetSaleZones: GetSaleZones,
    });
     

    function GetSaleZones() {
        return BaseAPIService.get("/api/SaleZone/GetSaleZones");
    }

});