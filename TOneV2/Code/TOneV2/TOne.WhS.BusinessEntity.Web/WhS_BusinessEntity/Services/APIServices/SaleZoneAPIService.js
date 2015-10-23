
app.service('WhS_BE_SaleZoneAPIService', function (BaseAPIService) {
    
    return ({
        GetSaleZonesInfo: GetSaleZonesInfo,
        GetSaleZonesInfoByIds: GetSaleZonesInfoByIds,
        GetSaleZoneGroupTemplates: GetSaleZoneGroupTemplates,
        GetSaleZonesByName: GetSaleZonesByName
    });
     

    function GetSaleZonesInfo(sellingNumberPlanId, filter) {
        return BaseAPIService.get("/api/SaleZone/GetSaleZonesInfo", {
            sellingNumberPlanId: sellingNumberPlanId,
            filter: filter
        });
    }

    function GetSaleZonesInfoByIds(input) {
        return BaseAPIService.post("/api/SaleZone/GetSaleZonesInfoByIds", input);
    }

    function GetSaleZoneGroupTemplates() {
        return BaseAPIService.get("/api/SaleZone/GetSaleZoneGroupTemplates");
    }

    function GetSaleZonesByName(customerId, saleZoneNameFilter) {
        return BaseAPIService.get("/api/SaleZone/GetSaleZonesByName", {
            customerId: customerId,
            saleZoneNameFilter: saleZoneNameFilter
        });
    }

});