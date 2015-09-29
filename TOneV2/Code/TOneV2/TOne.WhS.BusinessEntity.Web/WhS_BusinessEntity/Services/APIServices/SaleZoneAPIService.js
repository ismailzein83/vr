
app.service('WhS_BE_SaleZoneAPIService', function (BaseAPIService) {

    return ({
        GetSaleZonesInfo: GetSaleZonesInfo,
        GetSaleZonesInfoByIds: GetSaleZonesInfoByIds
    });
     

    function GetSaleZonesInfo(packageId, filter) {
        return BaseAPIService.get("/api/SaleZone/GetSaleZonesInfo", {
            packageId: packageId,
            filter: filter
        });
    }

    function GetSaleZonesInfoByIds(input) {
        return BaseAPIService.post("/api/SaleZone/GetSaleZonesInfoByIds", input);
    }

});