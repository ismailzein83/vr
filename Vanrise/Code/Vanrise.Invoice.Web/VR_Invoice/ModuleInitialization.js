app.run(['VR_Invoice_InvoiceActionService', function (VR_Invoice_InvoiceActionService) {
    VR_Invoice_InvoiceActionService.registerInvoiceRDLCReport();
    VR_Invoice_InvoiceActionService.registerSetInvoicePaidAction();
}]);

