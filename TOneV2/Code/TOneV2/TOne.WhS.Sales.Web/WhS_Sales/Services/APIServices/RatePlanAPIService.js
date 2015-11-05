(function (appControllers) {

    "use strict";

    ratePlanAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_Sales_ModuleConfig"];

    function ratePlanAPIService(BaseAPIService, UtilsService, WhS_Sales_ModuleConfig) {

        return ({
            GetRatePlanItems: GetRatePlanItems,
            GetRatePlan: GetRatePlan,
            SavePriceList: SavePriceList,
            SaveRatePlanDraft: SaveRatePlanDraft
        });

        function GetRatePlanItems(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, "RatePlan", "GetRatePlanItems"), input);
        }

        function GetRatePlan(ownerType, ownerId, status) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, "RatePlan", "GetRatePlan"), {
                ownerType: ownerType,
                ownerId: ownerId,
                status: status
            });
        }

        function SavePriceList(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, "RatePlan", "SavePriceList"), input);
        }

        function SaveRatePlanDraft(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, "RatePlan", "SaveRatePlanDraft"), input);
        }
    }

    appControllers.service("WhS_Sales_RatePlanAPIService", ratePlanAPIService);

})(appControllers);