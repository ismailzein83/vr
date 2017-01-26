(function (appControllers) {

    'use strict';

    VisibilityAccountDefinitionController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function VisibilityAccountDefinitionController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var isEditMode;

        var visibilityAccountDefinitionEntity;

        var accountBEDefinitionSelectorAPI;
        var accountBEDefinitionSelectorDeferred = UtilsService.createPromiseDeferred();

        var visibilityGridColumnsDirectiveAPI;
        var visibilityGridColumnsDirectiveDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                visibilityAccountDefinitionEntity = parameters.visibilityAccountDefinition;
            }

            isEditMode = (visibilityAccountDefinitionEntity != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onAccountDefinitionSelectorReady = function (api) {
                accountBEDefinitionSelectorAPI = api;
                accountBEDefinitionSelectorDeferred.resolve();
            };
            $scope.scopeModel.onVisibilityGridColumnsDirectiveReady = function (api) {
                visibilityGridColumnsDirectiveAPI = api;
                visibilityGridColumnsDirectiveDeferred.resolve();
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
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadAccountDefinitionSelectorPromise, loadVisibilityGridColumnsDirectivePromise]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function setTitle() {
            $scope.title = (isEditMode) ?
                UtilsService.buildTitleForUpdateEditor((visibilityAccountDefinitionEntity != undefined) ? visibilityAccountDefinitionEntity.Title : null, 'Account Definition') :
                UtilsService.buildTitleForAddEditor('Account Definition');
        }
        function loadStaticData() {
            if (visibilityAccountDefinitionEntity == undefined)
                return;

            $scope.scopeModel.title = visibilityAccountDefinitionEntity.Title;
        }
        function loadAccountDefinitionSelectorPromise() {
            var accountBEDefinitionLoadDeferred = UtilsService.createPromiseDeferred();

            accountBEDefinitionSelectorDeferred.promise.then(function () {
                var accountActionDefinitionPayload = {
                    filter: {
                        Filters: [{
                            $type: "Retail.BusinessEntity.Business.AccountBEDefinitionFilter, Retail.BusinessEntity.Business"
                        }]
                    },
                    selectedIds: visibilityAccountDefinitionEntity != undefined ? visibilityAccountDefinitionEntity.AccountBEDefinitionId : undefined
                };
                VRUIUtilsService.callDirectiveLoad(accountBEDefinitionSelectorAPI, accountActionDefinitionPayload, accountBEDefinitionLoadDeferred);
            });

            return accountBEDefinitionLoadDeferred.promise;
        }
        function loadVisibilityGridColumnsDirectivePromise() {
            var visibilityGridColumnsLoadDeferred = UtilsService.createPromiseDeferred();

            visibilityGridColumnsDirectiveDeferred.promise.then(function () {
                var visibilityGridColumnsDirectivePayload = {
                    gridColumns: visibilityAccountDefinitionEntity != undefined ? visibilityAccountDefinitionEntity.GridColumns : undefined
                };
                VRUIUtilsService.callDirectiveLoad(visibilityGridColumnsDirectiveAPI, visibilityGridColumnsDirectivePayload, visibilityGridColumnsLoadDeferred);
            });

            return visibilityGridColumnsLoadDeferred.promise;
        }

        function insert() {
            var visibilityAccountDefinitionObject = buildVisibilityAccountDefinitionFromScope();
            if ($scope.onVisibilityAccountDefinitionAdded != undefined && typeof ($scope.onVisibilityAccountDefinitionAdded) == 'function') {
                $scope.onVisibilityAccountDefinitionAdded(visibilityAccountDefinitionObject);
            }
            $scope.modalContext.closeModal();
        }
        function update() {
            var visibilityAccountDefinitionObject = buildVisibilityAccountDefinitionFromScope();
            if ($scope.onVisibilityAccountDefinitionUpdated != undefined && typeof ($scope.onVisibilityAccountDefinitionUpdated) == 'function') {
                $scope.onVisibilityAccountDefinitionUpdated(visibilityAccountDefinitionObject);
            }
            $scope.modalContext.closeModal();
        }

        function buildVisibilityAccountDefinitionFromScope() {

            return {
                Title: $scope.scopeModel.title,
                AccountBEDefinitionId: accountBEDefinitionSelectorAPI.getSelectedIds()
            };
        }
    }

    appControllers.controller('Retail_BE_VisibilityAccountDefinitionController', VisibilityAccountDefinitionController);

})(appControllers);