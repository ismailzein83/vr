(function (appControllers) {

    'use strict';

    RecurringChargeRuleSetController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function RecurringChargeRuleSetController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var isEditMode;

        var recurringChargeRuleSetEntity;
        var productDefinitionId;
        var excludedPackageIds;

        //var packageSelectorAPI;
        //var packageSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        //var accountChargeEvaluatorSelectiveAPI;
        //var accountChargeEvaluatorSelectiveReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                recurringChargeRuleSetEntity = parameters.recurringChargeRuleSet;
            }

            isEditMode = (recurringChargeRuleSetEntity != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};
            //$scope.scopeModel.isPackageSelectorDisabled = isEditMode ? true : false;

            //$scope.scopeModel.onPackageSelectorReady = function (api) {
            //    packageSelectorAPI = api;
            //    packageSelectorReadyDeferred.resolve();
            //};

            //$scope.scopeModel.onAccountChargeEvaluatorSelectiveReady = function (api) {
            //    accountChargeEvaluatorSelectiveAPI = api;
            //    accountChargeEvaluatorSelectiveReadyDeferred.resolve();
            //};

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
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function setTitle() {
            $scope.title = (isEditMode) ?
                UtilsService.buildTitleForUpdateEditor((recurringChargeRuleSetEntity != undefined) ? recurringChargeRuleSetEntity.Name : null, 'Recurring Charge Rule Set') :
                UtilsService.buildTitleForAddEditor('Recurring Charge Rule Set');
        }
        function loadStaticData() {
            if (recurringChargeRuleSetEntity == undefined)
                return;

            $scope.scopeModel.name = recurringChargeRuleSetEntity.Name;
        }
        function loadPackageSelector() {
            var packageSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            packageSelectorReadyDeferred.promise.then(function () {

                var packageSelectorPayload = {
                    filter: {
                        ExcludedPackageIds: !isEditMode ? excludedPackageIds : undefined,
                        Filters: [{
                            $type: "Retail.BusinessEntity.Business.ProductDefinitionPackageFilter, Retail.BusinessEntity.Business",
                            ProductDefinitionId: productDefinitionId
                        }]
                    }
                };
                if (recurringChargeRuleSetEntity != undefined) {
                    packageSelectorPayload.selectedIds = recurringChargeRuleSetEntity.PackageId;
                }
                VRUIUtilsService.callDirectiveLoad(packageSelectorAPI, packageSelectorPayload, packageSelectorLoadDeferred);
            });

            return packageSelectorLoadDeferred.promise;
        }
        function loadAccountChargeEvaluatorSelective() {
            var accountChargeEvaluatorSelectiveLoadDeferred = UtilsService.createPromiseDeferred();

            accountChargeEvaluatorSelectiveReadyDeferred.promise.then(function () {

                var accountChargeEvaluatorSelectivePayload;
                if (recurringChargeRuleSetEntity != undefined) {
                    accountChargeEvaluatorSelectivePayload = {
                        chargeEvaluator: recurringChargeRuleSetEntity.ChargeEvaluator
                    };
                }
                VRUIUtilsService.callDirectiveLoad(accountChargeEvaluatorSelectiveAPI, accountChargeEvaluatorSelectivePayload, accountChargeEvaluatorSelectiveLoadDeferred);
            });

            return accountChargeEvaluatorSelectiveLoadDeferred.promise;
        }

        function insert() {
            var recurringChargeRuleSetObject = buildRecurringChargeRuleSetObjectFromScope();

            if ($scope.onRecurringChargeRuleSetAdded != undefined && typeof ($scope.onRecurringChargeRuleSetAdded) == 'function') {
                $scope.onRecurringChargeRuleSetAdded(recurringChargeRuleSetObject);
            }
            $scope.modalContext.closeModal();
        }
        function update() {
            var recurringChargeRuleSetObject = buildRecurringChargeRuleSetObjectFromScope();

            if ($scope.onRecurringChargeRuleSetUpdated != undefined && typeof ($scope.onRecurringChargeRuleSetUpdated) == 'function') {
                $scope.onRecurringChargeRuleSetUpdated(recurringChargeRuleSetObject);
            }
            $scope.modalContext.closeModal();
        }

        function buildRecurringChargeRuleSetObjectFromScope() {

            return {
                Name: $scope.scopeModel.name
            };
        }
    }

    appControllers.controller('Retail_BE_RecurringChargeRuleSetEditorController', RecurringChargeRuleSetController);

})(appControllers);