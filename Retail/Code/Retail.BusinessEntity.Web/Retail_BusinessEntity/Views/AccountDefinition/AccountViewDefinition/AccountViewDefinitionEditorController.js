(function (appControllers) {

    'use strict';

    AccountViewDefinitionEditorController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function AccountViewDefinitionEditorController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var isEditMode;

        var accountViewDefinitionEntity;
       
        var settingsDirectiveAPI;
        var settingsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                accountViewDefinitionEntity = parameters.accountViewDefinitionEntity;
            }
            isEditMode = (accountViewDefinitionEntity != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.onAccountViewDefinitionSettingsReady = function (api) {
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
                    UtilsService.buildTitleForUpdateEditor((accountViewDefinitionEntity != undefined) ? accountViewDefinitionEntity.Name : null, 'Account View Definition') :
                    UtilsService.buildTitleForAddEditor('Account View  Definition');
            }
            function loadStaticData() {
                if (accountViewDefinitionEntity == undefined)
                    return;
                $scope.scopeModel.name = accountViewDefinitionEntity.Name;
                $scope.scopeModel.drillDownSectionName = accountViewDefinitionEntity.DrillDownSectionName;
                $scope.scopeModel.account360DegreeSectionName = accountViewDefinitionEntity.Account360DegreeSectionName;

            }
            function loadSettingsDirective() {
                var settingsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                settingsDirectiveReadyDeferred.promise.then(function () {
                    var settingsDirectivePayload;
                    if (accountViewDefinitionEntity != undefined) {
                        settingsDirectivePayload = { accountViewDefinitionSettings: accountViewDefinitionEntity.Settings };
                    }
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
            var accountViewDefinitionObject = buildAccountViewDefinitionObjectFromScope();

            if ($scope.onAccountViewDefinitionAdded != undefined && typeof ($scope.onAccountViewDefinitionAdded) == 'function') {
                $scope.onAccountViewDefinitionAdded(accountViewDefinitionObject);
            }
            $scope.modalContext.closeModal();
        }
        function update() {
            var accountViewDefinitionObject = buildAccountViewDefinitionObjectFromScope();

            if ($scope.onAccountViewDefinitionUpdated != undefined && typeof ($scope.onAccountViewDefinitionUpdated) == 'function') {
                $scope.onAccountViewDefinitionUpdated(accountViewDefinitionObject);
            }
            $scope.modalContext.closeModal();
        }

        function buildAccountViewDefinitionObjectFromScope() {
            return {
                AccountViewDefinitionId:accountViewDefinitionEntity != undefined?accountViewDefinitionEntity.AccountViewDefinitionId:UtilsService.guid(),
                Name: $scope.scopeModel.name,
                DrillDownSectionName:$scope.scopeModel.drillDownSectionName,
                Account360DegreeSectionName: $scope.scopeModel.account360DegreeSectionName,
                Settings: settingsDirectiveAPI.getData(),
            };
        }
    }

    appControllers.controller('Retail_BE_AccountViewDefinitionEditorController', AccountViewDefinitionEditorController);

})(appControllers);