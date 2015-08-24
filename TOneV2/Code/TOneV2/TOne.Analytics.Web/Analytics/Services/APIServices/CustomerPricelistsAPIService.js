app.service('CustomerPricelistsAPIService', function (BaseAPIService) {

    return ({
        GetCustomerPriceLists: GetCustomerPriceLists
    });

    function GetCustomerPriceLists(input) {
        return BaseAPIService.post('/api/CustomerPriceLists/GetCustomerPriceLists', input);
    }
});