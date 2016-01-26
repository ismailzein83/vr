(function (appControllers) {

    'use strict';

    CustomerIdentificationRuleService.$inject = ['WhS_CDRProcessing_CustomerIdentificationRuleAPIService', 'VRModalService', 'VRNotificationService'];

    function CustomerIdentificationRuleService(WhS_CDRProcessing_CustomerIdentificationRuleAPIService, VRModalService, VRNotificationService) {
        return {
            addCustomerIdentificationRule: addCustomerIdentificationRule,
            editCustomerIdentificationRule: editCustomerIdentificationRule,
            deleteCustomerIdentificationRule: deleteCustomerIdentificationRule
        };

        function addCustomerIdentificationRule(onCustomerIdentificationRuleAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onCustomerIdentificationRuleAdded = onCustomerIdentificationRuleAdded;
            };
            var parameters = {
            };
            VRModalService.showModal('/Client/Modules/WhS_CDRProcessing/Views/CustomerRule/CustomerIdentificationRuleEditor.html', parameters, settings);
        }

        function editCustomerIdentificationRule(obj, onCustomerIdentificationRuleUpdated) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onCustomerIdentificationRuleUpdated = onCustomerIdentificationRuleUpdated;
            };
            var parameters = {
                RuleId: obj.RuleId,
            };
            VRModalService.showModal('/Client/Modules/WhS_CDRProcessing/Views/CustomerRule/CustomerIdentificationRuleEditor.html', parameters, settings);
        }

        function deleteCustomerIdentificationRule($scope, customerRuleObj, onCustomerIdentificationRuleObjDeleted) {
            VRNotificationService.showConfirmation()
                .then(function (response) {
                    if (response) {
                        return WhS_CDRProcessing_CustomerIdentificationRuleAPIService.DeleteRule(customerRuleObj.Entity.RuleId)
                            .then(function (deletionResponse) {
                                VRNotificationService.notifyOnItemDeleted("Customer Identification Rule", deletionResponse);
                                onCustomerIdentificationRuleObjDeleted(customerRuleObj);
                            })
                            .catch(function (error) {
                                VRNotificationService.notifyException(error, $scope);
                            });
                    }
                });
        }
    }

    appControllers.service('WhS_CDRProcessing_CustomerIdentificationRuleService', CustomerIdentificationRuleService);

})(appControllers);
