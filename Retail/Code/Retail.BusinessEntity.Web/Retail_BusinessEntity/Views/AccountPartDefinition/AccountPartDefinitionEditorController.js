(function (appControllers) {

    'use strict';

    AccountPartDefinitionEditorController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService','Retail_BE_AccountPartDefinitionAPIService'];

    function AccountPartDefinitionEditorController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, Retail_BE_AccountPartDefinitionAPIService) {
        var isEditMode;
        var accountPartDefinitionId;
        var accountPartDefinitionEntity;
        var accountPartDefinitionAPI;
        var accountPartDefinitionReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                accountPartDefinitionId = parameters.accountPartDefinitionId;
            }
            isEditMode = (accountPartDefinitionId != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.accountPartDefinitionDirectiveReady = function (api) {
                accountPartDefinitionAPI = api;
                accountPartDefinitionReadyDeferred.resolve();
            };

            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateAccountPartDefinition() : insertAccountPartDefinition();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };

        }

        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                getAccountPartDefinition().then(function () {
                    loadAllControls().finally(function () {
                        accountPartDefinitionEntity = undefined;
                    });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }
        }

        function getAccountPartDefinition() {
            return Retail_BE_AccountPartDefinitionAPIService.GetAccountPartDefinition(accountPartDefinitionId).then(function (response) {
                accountPartDefinitionEntity = response;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadAccountPartDefinitionTypes]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function loadAccountPartDefinitionTypes() {
            var loadAccountPartDefinitionPromiseDeferred = UtilsService.createPromiseDeferred();
            accountPartDefinitionReadyDeferred.promise.then(function () {
                var payloadDirective;
                if (accountPartDefinitionEntity != undefined)
                    payloadDirective = { partDefinitionSettings: accountPartDefinitionEntity.Settings };
                VRUIUtilsService.callDirectiveLoad(accountPartDefinitionAPI, payloadDirective, loadAccountPartDefinitionPromiseDeferred);
            });
            return loadAccountPartDefinitionPromiseDeferred.promise;
        }

        function loadStaticData() {
            if (accountPartDefinitionEntity != undefined) {
                $scope.scopeModel.name = accountPartDefinitionEntity.Name;
                $scope.scopeModel.title = accountPartDefinitionEntity.Title;
            }
        }

        function setTitle() {
            if (isEditMode) {
                var partUniqueName = (accountPartDefinitionEntity != undefined) ? accountPartDefinitionEntity.Name : undefined;
                $scope.title = UtilsService.buildTitleForUpdateEditor(partUniqueName, 'Account Section Definition');
            }
            else {
                $scope.title = UtilsService.buildTitleForAddEditor('Account Section Definition');
            }
        }

        function updateAccountPartDefinition() {
            $scope.scopeModel.isLoading = true;

            var accountPartDefinitionObj = buildAccountPartDefinitionObjFromScope();

            return Retail_BE_AccountPartDefinitionAPIService.UpdateAccountPartDefinition(accountPartDefinitionObj).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('Account Section Definition', response, 'Name')) {
                    if ($scope.onAccountPartDefinitionUpdated != undefined) {
                        $scope.onAccountPartDefinitionUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function insertAccountPartDefinition() {
            $scope.scopeModel.isLoading = true;

            var accountPartDefinitionObj = buildAccountPartDefinitionObjFromScope();

            return Retail_BE_AccountPartDefinitionAPIService.AddAccountPartDefinition(accountPartDefinitionObj).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('Account Section Definition', response, 'Name')) {
                    if ($scope.onAccountPartDefinitionAdded != undefined)
                        $scope.onAccountPartDefinitionAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildAccountPartDefinitionObjFromScope() {
            var accountPartDefinitionObj = {
                AccountPartDefinitionId: accountPartDefinitionId,
                Name: $scope.scopeModel.name,
                Title: $scope.scopeModel.title,
                Settings: accountPartDefinitionAPI.getData()
            };
            return accountPartDefinitionObj;
        }
    }

    appControllers.controller('Retail_BE_AccountPartDefinitionEditorController', AccountPartDefinitionEditorController);

})(appControllers);