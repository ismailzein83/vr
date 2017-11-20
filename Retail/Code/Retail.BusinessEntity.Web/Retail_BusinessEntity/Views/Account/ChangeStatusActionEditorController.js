(function (appControllers) {

    'use strict';

    ChangeStatusActionEditorController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService', 'Retail_BE_ChangeStatusActionAPIService', 'VRDateTimeService', 'VRValidationService'];

    function ChangeStatusActionEditorController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, Retail_BE_ChangeStatusActionAPIService, VRDateTimeService, VRValidationService) {

        var accountBEDefinitionId;
        var accountId;
        var accountActionDefinitionId;
        var accountViewDefinitions;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                accountBEDefinitionId = parameters.accountBEDefinitionId;
                accountId = parameters.accountId;
                accountActionDefinitionId = parameters.accountActionDefinitionId;
            }
        }
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.statusChangedDate = VRDateTimeService.getNowDateTime();
            $scope.scopeModel.save = function () {
                $scope.scopeModel.isLoading = true;
                return changeStatus();
            };
            $scope.scopeModel.validateChangeStatusDate = function () {
                if ($scope.scopeModel.statusChangedDate > new Date())
                {
                    return "Date cannot be greater than today.";
                }
                return null;
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
            return UtilsService.waitMultipleAsyncOperations([setTitle]).then(function () {
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function setTitle() {
            $scope.title = 'Change Status';
        }
        function changeStatus()
        {
            return Retail_BE_ChangeStatusActionAPIService.ChangeAccountStatus(accountBEDefinitionId, accountId, accountActionDefinitionId, $scope.scopeModel.statusChangedDate).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('Status', response, 'Name')) {
                    if ($scope.onItemUpdated != undefined)
                        $scope.onItemUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
                $scope.scopeModel.isLoading = false;
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
                $scope.scopeModel.isLoading = false;
            });
        }
    
    }

    appControllers.controller('Retail_BE_ChangeStatusActionEditorController', ChangeStatusActionEditorController);

})(appControllers);