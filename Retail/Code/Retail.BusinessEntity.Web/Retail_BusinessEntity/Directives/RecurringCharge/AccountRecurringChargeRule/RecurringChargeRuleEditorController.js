(function (appControllers) {

    'use strict';

    RecurringChargeRuleController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function RecurringChargeRuleController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var isEditMode;

        var recurringChargeRuleEntity;
        var recurringChargeRuleNames;

        var recurringChargeDefinitionSelectorAPI;
        var recurringChargeDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var accountChargeEvaluatorSelectiveAPI;
        var accountChargeEvaluatorSelectiveReadyDeferred = UtilsService.createPromiseDeferred();

        var accountConditionSelectiveAPI;
        var accountConditionSelectiveReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                recurringChargeRuleEntity = parameters.recurringChargeRule;
                recurringChargeRuleNames = parameters.recurringChargeRuleNames;
            }

            isEditMode = (recurringChargeRuleEntity != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onRecurringChargeDefinitionSelectorReady = function (api) {
                recurringChargeDefinitionSelectorAPI = api;
                recurringChargeDefinitionSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onAccountChargeEvaluatorSelectiveReady = function (api) {
                accountChargeEvaluatorSelectiveAPI = api;
                accountChargeEvaluatorSelectiveReadyDeferred.resolve();
            };
            $scope.scopeModel.onAccountConditionSelectiveReady = function (api) {
                accountConditionSelectiveAPI = api;
                accountConditionSelectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.onValidateNames = function () {
                if (recurringChargeRuleNames == undefined)
                    return null;

                if (recurringChargeRuleEntity != undefined && recurringChargeRuleEntity.Name == $scope.scopeModel.name)
                    return null;

                for (var i = 0; i < recurringChargeRuleNames.length; i++) {
                    var currentName = recurringChargeRuleNames[i];
                    if ($scope.scopeModel.name.toLowerCase() == currentName.toLowerCase()) {
                        return 'Same Rule Name Exists';
                    }
                }
                return null;
            };

            $scope.scopeModel.save = function () {
                if (isEditMode)
                    return update();
                else
                    return insert();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }
        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadRecurringChargeDefinitionSelector, loadAccountChargeEvaluatorSelective, loadAccountConditionSelective])
                .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function setTitle() {
            $scope.title = (isEditMode) ?
                UtilsService.buildTitleForUpdateEditor((recurringChargeRuleEntity != undefined) ? recurringChargeRuleEntity.Name : null, 'Recurring Charge Rule Set') :
                UtilsService.buildTitleForAddEditor('Recurring Charge Rule Set');
        }
        function loadStaticData() {
            if (recurringChargeRuleEntity == undefined)
                return;

            $scope.scopeModel.name = recurringChargeRuleEntity.Name;
        }
        function loadRecurringChargeDefinitionSelector() {
            var recurringChargeDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            recurringChargeDefinitionSelectorReadyDeferred.promise.then(function () {

                var recurringChargeDefinitionSelectorPayload;
                if (recurringChargeRuleEntity != undefined) {
                    recurringChargeDefinitionSelectorPayload = {
                        selectedIds: recurringChargeRuleEntity.RecurringChargeDefinitionId
                    };
                }
                VRUIUtilsService.callDirectiveLoad(recurringChargeDefinitionSelectorAPI, recurringChargeDefinitionSelectorPayload, recurringChargeDefinitionSelectorLoadDeferred);
            });

            return recurringChargeDefinitionSelectorLoadDeferred.promise;
        }
        function loadAccountChargeEvaluatorSelective() {
            var accountChargeEvaluatorSelectiveLoadDeferred = UtilsService.createPromiseDeferred();

            accountChargeEvaluatorSelectiveReadyDeferred.promise.then(function () {

                var accountChargeEvaluatorSelectivePayload;
                if (recurringChargeRuleEntity != undefined) {
                    accountChargeEvaluatorSelectivePayload = {
                        chargeEvaluator: recurringChargeRuleEntity.ChargeEvaluator
                    };
                }
                VRUIUtilsService.callDirectiveLoad(accountChargeEvaluatorSelectiveAPI, accountChargeEvaluatorSelectivePayload, accountChargeEvaluatorSelectiveLoadDeferred);
            });

            return accountChargeEvaluatorSelectiveLoadDeferred.promise;
        }
        function loadAccountConditionSelective() {
            var accountConditionSelectiveLoadDeferred = UtilsService.createPromiseDeferred();

            accountConditionSelectiveReadyDeferred.promise.then(function () {

                var accountConditionSelectivePayload;
                if (recurringChargeRuleEntity != undefined) {
                    accountConditionSelectivePayload = {
                        beFilter: recurringChargeRuleEntity.Condition
                    };
                }
                VRUIUtilsService.callDirectiveLoad(accountConditionSelectiveAPI, accountConditionSelectivePayload, accountConditionSelectiveLoadDeferred);
            });

            return accountConditionSelectiveLoadDeferred.promise;
        }

        function insert() {
            var recurringChargeRuleObject = buildRecurringChargeRuleObjectFromScope();

            if ($scope.onRecurringChargeRuleAdded != undefined && typeof ($scope.onRecurringChargeRuleAdded) == 'function') {
                $scope.onRecurringChargeRuleAdded(recurringChargeRuleObject);
            }
            $scope.modalContext.closeModal();
        }
        function update() {
            var recurringChargeRuleObject = buildRecurringChargeRuleObjectFromScope();

            if ($scope.onRecurringChargeRuleUpdated != undefined && typeof ($scope.onRecurringChargeRuleUpdated) == 'function') {
                $scope.onRecurringChargeRuleUpdated(recurringChargeRuleObject);
            }
            $scope.modalContext.closeModal();
        }

        function buildRecurringChargeRuleObjectFromScope() {

            return {
                Name: $scope.scopeModel.name,
                RecurringChargeRuleId: recurringChargeRuleEntity != undefined ? recurringChargeRuleEntity.RecurringChargeRuleId : UtilsService.guid(),
                RecurringChargeDefinitionId: recurringChargeDefinitionSelectorAPI.getSelectedIds(),
                ChargeEvaluator: accountChargeEvaluatorSelectiveAPI.getData(),
                Condition: accountConditionSelectiveAPI.getData()
            };
        }
    }

    appControllers.controller('Retail_BE_RecurringChargeRuleEditorController', RecurringChargeRuleController);

})(appControllers);