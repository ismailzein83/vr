(function (appControllers) {

    'use strict';

    VisibilityAccountDefinitionController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService', 'Retail_BE_AccountBEDefinitionAPIService'];

    function VisibilityAccountDefinitionController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, Retail_BE_AccountBEDefinitionAPIService) {

        var isEditMode;

        var visibilityAccountDefinitionEntity;
        var retailBEVisibilityEditorRuntime;
        var accountTypeTitlesById;

        var accountBEDefinitionId;
        var accountBEDefinitionSettings;

        var accountBEDefinitionSelectorAPI;
        var accountBEDefinitionSelectorDeferred = UtilsService.createPromiseDeferred();
        var accountBEDefinitionSelectionChangedDeferred;

        var visibilityGridColumnDirectiveAPI;
        var visibilityGridColumnDirectiveReady = UtilsService.createPromiseDeferred();

        var visibilityViewDirectiveAPI;
        var visibilityViewDirectiveReady = UtilsService.createPromiseDeferred();

        var visibilityActionDirectiveAPI;
        var visibilityActionDirectiveReady = UtilsService.createPromiseDeferred();

        var visibilityAccountTypeDirectiveAPI;
        var visibilityAccountTypeDirectiveReady = UtilsService.createPromiseDeferred();


        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                visibilityAccountDefinitionEntity = parameters.visibilityAccountDefinition;
                retailBEVisibilityEditorRuntime = parameters.retailBEVisibilityEditorRuntime;
            }

            isEditMode = (visibilityAccountDefinitionEntity != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onAccountBEDefinitionSelectorReady = function (api) {
                accountBEDefinitionSelectorAPI = api;
                accountBEDefinitionSelectorDeferred.resolve();
            };
            $scope.scopeModel.onVisibilityGridColumnDirectiveReady = function (api) {
                visibilityGridColumnDirectiveAPI = api;
                visibilityGridColumnDirectiveReady.resolve();
            };
            $scope.scopeModel.onVisibilityViewDirectiveReady = function (api) {
                visibilityViewDirectiveAPI = api;
                visibilityViewDirectiveReady.resolve();
            };
            $scope.scopeModel.onVisibilityActionDirectiveReady = function (api) {
                visibilityActionDirectiveAPI = api;
                visibilityActionDirectiveReady.resolve();
            };
            $scope.scopeModel.onVisibilityAccountTypeDirectiveReady = function (api) {
                visibilityAccountTypeDirectiveAPI = api;
                visibilityAccountTypeDirectiveReady.resolve();
            };

            $scope.scopeModel.onAccountBEDefinitionSelectionChanged = function (selectedAccountBEDefinition) {
                if (selectedAccountBEDefinition != undefined) {

                    accountBEDefinitionId = selectedAccountBEDefinition.BusinessEntityDefinitionId;

                    Retail_BE_AccountBEDefinitionAPIService.GetAccountBEDefinitionSettings(accountBEDefinitionId).then(function (response) {
                        accountBEDefinitionSettings = response;

                        if (accountBEDefinitionSelectionChangedDeferred != undefined) {
                            accountBEDefinitionSelectionChangedDeferred.resolve();
                        }
                        else {
                            loadVisibilityAccountTypeDirective();
                            loadVisibilityGridColumnDirective();

                            function loadVisibilityAccountTypeDirective() {
                                var visibilityAccountTypeLoadDeferred = UtilsService.createPromiseDeferred();

                                var visibilityAccountTypeDirectivePayload = {
                                    accountBEDefinitionId: accountBEDefinitionId
                                };
                                VRUIUtilsService.callDirectiveLoad(visibilityAccountTypeDirectiveAPI, visibilityAccountTypeDirectivePayload, visibilityAccountTypeLoadDeferred);

                                return visibilityAccountTypeLoadDeferred.promise;
                            }
                            function loadVisibilityGridColumnDirective() {
                                var visibilityGridColumnLoadDeferred = UtilsService.createPromiseDeferred();

                                var visibilityGridColumnDirectivePayload = {
                                    columnDefinitions: (accountBEDefinitionSettings && accountBEDefinitionSettings.GridDefinition) ? accountBEDefinitionSettings.GridDefinition.ColumnDefinitions : undefined,
                                };
                                VRUIUtilsService.callDirectiveLoad(visibilityGridColumnDirectiveAPI, visibilityGridColumnDirectivePayload, visibilityGridColumnLoadDeferred);

                                return visibilityGridColumnLoadDeferred.promise;
                            }
                        }

                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    }).finally(function () {
                        $scope.scopeModel.isLoading = false;
                    });
                }
            }

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
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadAccountDefinitionSelector, loadVisibilityGridColumnDirective, loadVisibilityViewDirective,
                        loadVisibilityActionDirective, loadVisibilityAccountTypeDirective])
                .catch(function (error) {
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
        function loadAccountDefinitionSelector() {
            if (isEditMode) {
                accountBEDefinitionSelectionChangedDeferred = UtilsService.createPromiseDeferred();
            }

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
        function loadVisibilityGridColumnDirective() {
            if (!isEditMode)
                return;

            var visibilityGridColumnLoadDeferred = UtilsService.createPromiseDeferred();

            UtilsService.waitMultiplePromises([visibilityGridColumnDirectiveReady.promise, accountBEDefinitionSelectionChangedDeferred.promise]).then(function () {
                var visibilityGridColumnDirectivePayload = {
                    gridColumnDefinitions: (accountBEDefinitionSettings && accountBEDefinitionSettings.GridDefinition) ? accountBEDefinitionSettings.GridDefinition.ColumnDefinitions: undefined,
                    gridColumns: visibilityAccountDefinitionEntity != undefined ? visibilityAccountDefinitionEntity.GridColumns : undefined
                };
                VRUIUtilsService.callDirectiveLoad(visibilityGridColumnDirectiveAPI, visibilityGridColumnDirectivePayload, visibilityGridColumnLoadDeferred);
            });

            return visibilityGridColumnLoadDeferred.promise;
        }
        function loadVisibilityViewDirective() {
            if (!isEditMode)
                return;

            var visibilityViewLoadDeferred = UtilsService.createPromiseDeferred();

            UtilsService.waitMultiplePromises([visibilityViewDirectiveReady.promise, accountBEDefinitionSelectionChangedDeferred.promise]).then(function () {
                var visibilityViewDirectivePayload = {
                    viewDefinitions: accountBEDefinitionSettings != undefined ? accountBEDefinitionSettings.AccountViewDefinitions : undefined,
                    views: visibilityAccountDefinitionEntity != undefined ? visibilityAccountDefinitionEntity.Views : undefined
                };
                VRUIUtilsService.callDirectiveLoad(visibilityViewDirectiveAPI, visibilityViewDirectivePayload, visibilityViewLoadDeferred);
            });

            return visibilityViewLoadDeferred.promise;
        }
        function loadVisibilityActionDirective() {
            if (!isEditMode)
                return;

            var visibilityActionLoadDeferred = UtilsService.createPromiseDeferred();

            UtilsService.waitMultiplePromises([visibilityViewDirectiveReady.promise, accountBEDefinitionSelectionChangedDeferred.promise]).then(function () {
                var visibilityActionDirectivePayload = {
                    actionDefinitions: accountBEDefinitionSettings != undefined ? accountBEDefinitionSettings.ActionDefinitions : undefined,
                    actions: visibilityAccountDefinitionEntity != undefined ? visibilityAccountDefinitionEntity.Actions : undefined
                };
                VRUIUtilsService.callDirectiveLoad(visibilityActionDirectiveAPI, visibilityActionDirectivePayload, visibilityActionLoadDeferred);
            });

            return visibilityActionLoadDeferred.promise;
        }
        function loadVisibilityAccountTypeDirective() {
            if (!isEditMode)
                return;

            var visibilityAccountTypeLoadDeferred = UtilsService.createPromiseDeferred();

            UtilsService.waitMultiplePromises([visibilityAccountTypeDirectiveReady.promise, accountBEDefinitionSelectionChangedDeferred.promise]).then(function () {
                var visibilityAccountTypeDirectivePayload = {
                    accountBEDefinitionId: accountBEDefinitionId,
                    accountTypes: visibilityAccountDefinitionEntity != undefined ? visibilityAccountDefinitionEntity.AccountTypes : undefined
                };
                VRUIUtilsService.callDirectiveLoad(visibilityAccountTypeDirectiveAPI, visibilityAccountTypeDirectivePayload, visibilityAccountTypeLoadDeferred);
            });

            return visibilityAccountTypeLoadDeferred.promise;
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

            var selectedAccountBEDefinition = $scope.scopeModel.selectedAccountBEDefinition;

            return {
                Title: $scope.scopeModel.title,
                AccountBEDefinitionId: selectedAccountBEDefinition != undefined ? selectedAccountBEDefinition.BusinessEntityDefinitionId : undefined,
                AccountBEDefinitionName: selectedAccountBEDefinition != undefined ? selectedAccountBEDefinition.Name : undefined,
                GridColumns: visibilityGridColumnDirectiveAPI.getData(),
                Views: visibilityViewDirectiveAPI.getData(),
                Actions: visibilityActionDirectiveAPI.getData(),
                AccountTypes: visibilityAccountTypeDirectiveAPI.getData()
            };
        }
    }

    appControllers.controller('Retail_BE_VisibilityAccountDefinitionController', VisibilityAccountDefinitionController);

})(appControllers);