(function (appControllers) {

    'use strict';

    DealAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_Deal_ModuleConfig', 'SecurityService'];

    function DealAPIService(BaseAPIService, UtilsService, WhS_Deal_ModuleConfig, SecurityService) {
        var controllerName = 'DealDefinition';

        function GetFilteredSwapDeals(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Deal_ModuleConfig.moduleName, controllerName, 'GetFilteredSwapDeals'), input);
        }
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
        function GetSwapDealSettingData() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Deal_ModuleConfig.moduleName, controllerName, 'GetSwapDealSettingData'));
        }
        return ({
            GetFilteredSwapDeals: GetFilteredSwapDeals,
            GetFilteredVolCommitmentDeals:GetFilteredVolCommitmentDeals,
            GetDeal: GetDeal,
            AddDeal: AddDeal,
            UpdateDeal: UpdateDeal,
            HasAddDealPermission: HasAddDealPermission,
            HasEditDealPermission: HasEditDealPermission,
            GetSwapDealSettingData: GetSwapDealSettingData
        });
    }

    appControllers.service('WhS_Deal_DealAPIService', DealAPIService);

})(appControllers);