(function (appControllers) {

    'use strict';

    AccountIdentificationAssignmentEditorController.$inject = ['$scope', 'Retail_BE_AccountIdentificationAPIService', 'VR_GenericData_GenericRule', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService', 'VRValidationService'];

    function AccountIdentificationAssignmentEditorController($scope, Retail_BE_AccountIdentificationAPIService, VR_GenericData_GenericRule, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, VRValidationService) {

        var accountId;

        var ruleDefinitionSelectorAPI;
        var ruleDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                accountId = parameters.accountId;
            }
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onRuleDefinitionSelectorReady = function (api) {
                ruleDefinitionSelectorAPI = api;
                ruleDefinitionSelectorReadyDeferred.resolve();
            }

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

            $scope.scopeModel.assignIdentificationRule = function () {
                $scope.modalContext.closeModal();

                VR_GenericData_GenericRule.addGenericRule(ruleDefinitionSelectorAPI.getSelectedIds(), onAccountIdentificationRuleAdded);
            }
        }

        function load() {
            $scope.scopeModel.isLoading = true;

            loadAllControls();
        }


        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadRuleDefinitionSelector]).then(function () {
               var identificationRules = ruleDefinitionSelectorAPI.getDatasource();
               if (identificationRules.length == 1) {
                   $scope.modalContext.closeModal();
                   VR_GenericData_GenericRule.addGenericRule(identificationRules[0].GenericRuleDefinitionId, onAccountIdentificationRuleAdded);
               }
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function setTitle() {

            $scope.title = 'Assign Identification Rule to Account';
        }

        function loadRuleDefinitionSelector() {
            var ruleDefinitionLoadDeferred = UtilsService.createPromiseDeferred();

            ruleDefinitionSelectorReadyDeferred.promise.then(function () {
                var ruleDefinitionPayload = {
                    filter: { Filters: [{ $type: "Retail.BusinessEntity.Business.AccountMappingRuleDefinitionFilter,Retail.BusinessEntity.Business" }] }
                };

                VRUIUtilsService.callDirectiveLoad(ruleDefinitionSelectorAPI, ruleDefinitionPayload, ruleDefinitionLoadDeferred);
            });

            return ruleDefinitionLoadDeferred.promise;
        }

        function onAccountIdentificationRuleAdded(ruleObj) {
            var accountIdentificationDetail = {
                AccountId: ruleObj.Entity.Settings != undefined ? ruleObj.Entity.Settings.Value : accountId,
                Description: ruleObj.Entity.Description
            };

            $scope.onAccountIdentificationRuleAdded(accountIdentificationDetail);
        };

    }

    appControllers.controller('Retail_BE_AccountIdentificationAssignmentEditorController', AccountIdentificationAssignmentEditorController);

})(appControllers);