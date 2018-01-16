app.run(['Retail_Teles_TelesAccountActionService', function (Retail_Teles_TelesAccountActionService) {
    Retail_Teles_TelesAccountActionService.registerMappingTelesAccount();
    Retail_Teles_TelesAccountActionService.registerMappingTelesSite();
    Retail_Teles_TelesAccountActionService.registerMappingTelesUser();
    Retail_Teles_TelesAccountActionService.registerChangeUserRoutingGroup();
}]);

