
app.service('WhS_BE_SaleZonePackageAPIService', function (BaseAPIService) {

    return ({

        GetSaleZonePackages: GetSaleZonePackages,
    });


    function GetSaleZonePackages() {
        return BaseAPIService.get("/api/SaleZonePackage/GetSaleZonePackages");
    }

});