
app.service('SaleZoneAPIService', function (BaseAPIService) {

    return ({

        GetSaleZones: GetSaleZones,
    });
     

    function GetSaleZones(packageId) {
        return BaseAPIService.get("/api/SaleZone/GetSaleZones", {
            packageId: packageId
        });
    }

});