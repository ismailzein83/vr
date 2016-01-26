(function (appControllers) {

    'use strict';

    SupplierIdentificationRuleService.$inject = ['WhS_CDRProcessing_SupplierIdentificationRuleAPIService', 'VRModalService', 'VRNotificationService'];

    function SupplierIdentificationRuleService(WhS_CDRProcessing_SupplierIdentificationRuleAPIService, VRModalService, VRNotificationService) {
        return {
            addSupplierIdentificationRule: addSupplierIdentificationRule,
            editSupplierIdentificationRule: editSupplierIdentificationRule,
            deleteSupplierIdentificationRule: deleteSupplierIdentificationRule
        };

        function addSupplierIdentificationRule(onSupplierIdentificationRuleAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onSupplierIdentificationRuleAdded = onSupplierIdentificationRuleAdded;
            };
            var parameters = {
            };
            VRModalService.showModal('/Client/Modules/WhS_CDRProcessing/Views/SupplierRule/SupplierIdentificationRuleEditor.html', parameters, settings);
        }

        function editSupplierIdentificationRule(obj, onSupplierIdentificationRuleUpdated) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onSupplierIdentificationRuleUpdated = onSupplierIdentificationRuleUpdated;
            };
            var parameters = {
                RuleId: obj.RuleId,
            };

            VRModalService.showModal('/Client/Modules/WhS_CDRProcessing/Views/SupplierRule/SupplierIdentificationRuleEditor.html', parameters, settings);
        }

        function deleteSupplierIdentificationRule($scope, supplierRuleObj, onSupplierIdentificationRuleDeleted) {
            VRNotificationService.showConfirmation()
                .then(function (response) {
                    if (response) {
                        return WhS_CDRProcessing_SupplierIdentificationRuleAPIService.DeleteRule(supplierRuleObj.Entity.RuleId)
                            .then(function (deletionResponse) {
                                VRNotificationService.notifyOnItemDeleted("Supplier Identification Rule", deletionResponse);
                                onSupplierIdentificationRuleDeleted(supplierRuleObj);
                            })
                            .catch(function (error) {
                                VRNotificationService.notifyException(error, $scope);
                            });
                    }
                });
        }
    }

    appControllers.service('WhS_CDRProcessing_SupplierIdentificationRuleService', SupplierIdentificationRuleService);

})(appControllers);
