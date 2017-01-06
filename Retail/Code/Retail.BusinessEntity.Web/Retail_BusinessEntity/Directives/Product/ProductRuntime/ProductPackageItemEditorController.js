(function (appControllers) {

    'use strict';

    ProductPackageItemController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function ProductPackageItemController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var isEditMode;

        var context;
        var packageItemEntity;

        var packageSelectorAPI;
        var packageSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var accountConditionSelectiveAPI;
        var accountConditionSelectiveReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            console.log(context);

            if (parameters != undefined) {
                packageItemEntity = parameters.packageItem;
                context = parameters.context;
            }
            isEditMode = (packageItemEntity != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onPackageSelectorReady = function (api) {
                packageSelectorAPI = api;
                packageSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onAccountConditionSelectiveReady = function (api) {
                accountConditionSelectiveAPI = api;
                accountConditionSelectiveReadyDeferred.resolve();
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
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadPackageSelector]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function setTitle() {
            $scope.title = (isEditMode) ?
                UtilsService.buildTitleForUpdateEditor((packageItemEntity != undefined) ? packageItemEntity.FieldName : null, 'Column') :
                UtilsService.buildTitleForAddEditor('Column');
        }
        function loadStaticData() {
            if (packageItemEntity == undefined)
                return;
        }
        function loadPackageSelector() {
            var packageSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            packageSelectorReadyDeferred.promise.then(function () {

                var packageSelectorPayload;
                if (packageItemEntity != undefined) {
                    packageSelectorPayload = {
                        selectedIds: packageItemEntity.PackageId
                    };
                }
                VRUIUtilsService.callDirectiveLoad(packageSelectorAPI, packageSelectorPayload, packageSelectorLoadDeferred);
            });

            return packageSelectorLoadDeferred.promise;
        }
        function loadAccountConditionSelective() {
            var accountConditionSelectiveLoadDeferred = UtilsService.createPromiseDeferred();

            accountConditionSelectiveReadyDeferred.promise.then(function () {

                var accountConditionSelectivePayload;
                if (packageItemEntity != undefined) {
                    accountConditionSelectivePayload = {
                        beFilter: packageItemEntity.SubAccountsAvailabilityCondition
                    };
                }
                VRUIUtilsService.callDirectiveLoad(accountConditionSelectiveAPI, accountConditionSelectivePayload, accountConditionSelectiveLoadDeferred);
            });

            return accountConditionSelectiveLoadDeferred.promise;
        }

        function insert() {
            var columnDefinitionObject = buildColumnDefinitionObjectFromScope();

            if ($scope.onColumnDefinitionAdded != undefined && typeof ($scope.onColumnDefinitionAdded) == 'function') {
                $scope.onColumnDefinitionAdded(columnDefinitionObject);
            }
            $scope.modalContext.closeModal();
        }
        function update() {
            var columnDefinitionObject = buildColumnDefinitionObjectFromScope();

            if ($scope.onColumnDefinitionUpdated != undefined && typeof ($scope.onColumnDefinitionUpdated) == 'function') {
                $scope.onColumnDefinitionUpdated(columnDefinitionObject);
            }
            $scope.modalContext.closeModal();
        }

        function buildColumnDefinitionObjectFromScope() {

            var accountGenericFieldDefinitionSelectorObj = packageSelectorAPI.getData();

            return {
                FieldName: accountGenericFieldDefinitionSelectorObj != undefined ? accountGenericFieldDefinitionSelectorObj.Name : undefined,
                Header: $scope.scopeModel.header,
                IsAvailableInRoot: $scope.scopeModel.IsAvailableInRoot,
                IsAvailableInSubAccounts: $scope.scopeModel.IsAvailableInSubAccounts,
                SubAccountsAvailabilityCondition: $scope.scopeModel.IsAvailableInSubAccounts == true ? accountConditionSelectiveAPI.getData() : null
            };
        }
    }

    appControllers.controller('Retail_BE_ProductPackageItemEditorController', ProductPackageItemController);

})(appControllers);