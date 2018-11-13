(function (app) {

    'use strict';

    DealDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_Deal_ModuleConfig'];

    function DealDefinitionAPIService(BaseAPIService, UtilsService, WhS_Deal_ModuleConfig) {
        var controllerName = 'DealDefinition';

        function GetDealDefinitionInfo(serializedFilter) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Deal_ModuleConfig.moduleName, controllerName, "GetDealDefinitionInfo"),
             {
                 serializedFilter: serializedFilter
             });
        }

        function DeleteDeal(dealId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Deal_ModuleConfig.moduleName, controllerName, "DeleteDeal"),
                {
                    dealId: dealId
                });
        }
        function GetEffectiveOnDate(isSale, isShifted, carrierId, BED, offset) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Deal_ModuleConfig.moduleName, controllerName, "GetEffectiveOnDate"),
                {
                    isSale: isSale,
                    isShifted: isShifted,
                    carrierId:carrierId,
                    BED: BED,
                    offset: offset
                });
        }

        return ({
            GetDealDefinitionInfo: GetDealDefinitionInfo,
            DeleteDeal: DeleteDeal,
            GetEffectiveOnDate: GetEffectiveOnDate
        });
    }

    app.service('WhS_Deal_DealDefinitionAPIService', DealDefinitionAPIService);

})(app);