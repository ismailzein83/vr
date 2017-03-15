app.run(['Retail_BE_AccountActionService', 'Retail_BE_PackageService', function (Retail_BE_AccountActionService, Retail_BE_PackageService) {
    Retail_BE_AccountActionService.registerEditAccount();
    Retail_BE_AccountActionService.registerOpen360DegreeAccount();
    Retail_BE_AccountActionService.registerBPActionAccount();
    Retail_BE_PackageService.registerObjectTrackingDrillDownToPackage();
   
}]);

