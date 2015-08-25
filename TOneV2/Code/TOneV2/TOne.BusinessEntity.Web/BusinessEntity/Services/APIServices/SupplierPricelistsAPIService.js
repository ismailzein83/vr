app.service('SupplierPricelistsAPIService', function (BaseAPIService) {

    return ({
        GetSupplierPriceLists: GetSupplierPriceLists,
        GetSupplierPriceListDetails: GetSupplierPriceListDetails
    });

    function GetSupplierPriceLists(input) {
        return BaseAPIService.post('/api/SupplierPriceLists/GetSupplierPriceLists', input);
    }
    function GetSupplierPriceListDetails(input) {
        return BaseAPIService.post('/api/SupplierPriceLists/GetSupplierPriceListDetails', input);
    }
});