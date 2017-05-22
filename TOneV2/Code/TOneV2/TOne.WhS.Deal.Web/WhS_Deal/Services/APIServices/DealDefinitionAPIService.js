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

        return ({
            GetDealDefinitionInfo: GetDealDefinitionInfo
        });
    }

    app.service('WhS_Deal_DealDefinitionAPIService', DealDefinitionAPIService);

})(app);