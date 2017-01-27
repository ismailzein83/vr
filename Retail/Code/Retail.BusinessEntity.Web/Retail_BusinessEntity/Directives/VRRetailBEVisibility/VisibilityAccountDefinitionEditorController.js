(function (appControllers) {

    'use strict';

    VisibilityAccountDefinitionController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function VisibilityAccountDefinitionController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var isEditMode;

        var visibilityAccountDefinitionEntity;
        var retailBEVisibilityEditorRuntime;
        var accountTypeTitlesById;
        var accountBEDefinitionId;

        var accountBEDefinitionSelectorAPI;
        var accountBEDefinitionSelectorDeferred = UtilsService.createPromiseDeferred();
        var accountBEDefinitionSelectionChangedDeferred;

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
            $scope.scopeModel.onVisibilityAccountTypeDirectiveReady = function (api) {
                visibilityAccountTypeDirectiveAPI = api;
                visibilityAccountTypeDirectiveReady.resolve();
            };

            $scope.scopeModel.onAccountBEDefinitionSelectionChanged = function (selectedAccountBEDefinition) {
                if (selectedAccountBEDefinition != undefined) {

                    accountBEDefinitionId = selectedAccountBEDefinition.BusinessEntityDefinitionId;

                    if (accountBEDefinitionSelectionChangedDeferred != undefined) {
                        accountBEDefinitionSelectionChangedDeferred.resolve();
                    }
                    else {
                        loadVisibilityAccountTypeDirective();
                    }

                    function loadVisibilityAccountTypeDirective() {
                        var visibilityAccountTypeLoadDeferred = UtilsService.createPromiseDeferred();

                        var visibilityAccountTypeDirectivePayload = {
                            accountBEDefinitionId: accountBEDefinitionId
                        };
                        VRUIUtilsService.callDirectiveLoad(visibilityAccountTypeDirectiveAPI, visibilityAccountTypeDirectivePayload, visibilityAccountTypeLoadDeferred);

                        return visibilityAccountTypeLoadDeferred.promise;
                    }
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
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadAccountDefinitionSelector, loadVisibilityAccountTypeDirective]).catch(function (error) {
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
        function loadVisibilityAccountTypeDirective() {
            if (!isEditMode)
                return;

            var visibilityAccountTypeLoadDeferred = UtilsService.createPromiseDeferred();

            UtilsService.waitMultiplePromises([visibilityAccountTypeDirectiveReady.promise, accountBEDefinitionSelectionChangedDeferred.promise]).then(function () {
                var visibilityAccountTypeDirectivePayload = {
                    accountBEDefinitionId: accountBEDefinitionId,
                    accountTypes: visibilityAccountDefinitionEntity != undefined ? visibilityAccountDefinitionEntity.AccountTypes : undefined,
                    accountTypeTitlesById: retailBEVisibilityEditorRuntime != undefined ? retailBEVisibilityEditorRuntime.AccountTypeTitlesById : undefined
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
                AccountTypes: visibilityAccountTypeDirectiveAPI.getData()
            };
        }
    }

    appControllers.controller('Retail_BE_VisibilityAccountDefinitionController', VisibilityAccountDefinitionController);

})(appControllers);