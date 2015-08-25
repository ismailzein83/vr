app.service('SupplierTariffAPIService', function (BaseAPIService) {

    return ({
        GetFilteredSupplierTariffs: GetFilteredSupplierTariffs,
        GetZonesBySupplierID: GetZonesBySupplierID
    });

    function GetFilteredSupplierTariffs(input) {
        return BaseAPIService.post('/api/SupplierTariff/GetFilteredSupplierTariffs', input);
    }

    function GetZonesBySupplierID(supplierID) {
        return BaseAPIService.get('/api/SupplierTariff/GetZonesBySupplierID', {
            supplierID: supplierID
        });
    }
});
