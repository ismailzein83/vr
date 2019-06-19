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
                    carrierId: carrierId,
                    BED: BED,
                    offset: offset
                });
        }
        function GetDealSettingsDetail(dealId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Deal_ModuleConfig.moduleName, controllerName, 'GetDealSettingsDetail'), {
                dealId: dealId
            });
        }

        function GetLastDealInfo(dealId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Deal_ModuleConfig.moduleName, controllerName, 'GetLastDealInfo'), {
                dealId: dealId
            });
        }
        function GetLastSwapDealProgress(dealId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Deal_ModuleConfig.moduleName, controllerName, 'GetLastSwapDealProgress'), {
                dealId: dealId
            });
        }
        function GetLastVolumeCommitmentProgress(dealId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Deal_ModuleConfig.moduleName, controllerName, 'GetLastVolumeCommitmentProgress'), {
                dealId: dealId
            });
        }


        return ({
            GetDealDefinitionInfo: GetDealDefinitionInfo,
            DeleteDeal: DeleteDeal,
            GetEffectiveOnDate: GetEffectiveOnDate,
            GetDealSettingsDetail: GetDealSettingsDetail,
            GetLastDealInfo: GetLastDealInfo,
            GetLastSwapDealProgress: GetLastSwapDealProgress,
            GetLastVolumeCommitmentProgress: GetLastVolumeCommitmentProgress
        });
    } 

    app.service('WhS_Deal_DealDefinitionAPIService', DealDefinitionAPIService);

})(app);

(function (app) {

    'use strict';

    DealTimePeriodAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_Deal_ModuleConfig'];

    function DealTimePeriodAPIService(BaseAPIService, UtilsService, WhS_Deal_ModuleConfig) {

        var controllerName = 'DealTimePeriod';

        function GetDealTimePeriodTemplateConfigs(serializedFilter) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Deal_ModuleConfig.moduleName, controllerName, "GetDealTimePeriodTemplateConfigs"));
        }

        return {
            GetDealTimePeriodTemplateConfigs: GetDealTimePeriodTemplateConfigs
        };
    }

    app.service('WhS_Deal_DealTimePeriodAPIService', DealTimePeriodAPIService);
})(app);