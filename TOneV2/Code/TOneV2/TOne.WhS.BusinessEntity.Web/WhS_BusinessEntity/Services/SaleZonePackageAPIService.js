
app.service('SaleZonePackageAPIService', function (BaseAPIService) {

    return ({

        GetSaleZonePackages: GetSaleZonePackages,
    });


    function GetSaleZonePackages() {
        return BaseAPIService.get("/api/SaleZone/GetSaleZonePackages");
    }

});