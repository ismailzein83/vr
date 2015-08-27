'use strict'
var serviceObj = function (BaseAPIService) {

    return ({
        GetFilteredSupplierInvoices: GetFilteredSupplierInvoices,
        GetFilteredSupplierInvoiceDetails: GetFilteredSupplierInvoiceDetails
    });

    function GetFilteredSupplierInvoices(input) {
        return BaseAPIService.post('/api/SupplierInvoice/GetFilteredSupplierInvoices', input);
    }

    function GetFilteredSupplierInvoiceDetails(input) {
        return BaseAPIService.post('/api/SupplierInvoice/GetFilteredSupplierInvoiceDetails', input);
    }
}

serviceObj.$inject = ['BaseAPIService'];
appControllers.service('SupplierInvoiceAPIService', serviceObj);
