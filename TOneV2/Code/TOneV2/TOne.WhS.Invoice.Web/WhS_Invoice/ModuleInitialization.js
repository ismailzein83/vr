app.run(['WhS_Invoice_InvoiceAccountService', 'WhS_Invoice_InvoiceService', function (WhS_Invoice_InvoiceAccountService, WhS_Invoice_InvoiceService) {
    WhS_Invoice_InvoiceAccountService.registerDrillDownToCarrierAccount();
    WhS_Invoice_InvoiceAccountService.registerDrillDownToCarrierProfile();
    WhS_Invoice_InvoiceService.registerCompareAction();

}]);

