(function (appControllers) {

    'use strict';

    SupplierSMSRatePlanService.$inject = ['VRModalService'];
    function SupplierSMSRatePlanService(VRModalService) {

        function addSMSRates(selectedSupplier, onSupplierSMSRatesApplied) {
            var parameters = {
                supplierInfo: selectedSupplier
            };

            var modalSettings = {
                onScopeReady: function (modalScope) {
                    modalScope.title = "Add SMS Rates for Supplier '" + selectedSupplier.Name + "'";
                    modalScope.onSupplierSMSRatesApplied = onSupplierSMSRatesApplied;
                }
            };
            VRModalService.showModal("/Client/Modules/WhS_SMSBusinessEntity/Views/SupplierSMSRatePlanEditor.html", parameters, modalSettings);
        }

        function viewFutureSMSRate(mobileNetworkName, futureSMSRate) {
            var parameters = {
                mobileNetworkName: mobileNetworkName,
                futureSMSRate: futureSMSRate
            };

            var settings = {};

            VRModalService.showModal("/Client/Modules/WhS_SMSBusinessEntity/Views/FutureSMSRate.html", parameters, settings);
        }

        return {
            addSMSRates: addSMSRates,
            viewFutureSMSRate: viewFutureSMSRate
        };
    }

    appControllers.service('WhS_SMSBusinessEntity_SupplierRatePlanService', SupplierSMSRatePlanService);

})(appControllers); 