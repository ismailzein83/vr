'use strict'
var serviceObj = function (BaseAPIService) {

    return ({
        GetFilteredSupplierInvoices: GetFilteredSupplierInvoices,
        GetFilteredSupplierInvoiceDetails: GetFilteredSupplierInvoiceDetails,
        GetFilteredSupplierInvoiceDetailsGroupedByDay: GetFilteredSupplierInvoiceDetailsGroupedByDay,
        DeleteInvoice: DeleteInvoice,
        ToggleInvoiceLock: ToggleInvoiceLock
    });

    function GetFilteredSupplierInvoices(input) {
        return BaseAPIService.post('/api/SupplierInvoice/GetFilteredSupplierInvoices', input);
    }

    function GetFilteredSupplierInvoiceDetails(input) {
        return BaseAPIService.post('/api/SupplierInvoice/GetFilteredSupplierInvoiceDetails', input);
    }

    function GetFilteredSupplierInvoiceDetailsGroupedByDay(input) {
        return BaseAPIService.post('/api/SupplierInvoice/GetFilteredSupplierInvoiceDetailsGroupedByDay', input);
    }

    function DeleteInvoice(invoiceID) {
        return BaseAPIService.get('/api/SupplierInvoice/DeleteInvoice', {
            invoiceID: invoiceID
        });
    }

    function ToggleInvoiceLock(invoiceID) {
        return BaseAPIService.get('/api/SupplierInvoice/ToggleInvoiceLock', {
            invoiceID: invoiceID
        });
    }
}

serviceObj.$inject = ['BaseAPIService'];
appControllers.service('SupplierInvoiceAPIService', serviceObj);
