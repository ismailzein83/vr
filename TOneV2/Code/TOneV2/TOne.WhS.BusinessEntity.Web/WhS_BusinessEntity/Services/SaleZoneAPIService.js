
app.service('WhS_BE_SaleZoneAPIService', function (BaseAPIService) {

    return ({

        GetSaleZones: GetSaleZones,
    });
     

    function GetSaleZones(packageId) {
        return BaseAPIService.get("/api/SaleZone/GetSaleZones", {
            packageId: packageId
        });
    }

});