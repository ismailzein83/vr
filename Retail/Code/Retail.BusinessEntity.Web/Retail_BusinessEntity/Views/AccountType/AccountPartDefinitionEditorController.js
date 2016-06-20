(function (appControllers) {

    'use strict';

    AccountPartDefinitionEditorController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService','Retail_BE_AccountPartAvailabilityOptionsEnum','Retail_BE_AccountPartRequiredOptionsEnum'];

    function AccountPartDefinitionEditorController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, Retail_BE_AccountPartAvailabilityOptionsEnum, Retail_BE_AccountPartRequiredOptionsEnum) {
        var isEditMode;

        var accountPartDefinitionEntity;
        var accountPartDefinitionAPI;
        var accountPartDefinitionReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                accountPartDefinitionEntity = parameters.accountPartDefinitionEntity;
            }

            isEditMode = (accountPartDefinitionEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};


            $scope.scopeModel.accountPartAvailability = UtilsService.getArrayEnum(Retail_BE_AccountPartAvailabilityOptionsEnum);

            $scope.scopeModel.accountPartRequiredOptions = UtilsService.getArrayEnum(Retail_BE_AccountPartRequiredOptionsEnum);

            $scope.scopeModel.accountPartDefinitionDirectiveReady = function (api) {
                accountPartDefinitionAPI = api;
                accountPartDefinitionReadyDeferred.resolve();
            }

            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateAccountPartDefinition() : insertAccountPartDefinition();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };

        }

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle,loadStaticData, loadAccountPartDefinitionTypes]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function loadAccountPartDefinitionTypes() {
            var loadAccountPartDefinitionPromiseDeferred = UtilsService.createPromiseDeferred();
            accountPartDefinitionReadyDeferred.promise.then(function () {
                var payloadDirective;
                if(accountPartDefinitionEntity !=undefined)
                    payloadDirective = { partDefinitionSettings: accountPartDefinitionEntity.Settings };
                VRUIUtilsService.callDirectiveLoad(accountPartDefinitionAPI, payloadDirective, loadAccountPartDefinitionPromiseDeferred);
            });
            return loadAccountPartDefinitionPromiseDeferred.promise;
        }

        function loadStaticData()
        {
            if(accountPartDefinitionEntity !=undefined)
            {
                $scope.scopeModel.partUniqueName = accountPartDefinitionEntity.PartUniqueName;
                $scope.scopeModel.selectedAccountPartAvailability = UtilsService.getItemByVal($scope.scopeModel.accountPartAvailability, accountPartDefinitionEntity.AvailabilitySettings, "value");
                $scope.scopeModel.selectedAccountPartRequiredOptions = UtilsService.getItemByVal($scope.scopeModel.accountPartRequiredOptions, accountPartDefinitionEntity.RequiredSettings, "value");
            }
        }
        function setTitle() {
            if (isEditMode) {
                var partUniqueName = (accountPartDefinitionEntity != undefined) ? accountPartDefinitionEntity.PartUniqueName : undefined;
                $scope.title = UtilsService.buildTitleForUpdateEditor(partUniqueName, 'Account Part Definition');
            }
            else {
                $scope.title = UtilsService.buildTitleForAddEditor('Account Part Definition');
            }
        }

        function updateAccountPartDefinition() {
            var accountPartDefinitionObj = buildAccountPartDefinitionObjFromScope();

            if ($scope.onAccountPartDefinitionUpdated != undefined) {
                $scope.onAccountPartDefinitionUpdated(accountPartDefinitionObj);
            }
            $scope.modalContext.closeModal();
        }

        function insertAccountPartDefinition() {
            var accountPartDefinitionObj = buildAccountPartDefinitionObjFromScope();
            if ($scope.onAccountPartDefinitionAdded != undefined) {
                $scope.onAccountPartDefinitionAdded(accountPartDefinitionObj);
            }
            $scope.modalContext.closeModal();
        }

        function buildAccountPartDefinitionObjFromScope() {
            var accountPartDefinitionObj = {
                PartUniqueName: $scope.scopeModel.partUniqueName,
                AvailabilitySettings: $scope.scopeModel.selectedAccountPartAvailability.value,
                RequiredSettings: $scope.scopeModel.selectedAccountPartRequiredOptions.value,
                Settings: accountPartDefinitionAPI.getData()
            };
            return accountPartDefinitionObj;
        }
    }

    appControllers.controller('Retail_BE_AccountPartDefinitionEditorController', AccountPartDefinitionEditorController);

})(appControllers);