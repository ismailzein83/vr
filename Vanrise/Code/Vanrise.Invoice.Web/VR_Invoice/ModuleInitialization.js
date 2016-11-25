app.run(['VR_Invoice_InvoiceActionService', function (VR_Invoice_InvoiceActionService) {
    VR_Invoice_InvoiceActionService.registerInvoiceRDLCReport();
    VR_Invoice_InvoiceActionService.registerSetInvoicePaidAction();
    VR_Invoice_InvoiceActionService.registerSetInvoiceLockedAction();
    VR_Invoice_InvoiceActionService.registerRecreateAction();
}]);

      