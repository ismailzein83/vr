
app.service('WhS_BE_SaleZoneAPIService', function (BaseAPIService) {

    return ({
        GetSaleZonesInfo: GetSaleZonesInfo,
    });
     

    function GetSaleZonesInfo(packageId, filter) {
        return BaseAPIService.get("/api/SaleZone/GetSaleZonesInfo", {
            packageId: packageId,
            filter: filter
        });
    }

    function GetSaleZonesInfoByIds(packageId, saleZoneIds) {
        return BaseAPIService.get("/api/SaleZone/GetSaleZonesInfoByIds", {
            packageId: packageId,
            saleZoneIds: saleZoneIds
        });
    }

});