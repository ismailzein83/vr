(function (appControllers) {


    "use strict";
    ExcludedItemsAPIServicce.$inject = ['BaseAPIService', 'UtilsService', 'WhS_Sales_ModuleConfig'];

    function ExcludedItemsAPIServicce(BaseAPIService, UtilsService, WhS_Sales_ModuleConfig) {
        var controllerName = "ExcludedItemsController";

        function GetFilteredExcludedItems(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, "GetFilteredExcludedItems"), input);
        }
        return ({
            GetFilteredExcludedItems: GetFilteredExcludedItems
        });
    }

    appControllers.service('WhS_Sales_ExcludedItemsAPIService', ExcludedItemsAPIServicce);

})(appControllers);