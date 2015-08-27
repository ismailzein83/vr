'use strict'
var serviceObj = function (BaseAPIService) {

    return ({
        GetFilteredSupplierInvoices: GetFilteredSupplierInvoices
    });

    function GetFilteredSupplierInvoices(input) {
        return BaseAPIService.post('/api/SupplierInvoice/GetFilteredSupplierInvoices', input);
    }
}

serviceObj.$inject = ['BaseAPIService'];
appControllers.service('SupplierInvoiceAPIService', serviceObj);
