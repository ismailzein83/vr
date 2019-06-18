﻿(function (app) {

    'use strict';

    SwapDealAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_Deal_ModuleConfig', 'SecurityService'];

    function SwapDealAPIService(BaseAPIService, UtilsService, WhS_Deal_ModuleConfig, SecurityService) {
        var controllerName = 'SwapDeal';

        function GetFilteredSwapDeals(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Deal_ModuleConfig.moduleName, controllerName, 'GetFilteredSwapDeals'), input);
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
        function HasViewSwapDealPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(WhS_Deal_ModuleConfig.moduleName, controllerName, ['GetFilteredSwapDeals']));
        }
        function GetSwapDealSettingData() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Deal_ModuleConfig.moduleName, controllerName, 'GetSwapDealSettingData'));
        }
        function GetSwapDealHistoryDetailbyHistoryId(swapDealHistoryId) {

            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Deal_ModuleConfig.moduleName, controllerName, 'GetSwapDealHistoryDetailbyHistoryId'), {
                swapDealHistoryId: swapDealHistoryId
            });
        }
        function getSwapDealFromAnalysis(genericBusinessEntityId, businessEntityDefinitionId) {

            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Deal_ModuleConfig.moduleName, controllerName, 'GetSwapDealFromAnalysis'), {
                genericBusinessEntityId: genericBusinessEntityId,
                businessEntityDefinitionId: businessEntityDefinitionId
            });
        }

        function RecurDeal(dealId, recurringNumber, recurringType) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Deal_ModuleConfig.moduleName, controllerName, "RecurDeal"), {
                dealId: dealId,
                recurringNumber: recurringNumber,
                recurringType: recurringType
            });
        }

        return ({
            GetFilteredSwapDeals: GetFilteredSwapDeals,
            GetDeal: GetDeal,
            AddDeal: AddDeal,
            UpdateDeal: UpdateDeal,
            HasAddDealPermission: HasAddDealPermission,
            HasEditDealPermission: HasEditDealPermission,
            HasViewSwapDealPermission: HasViewSwapDealPermission,
            GetSwapDealSettingData: GetSwapDealSettingData,
            GetSwapDealHistoryDetailbyHistoryId: GetSwapDealHistoryDetailbyHistoryId,
            RecurDeal: RecurDeal,
            getSwapDealFromAnalysis: getSwapDealFromAnalysis
        });
    }

    app.service('WhS_Deal_SwapDealAPIService', SwapDealAPIService);

})(app);