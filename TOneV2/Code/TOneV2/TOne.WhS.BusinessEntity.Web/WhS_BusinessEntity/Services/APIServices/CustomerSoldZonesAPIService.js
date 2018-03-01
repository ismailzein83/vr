(function (app) {

    "use strict";
    customerSoldZones.$inject = ["BaseAPIService", "UtilsService", "WhS_BE_ModuleConfig"];

    function customerSoldZones(BaseAPIService, UtilsService, WhS_BE_ModuleConfig) {

        var controllerName = "CustomerSoldZones";

        function GetFilteredCustomerSoldZones(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetFilteredCustomerSoldZones"), input);
        }


        return ({
            GetFilteredCustomerSoldZones: GetFilteredCustomerSoldZones
        });
    }

    app.service("WhS_BE_CustomerSoldZones", customerSoldZones);
})(app);