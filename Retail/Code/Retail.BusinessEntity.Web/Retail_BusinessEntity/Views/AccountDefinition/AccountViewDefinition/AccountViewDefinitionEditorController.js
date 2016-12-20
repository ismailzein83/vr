(function (appControllers) {

    'use strict';

    AccountViewDefinitionEditorController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function AccountViewDefinitionEditorController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var isEditMode;

        var accountViewDefinitionEntity;
       

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
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData]).catch(function (error) {
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
                Name:$scope.scopeModel.name
            };
        }
    }

    appControllers.controller('Retail_BE_AccountViewDefinitionEditorController', AccountViewDefinitionEditorController);

})(appControllers);