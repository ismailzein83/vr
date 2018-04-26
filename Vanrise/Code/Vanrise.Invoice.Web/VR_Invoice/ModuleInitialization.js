app.run(['VR_Invoice_InvoiceActionService', 'VR_Invoice_InvoiceTypeService', function (VR_Invoice_InvoiceActionService, VR_Invoice_InvoiceTypeService) {
    VR_Invoice_InvoiceActionService.registerInvoiceRDLCReport();
    VR_Invoice_InvoiceActionService.registerSetInvoicePaidAction();
    VR_Invoice_InvoiceActionService.registerSetInvoiceLockedAction();
    VR_Invoice_InvoiceActionService.registerRecreateAction();
    VR_Invoice_InvoiceActionService.registerInvoiceNoteAction();
    VR_Invoice_InvoiceActionService.registerSendEmailAction();
    VR_Invoice_InvoiceTypeService.registerObjectTrackingDrillDownToInvoiceType();
    VR_Invoice_InvoiceActionService.registerDownloadFileInvoiceAction();
    VR_Invoice_InvoiceActionService.registerSetInvoiceDeletedAction();
    VR_Invoice_InvoiceActionService.registerApproveInvoiceAction();
}]);

      