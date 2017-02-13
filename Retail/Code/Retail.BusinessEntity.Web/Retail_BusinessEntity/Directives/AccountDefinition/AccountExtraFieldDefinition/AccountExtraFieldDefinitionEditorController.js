(function (appControllers) {

    'use strict';

    AccountExtraFieldDefinitionEditorController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function AccountExtraFieldDefinitionEditorController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var isEditMode;

        var accountBEDefinitionId;
        var accountExtraFieldDefinitionEntity;

        var settingsDirectiveAPI;
        var settingsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();


        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                accountBEDefinitionId = parameters.accountBEDefinitionId;
                accountExtraFieldDefinitionEntity = parameters.accountExtraFieldDefinitionEntity;
            }
            isEditMode = (accountExtraFieldDefinitionEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onAccountExtraFieldDefinitionSettingsReady = function (api) {
                settingsDirectiveAPI = api;
                settingsDirectiveReadyDeferred.resolve();
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

            function setTitle() {
                $scope.title = (isEditMode) ?
                    UtilsService.buildTitleForUpdateEditor((accountExtraFieldDefinitionEntity != undefined) ? accountExtraFieldDefinitionEntity.Name : null, 'Extra Field') :
                    UtilsService.buildTitleForAddEditor('Extra Field');
            }
            function loadStaticData() {
                if (accountExtraFieldDefinitionEntity == undefined)
                    return;
                $scope.scopeModel.name = accountExtraFieldDefinitionEntity.Name;

            }
            function loadSettingsDirective() {
                var settingsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                settingsDirectiveReadyDeferred.promise.then(function () {

                    var settingsDirectivePayload = {
                        accountBEDefinitionId: accountBEDefinitionId,
                        accountExtraFieldDefinitionSettings: accountExtraFieldDefinitionEntity != undefined ? accountExtraFieldDefinitionEntity.Settings : undefined
                    };
                    VRUIUtilsService.callDirectiveLoad(settingsDirectiveAPI, settingsDirectivePayload, settingsDirectiveLoadDeferred);
                });

                return settingsDirectiveLoadDeferred.promise;
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadSettingsDirective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function insert() {
            var accountExtraFieldDefinitionObject = buildAccountExtraFieldDefinitionObjectFromScope();

            if ($scope.onAccountExtraFieldDefinitionAdded != undefined && typeof ($scope.onAccountExtraFieldDefinitionAdded) == 'function') {
                $scope.onAccountExtraFieldDefinitionAdded(accountExtraFieldDefinitionObject);
            }
            $scope.modalContext.closeModal();
        }

        function update() {
            var accountExtraFieldDefinitionObject = buildAccountExtraFieldDefinitionObjectFromScope();

            if ($scope.onAccountExtraFieldDefinitionUpdated != undefined && typeof ($scope.onAccountExtraFieldDefinitionUpdated) == 'function') {
                $scope.onAccountExtraFieldDefinitionUpdated(accountExtraFieldDefinitionObject);
            }
            $scope.modalContext.closeModal();
        }

        function buildAccountExtraFieldDefinitionObjectFromScope() {
            return {
                AccountExtraFieldDefinitionId: accountExtraFieldDefinitionEntity != undefined ? accountExtraFieldDefinitionEntity.AccountExtraFieldDefinitionId : UtilsService.guid(),
                Name: $scope.scopeModel.name,
                Settings: settingsDirectiveAPI.getData()
            };
        }

    }

    appControllers.controller('Retail_BE_AccountExtraFieldDefinitionEditorController', AccountExtraFieldDefinitionEditorController);

})(appControllers);