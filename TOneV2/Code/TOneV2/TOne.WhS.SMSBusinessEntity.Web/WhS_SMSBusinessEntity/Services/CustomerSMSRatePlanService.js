(function (appControllers) {

    'use strict';

    CustomerSMSRatePlanService.$inject = ['VRModalService'];
    function CustomerSMSRatePlanService(VRModalService) {

        function addSMSRates(selectedCustomer, onDraftSaved, onSaleSMSRatesApplied) {
            var parameters = {
                customerInfo: selectedCustomer
            };

            var modalSettings = {
                onScopeReady: function (modalScope) {
                    modalScope.title = "Add SMS Rates for Customer '" + selectedCustomer.Name + "'";
                    modalScope.onDraftSaved = onDraftSaved;
                    modalScope.onSaleSMSRatesApplied = onSaleSMSRatesApplied;
                }
            };
            VRModalService.showModal("/Client/Modules/WhS_SMSBusinessEntity/Views/CustomerSMSRatePlanEditor.html", parameters, modalSettings);
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

    appControllers.service('WhS_SMSBusinessEntity_CustomerRatePlanService', CustomerSMSRatePlanService);

})(appControllers); 