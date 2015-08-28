'use strict'
var serviceObj = function (BaseAPIService) {

    return ({
        GetFilteredSupplierInvoices: GetFilteredSupplierInvoices,
        GetFilteredSupplierInvoiceDetails: GetFilteredSupplierInvoiceDetails,
        DeleteInvoice: DeleteInvoice
    });

    function GetFilteredSupplierInvoices(input) {
        return BaseAPIService.post('/api/SupplierInvoice/GetFilteredSupplierInvoices', input);
    }

    function GetFilteredSupplierInvoiceDetails(input) {
        return BaseAPIService.post('/api/SupplierInvoice/GetFilteredSupplierInvoiceDetails', input);
    }

    function DeleteInvoice(invoiceID) {
        return BaseAPIService.get('/api/SupplierInvoice/DeleteSupplierInvoice', {
            invoiceID: invoiceID
        });
    }
}

serviceObj.$inject = ['BaseAPIService'];
appControllers.service('SupplierInvoiceAPIService', serviceObj);
