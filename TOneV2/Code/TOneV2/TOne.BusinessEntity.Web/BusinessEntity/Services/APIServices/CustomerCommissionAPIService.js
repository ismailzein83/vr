app.service('CustomerCommissionAPIService', function (BaseAPIService) {

    return ({
        GetCustomerCommissions: GetCustomerCommissions
    });

    function GetCustomerCommissions(input) {
        return BaseAPIService.post('/api/CustomerCommission/GetCustomerCommissions', input);
    }

});