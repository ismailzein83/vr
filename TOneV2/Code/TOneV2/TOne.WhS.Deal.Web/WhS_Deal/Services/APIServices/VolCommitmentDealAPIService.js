(function (app) {

    'use strict';

    VolCommitmentDealAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_Deal_ModuleConfig', 'SecurityService'];

    function VolCommitmentDealAPIService(BaseAPIService, UtilsService, WhS_Deal_ModuleConfig, SecurityService) {
        var controllerName = 'VolCommitmentDeal';
        function GetFilteredVolCommitmentDeals(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Deal_ModuleConfig.moduleName, controllerName, 'GetFilteredVolCommitmentDeals'), input);
        }
        function GetDeal(dealId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Deal_ModuleConfig.moduleName, controllerName, 'GetDeal'), {
                dealId: dealId
            });
        }
        function AddDeal(deal) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Deal_ModuleConfig.moduleName, controllerName, 'AddDeal'), deal);
        }

        function UpdateDeal(deal) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Deal_ModuleConfig.moduleName, controllerName, 'UpdateDeal'), deal);
        }

        function HasAddDealPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(WhS_Deal_ModuleConfig.moduleName, controllerName, ['AddDeal']));
        }
        function HasEditDealPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(WhS_Deal_ModuleConfig.moduleName, controllerName, ['UpdateDeal']));
        }
        function GetVolumeCommitmentHistoryDetailbyHistoryId(volumeCommitmentHistoryId) {

            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Deal_ModuleConfig.moduleName, controllerName, 'GetVolumeCommitmentHistoryDetailbyHistoryId'), {
                volumeCommitmentHistoryId: volumeCommitmentHistoryId
            });
        }
        function GetSaleRateEvaluatorConfigurationTemplateConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Deal_ModuleConfig.moduleName, controllerName, "GetSaleRateEvaluatorConfigurationTemplateConfigs"));
        }
        function GetSupplierRateEvaluatorConfigurationTemplateConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Deal_ModuleConfig.moduleName, controllerName, "GetSupplierRateEvaluatorConfigurationTemplateConfigs"));
        }
        return ({
            GetFilteredVolCommitmentDeals: GetFilteredVolCommitmentDeals,
            GetDeal: GetDeal,
            AddDeal: AddDeal,
            UpdateDeal: UpdateDeal,
            HasAddDealPermission: HasAddDealPermission,
            HasEditDealPermission: HasEditDealPermission,
            GetVolumeCommitmentHistoryDetailbyHistoryId: GetVolumeCommitmentHistoryDetailbyHistoryId,
            GetSaleRateEvaluatorConfigurationTemplateConfigs: GetSaleRateEvaluatorConfigurationTemplateConfigs,
            GetSupplierRateEvaluatorConfigurationTemplateConfigs: GetSupplierRateEvaluatorConfigurationTemplateConfigs
        });
    }

    app.service('WhS_Deal_VolCommitmentDealAPIService', VolCommitmentDealAPIService);

})(app);