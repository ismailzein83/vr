
app.service('WhS_CDRProcessing_MainService', ['VRModalService', 'VRNotificationService', 'UtilsService', 'WhS_CDRProcessing_CustomerIdentificationRuleAPIService', 'WhS_CDRProcessing_SupplierIdentificationRuleAPIService', 'WhS_CDRProcessing_NormalizationRuleAPIService','WhS_CDRProcessing_DefineCDRFieldsAPIService',

    function (VRModalService, VRNotificationService, UtilsService, WhS_CDRProcessing_CustomerIdentificationRuleAPIService, WhS_CDRProcessing_SupplierIdentificationRuleAPIService, WhS_CDRProcessing_NormalizationRuleAPIService, WhS_CDRProcessing_DefineCDRFieldsAPIService) {

    return ({
        addCustomerIdentificationRule: addCustomerIdentificationRule,
        editCustomerIdentificationRule: editCustomerIdentificationRule,
        deleteCustomerIdentificationRule: deleteCustomerIdentificationRule,
        addSupplierIdentificationRule: addSupplierIdentificationRule,
        editSupplierIdentificationRule: editSupplierIdentificationRule,
        deleteSupplierIdentificationRule: deleteSupplierIdentificationRule,
        editNormalizationRule: editNormalizationRule,
        addNormalizationRule: addNormalizationRule,
        deleteNormalizationRule: deleteNormalizationRule,
        addSwitchIdentificationRule: addSwitchIdentificationRule,
        editSwitchIdentificationRule: editSwitchIdentificationRule,
        deleteSwitchIdentificationRule: deleteSwitchIdentificationRule,
        addNewCDRField: addNewCDRField,
        editCDRField: editCDRField,
        deleteCDRField: deleteCDRField

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
                            VRNotificationService.notifyOnItemDeleted("Customer Identification Rule", deletionResponse);
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
                            VRNotificationService.notifyOnItemDeleted("Supplier Identification Rule", deletionResponse);
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

    function addSwitchIdentificationRule(onSwitchIdentificationRuleAdded) {
        var settings = {};

        settings.onScopeReady = function (modalScope) {
            modalScope.onSwitchIdentificationRuleAdded = onSwitchIdentificationRuleAdded;
        };
        var parameters = {
        };
        VRModalService.showModal('/Client/Modules/WhS_CDRProcessing/Views/SwitchRule/SwitchIdentificationRuleEditor.html', parameters, settings);
    }
    function editSwitchIdentificationRule(obj, onSwitchIdentificationRuleUpdated) {
        var settings = {};

        settings.onScopeReady = function (modalScope) {
            modalScope.onSwitchIdentificationRuleUpdated = onSwitchIdentificationRuleUpdated;
        };
        var parameters = {
            RuleId: obj.RuleId,
        };
        VRModalService.showModal('/Client/Modules/WhS_CDRProcessing/Views/SwitchRule/SwitchIdentificationRuleEditor.html', parameters, settings);
    }
    function deleteSwitchIdentificationRule($scope, switchRuleObj, onSwitchIdentificationRuleObjDeleted) {
        VRNotificationService.showConfirmation()
            .then(function (response) {
                if (response) {
                    return WhS_CDRProcessing_CustomerIdentificationRuleAPIService.DeleteRule(switchRuleObj.Entity.RuleId)
                        .then(function (deletionResponse) {
                            VRNotificationService.notifyOnItemDeleted("Switch Identification Rule", deletionResponse);
                            onSwitchIdentificationRuleObjDeleted(switchRuleObj);
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                }
            });
    }

    function addNewCDRField(onCDRFieldAdded) {
        var settings = {};

        settings.onScopeReady = function (modalScope) {
            modalScope.onCDRFieldAdded = onCDRFieldAdded;
        };
        var parameters = {
        };
        VRModalService.showModal('/Client/Modules/WhS_CDRProcessing/Views/CDRFields/DefineCDRFieldsEditor.html', parameters, settings);
    }

    function editCDRField(obj, onCDRFieldUpdated) {
        var settings = {};

        settings.onScopeReady = function (modalScope) {
            modalScope.onCDRFieldUpdated = onCDRFieldUpdated;
        };
        var parameters = {
            ID: obj.ID,
        };
        VRModalService.showModal('/Client/Modules/WhS_CDRProcessing/Views/CDRFields/DefineCDRFieldsEditor.html', parameters, settings);
    }

    function deleteCDRField($scope, obj, onCDRFieldObjDeleted) {
        VRNotificationService.showConfirmation()
            .then(function (response) {
                if (response) {
                    return WhS_CDRProcessing_DefineCDRFieldsAPIService.DeleteCDRField(obj.Entity.ID)
                        .then(function (deletionResponse) {
                            VRNotificationService.notifyOnItemDeleted("CDR Field", deletionResponse);
                            onCDRFieldObjDeleted(obj);
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                }
            });
    }

}]);
