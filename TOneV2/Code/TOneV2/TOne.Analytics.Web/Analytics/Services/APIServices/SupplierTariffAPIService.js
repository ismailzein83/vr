app.service('SupplierTariffAPIService', function (BaseAPIService) {

    return ({
        GetFilteredSupplierTariffs: GetFilteredSupplierTariffs
    });

    function GetFilteredSupplierTariffs(input) {
        return BaseAPIService.post('/api/SupplierTariff/GetFilteredSupplierTariffs', input);
    }
});