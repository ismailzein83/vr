(function (appControllers) {

    'use strict';

    ChangeStatusActionEditorController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService', 'Retail_BE_ChangeStatusActionAPIService'];

    function ChangeStatusActionEditorController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, Retail_BE_ChangeStatusActionAPIService) {

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
            $scope.scopeModel.save = function () {
                $scope.scopeModel.isLoading = true;
                return changeStatus();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }
        function load() {
        }
      
        function changeStatus()
        {
            return Retail_BE_ChangeStatusActionAPIService.ChangeAccountStatus(accountBEDefinitionId, accountId, accountActionDefinitionId).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('Status', response, 'Name')) {
                    if ($scope.onItemUpdated != undefined)
                        $scope.onItemUpdated(response.UpdatedObject);
                    $scope.scopeModel.isLoading = false;
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
                $scope.scopeModel.isLoading = false;
            });
        }
    
    }

    appControllers.controller('Retail_BE_ChangeStatusActionEditorController', ChangeStatusActionEditorController);

})(appControllers);