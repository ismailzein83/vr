app.run(['NP_IVSwitch_EndPointService',
function (NP_IVSwitch_EndPointService) {
    NP_IVSwitch_EndPointService.registerDrillDownToCarrierAccount();
}]);

app.run(['NP_IVSwitch_RouteService',
function (NP_IVSwitch_RouteService) {
    NP_IVSwitch_RouteService.registerDrillDownToCarrierAccount();
}]);

