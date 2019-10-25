(function (appControllers) {
    "use strict";

    RetailBilling_CustomCodeChargeTypeService.$inject = ['VRModalService'];

    function RetailBilling_CustomCodeChargeTypeService(VRModalService) {

        function openExpressionEditorBuilder(onSetValue, params) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onSetValue = onSetValue;
            };
            var parameters = {
                params: params
            };
            VRModalService.showModal('/Client/Modules/Retail_Billing/Elements/Views/CustomCodePricingLogicEditorTemplate.html', parameters, modalSettings);
        }

        return {
            openExpressionEditorBuilder: openExpressionEditorBuilder
        };
    }

    appControllers.service('RetailBilling_CustomCodeChargeTypeService', RetailBilling_CustomCodeChargeTypeService);
})(appControllers);