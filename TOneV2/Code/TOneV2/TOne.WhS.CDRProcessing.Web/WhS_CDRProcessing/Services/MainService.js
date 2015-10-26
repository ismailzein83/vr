
app.service('WhS_CDRProcessing_MainService', ['VRModalService', 'VRNotificationService', 'UtilsService', 'WhS_CDRProcessing_SetCustomerRuleAPIService', 'WhS_CDRProcessing_SetSupplierRuleAPIService',
    function (VRModalService, VRNotificationService, UtilsService, WhS_CDRProcessing_SetCustomerRuleAPIService, WhS_CDRProcessing_SetSupplierRuleAPIService) {

    return ({
        addSetCustomerRule: addSetCustomerRule,
        editSetCustomerRule: editSetCustomerRule,
        deleteSetCustomerRule: deleteSetCustomerRule,
        addSetSupplierRule: addSetSupplierRule,
        editSetSupplierRule: editSetSupplierRule,
        deleteSetSupplierRule: deleteSetSupplierRule,
    });

    function addSetCustomerRule(onSetCustomerRuleAdded) {
        var settings = {};

        settings.onScopeReady = function (modalScope) {
            modalScope.onSetCustomerRuleAdded = onSetCustomerRuleAdded;
        };
        var  parameters={
        };     
        VRModalService.showModal('/Client/Modules/WhS_CDRProcessing/Views/CustomerRule/SetCustomerRuleEditor.html', parameters, settings);
    }
    function editSetCustomerRule(obj, onSetCustomerRuleUpdated) {
        var settings = {};

        settings.onScopeReady = function (modalScope) {
            modalScope.onSetCustomerRuleUpdated = onSetCustomerRuleUpdated;
        };
        var parameters = {
            RuleId: obj.RuleId,
        };
        VRModalService.showModal('/Client/Modules/WhS_CDRProcessing/Views/CustomerRule/SetCustomerRuleEditor.html', parameters, settings);
    }
    
    function deleteSetCustomerRule($scope, customerRuleObj, onSetCustomerRuleObjDeleted) {
        VRNotificationService.showConfirmation()
            .then(function (response) {
                if (response) {
                    return WhS_CDRProcessing_SetCustomerRuleAPIService.DeleteRule(customerRuleObj.Entity.RuleId)
                        .then(function (deletionResponse) {
                            VRNotificationService.notifyOnItemDeleted("Customer Rule", deletionResponse);
                            onSetCustomerRuleObjDeleted(customerRuleObj);
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                }
            });
    }
    function addSetSupplierRule(onSetSupplierRuleAdded) {
        var settings = {};

        settings.onScopeReady = function (modalScope) {
            modalScope.onSetSupplierRuleAdded = onSetSupplierRuleAdded;
        };
        var parameters = {
        };
        VRModalService.showModal('/Client/Modules/WhS_CDRProcessing/Views/SupplierRule/SetSupplierRuleEditor.html', parameters, settings);
    }
    function editSetSupplierRule(obj, onSetSupplierRuleUpdated) {
        var settings = {};

        settings.onScopeReady = function (modalScope) {
            modalScope.onSetSupplierRuleUpdated = onSetSupplierRuleUpdated;
        };
        var parameters = {
            RuleId: obj.RuleId,
        };

        VRModalService.showModal('/Client/Modules/WhS_CDRProcessing/Views/SupplierRule/SetSupplierRuleEditor.html', parameters, settings);
    }

    function deleteSetSupplierRule($scope,supplierRuleObj, onSetSupplierRuleDeleted) {
        VRNotificationService.showConfirmation()
            .then(function (response) {
                if (response) {
                    return WhS_CDRProcessing_SetSupplierRuleAPIService.DeleteRule(supplierRuleObj.Entity.RuleId)
                        .then(function (deletionResponse) {
                            VRNotificationService.notifyOnItemDeleted("Supplier Rule", deletionResponse);
                            onSetSupplierRuleDeleted(supplierRuleObj);
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                }
            });
    }
}]);
