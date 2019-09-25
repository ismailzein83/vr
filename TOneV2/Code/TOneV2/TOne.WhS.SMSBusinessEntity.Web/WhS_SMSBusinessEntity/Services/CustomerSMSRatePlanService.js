(function (appControllers) {

    'use strict';

    CustomerSMSRatePlanService.$inject = ['VRModalService'];
    function CustomerSMSRatePlanService(VRModalService) {

        function addSMSRates(selectedCustomer, onSaleSMSRatesApplied) {
            var parameters = {
                customerInfo: selectedCustomer
            };

            var modalSettings = {
                onScopeReady: function (modalScope) {
                    modalScope.title = "Add SMS Rates for Customer '" + selectedCustomer.Name + "'";
                    modalScope.onSaleSMSRatesApplied = onSaleSMSRatesApplied;
                }
            };
            VRModalService.showModal("/Client/Modules/WhS_SMSBusinessEntity/Views/CustomerSMSRatePlanEditor.html", parameters, modalSettings);
        }

        function uploadSMSRateChanges(selectedCustomer, currencyObj, effectiveDate, onSaleSMSRateChangesUploaded) {
            var parameters = {
                customerInfo: selectedCustomer,
                effectiveDate: effectiveDate,
                currencyObj: currencyObj
            };

            var modalSettings = {
                onScopeReady: function (modalScope) {
                    modalScope.title = "Upload SMS Rates for Customer '" + selectedCustomer.Name + "'";
                    modalScope.onSaleSMSRateChangesUploaded = onSaleSMSRateChangesUploaded;
                }
            };
            VRModalService.showModal("/Client/Modules/WhS_SMSBusinessEntity/Views/CustomerSMSUploadRatesEditor.html", parameters, modalSettings);
        }

        function viewFutureSMSRate(mobileNetworkName, futureSMSRate) {
            var parameters = {
                mobileNetworkName: mobileNetworkName,
                futureSMSRate: futureSMSRate
            };

            var settings = {};

            VRModalService.showModal("/Client/Modules/WhS_SMSBusinessEntity/Views/FutureSMSCustomerRate.html", parameters, settings);
        }

        return {
            addSMSRates: addSMSRates,
            uploadSMSRateChanges: uploadSMSRateChanges,
            viewFutureSMSRate: viewFutureSMSRate
        };
    }

    appControllers.service('WhS_SMSBusinessEntity_CustomerRatePlanService', CustomerSMSRatePlanService);

})(appControllers); 