app.service('SupplierCommissionAPIService', function (BaseAPIService) {

    return ({
        GetSupplierCommissions: GetSupplierCommissions
    });

    function GetSupplierCommissions(input) {
        return BaseAPIService.post('/api/SupplierCommission/GetSupplierCommissions', input);
    }

});