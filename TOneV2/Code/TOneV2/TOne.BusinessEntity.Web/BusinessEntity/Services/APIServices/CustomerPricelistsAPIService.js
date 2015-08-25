app.service('CustomerPricelistsAPIService', function (BaseAPIService) {

    return ({
        GetCustomerPriceLists: GetCustomerPriceLists,
        GetCustomerPriceListDetails: GetCustomerPriceListDetails
    });

    function GetCustomerPriceLists(input) {
        return BaseAPIService.post('/api/CustomerPriceLists/GetCustomerPriceLists', input);
    }
    function GetCustomerPriceListDetails(input) {
        return BaseAPIService.post('/api/CustomerPriceLists/GetCustomerPriceListDetails', input);
    }
});