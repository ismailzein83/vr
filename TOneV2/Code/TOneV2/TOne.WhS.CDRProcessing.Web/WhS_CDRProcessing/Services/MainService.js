
app.service('WhS_CDRProcessing_MainService', ['VRModalService', 'VRNotificationService', 'UtilsService', 'WhS_CDRProcessing_CustomerIdentificationRuleAPIService', 'WhS_CDRProcessing_SupplierIdentificationRuleAPIService', 'WhS_CDRProcessing_NormalizationRuleAPIService',

    function (VRModalService, VRNotificationService, UtilsService, WhS_CDRProcessing_CustomerIdentificationRuleAPIService, WhS_CDRProcessing_SupplierIdentificationRuleAPIService, WhS_CDRProcessing_NormalizationRuleAPIService) {

    return ({
        addCustomerIdentificationRule: addCustomerIdentificationRule,
        editCustomerIdentificationRule: editCustomerIdentificationRule,
        deleteCustomerIdentificationRule: deleteCustomerIdentificationRule,
        addSupplierIdentificationRule: addSupplierIdentificationRule,
        editSupplierIdentificationRule: editSupplierIdentificationRule,
        deleteSupplierIdentificationRule: deleteSupplierIdentificationRule,
        editNormalizationRule: editNormalizationRule,
        addNormalizationRule: addNormalizationRule,
        deleteNormalizationRule: deleteNormalizationRule
    });

    function addCustomerIdentificationRule(onCustomerIdentificationRuleAdded) {
        var settings = {};

        settings.onScopeReady = function (modalScope) {
            modalScope.onCustomerIdentificationRuleAdded = onCustomerIdentificationRuleAdded;
        };
        var  parameters={
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
                            VRNotificationService.notifyOnItemDeleted("Customer Rule", deletionResponse);
                            onCustomerIdentificationRuleObjDeleted(customerRuleObj);
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                }
            });
    }
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

    function deleteSupplierIdentificationRule($scope,supplierRuleObj, onSupplierIdentificationRuleDeleted) {
        VRNotificationService.showConfirmation()
            .then(function (response) {
                if (response) {
                    return WhS_CDRProcessing_SupplierIdentificationRuleAPIService.DeleteRule(supplierRuleObj.Entity.RuleId)
                        .then(function (deletionResponse) {
                            VRNotificationService.notifyOnItemDeleted("Supplier Rule", deletionResponse);
                            onSupplierIdentificationRuleDeleted(supplierRuleObj);
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                }
            });
    }
    function editNormalizationRule(normalizationRuleDetail, onNormalizationRuleUpdated) {
        var modalSettings = {};

        var parameters = {
            NormalizationRuleId: normalizationRuleDetail.Entity.RuleId,
        };

        modalSettings.onScopeReady = function (modalScope) {
            modalScope.title = UtilsService.buildTitleForUpdateEditor("Normalization Rule", normalizationRuleDetail.Entity.Description);

            modalScope.onNormalizationRuleUpdated = onNormalizationRuleUpdated;
        };

        VRModalService.showModal("/Client/Modules/WhS_CDRProcessing/Views/NormalizationRule/NormalizationRuleEditor.html", parameters, modalSettings);
    }

    function addNormalizationRule(onNormalizationRuleAdded) {
        var modalSettings = {};

        modalSettings.onScopeReady = function (modalScope) {
            modalScope.onNormalizationRuleAdded = onNormalizationRuleAdded;
        };

        var parameters = {
        };

        VRModalService.showModal("/Client/Modules/WhS_CDRProcessing/Views/NormalizationRule/NormalizationRuleEditor.html", parameters, modalSettings);
    }

    function deleteNormalizationRule(ruleDetail, onNormalizationRuleDeleted) {

        VRNotificationService.showConfirmation()
            .then(function (response) {
                if (response == true) {

                    return WhS_CDRProcessing_NormalizationRuleAPIService.DeleteRule(ruleDetail.Entity.RuleId)
                        .then(function (deletionResponse) {
                            if (VRNotificationService.notifyOnItemDeleted("Normalization Rule", deletionResponse))
                                onNormalizationRuleDeleted(ruleDetail);
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                }
            });
    }
}]);
