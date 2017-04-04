app.run(['WhS_Invoice_InvoiceAccountService', function (WhS_Invoice_InvoiceAccountService) {
    WhS_Invoice_InvoiceAccountService.registerDrillDownToCarrierAccount();
    WhS_Invoice_InvoiceAccountService.registerDrillDownToCarrierProfile();

}]);

