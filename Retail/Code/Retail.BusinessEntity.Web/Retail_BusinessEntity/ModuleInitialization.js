app.run(['Retail_BE_AccountActionService', 'Retail_BE_PackageService', 'Retail_BE_ProductService','Retail_BE_DIDService', function (Retail_BE_AccountActionService, Retail_BE_PackageService, Retail_BE_ProductService,Retail_BE_DIDService) {
    Retail_BE_AccountActionService.registerEditAccount();
    Retail_BE_AccountActionService.registerOpen360DegreeAccount();
    Retail_BE_AccountActionService.registerBPActionAccount();
    Retail_BE_PackageService.registerObjectTrackingDrillDownToPackage();
    Retail_BE_ProductService.registerObjectTrackingDrillDownToProduct();
    Retail_BE_DIDService.registerObjectTrackingDrillDownToDID();
    Retail_BE_AccountActionService.registerChangeStatusAction();
    
}]);

