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

        return ({
            GetDealDefinitionInfo: GetDealDefinitionInfo,
            DeleteDeal: DeleteDeal
        });
    }

    app.service('WhS_Deal_DealDefinitionAPIService', DealDefinitionAPIService);

})(app);