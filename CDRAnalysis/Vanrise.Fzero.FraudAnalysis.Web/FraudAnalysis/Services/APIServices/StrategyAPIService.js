(function (appControllers) {

    'use strict';

    StrategyAPIService.$inject = ['BaseAPIService', 'UtilsService', 'CDRAnalysis_FA_ModuleConfig', 'SecurityService'];

    function StrategyAPIService(BaseAPIService, UtilsService, CDRAnalysis_FA_ModuleConfig, SecurityService) {

        var controllerName = 'Strategy';

        function GetFilteredStrategies(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(CDRAnalysis_FA_ModuleConfig.moduleName, controllerName, 'GetFilteredStrategies'), input);
        }

        function GetStrategiesInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(CDRAnalysis_FA_ModuleConfig.moduleName, controllerName, 'GetStrategiesInfo'), {
                filter: filter
            });
        }

        function GetStrategy(strategyId) {
            return BaseAPIService.get(UtilsService.getServiceURL(CDRAnalysis_FA_ModuleConfig.moduleName, controllerName, 'GetStrategy'), {
                strategyId: strategyId
            });
        }

        function GetFilters(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(CDRAnalysis_FA_ModuleConfig.moduleName, controllerName, 'GetFilters'),
                {filter:filter});
        }

        function GetAggregates() {
            return BaseAPIService.get(UtilsService.getServiceURL(CDRAnalysis_FA_ModuleConfig.moduleName, controllerName, 'GetAggregates'));
        }

        function AddStrategy(strategy) {
            return BaseAPIService.post(UtilsService.getServiceURL(CDRAnalysis_FA_ModuleConfig.moduleName, controllerName, 'AddStrategy'), strategy);
        }

        function HasAddStrategyPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(CDRAnalysis_FA_ModuleConfig.moduleName, controllerName, ['AddStrategy']));
        }

        function UpdateStrategy(strategy) {
            return BaseAPIService.post(UtilsService.getServiceURL(CDRAnalysis_FA_ModuleConfig.moduleName, controllerName, 'UpdateStrategy'), strategy);
        }


        function HasUpdateStrategyPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(CDRAnalysis_FA_ModuleConfig.moduleName, controllerName, ['UpdateStrategy']));
        }
        function GetStrategyCriteriaTemplateConfigs()
        {
            return BaseAPIService.get(UtilsService.getServiceURL(CDRAnalysis_FA_ModuleConfig.moduleName, controllerName, 'GetStrategyCriteriaTemplateConfigs'));
        }

        return ({
            HasAddStrategyPermission: HasAddStrategyPermission,
            HasUpdateStrategyPermission: HasUpdateStrategyPermission,
            GetFilteredStrategies: GetFilteredStrategies,
            GetStrategiesInfo: GetStrategiesInfo,
            GetStrategy: GetStrategy,
            GetFilters: GetFilters,
            GetAggregates: GetAggregates,
            AddStrategy: AddStrategy,
            UpdateStrategy: UpdateStrategy,
            GetStrategyCriteriaTemplateConfigs: GetStrategyCriteriaTemplateConfigs
        });
    }

    appControllers.service('StrategyAPIService', StrategyAPIService);

})(appControllers);
