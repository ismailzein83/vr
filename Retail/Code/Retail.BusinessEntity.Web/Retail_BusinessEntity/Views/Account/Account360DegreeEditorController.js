(function (appControllers) {

    'use strict';

    Account360DegreeEditorController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService','Retail_BE_AccountAPIService'];

    function Account360DegreeEditorController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, Retail_BE_AccountAPIService) {

        var accountId;
        var accountEntity;
        var accountViewsDefinitions;
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                accountId = parameters.accountId;
            }
        }
        function defineScope() {
            $scope.scopeModel = {};
           
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }
        function load() {
            $scope.scopeModel.isLoading = true;
            UtilsService.waitMultipleAsyncOperations([getAccount, getAccountViewsDefinitions]).then(function () {
                loadAllControls();
            });
        }
        function getAccount()
        {
            return Retail_BE_AccountAPIService.GetAccount(accountId).then(function (response) {
                accountEntity = response;
            });
        }
        function getAccountViewsDefinitions()
        {

        }
        function loadAllControls() {

            function setTitle() {
                $scope.title = (accountEntity != undefined) ? accountEntity.Name : undefined;
            }
            function loadStaticData() {
                if (accountEntity == undefined)
                    return;
            }
            
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
    }

    appControllers.controller('Retail_BE_Account360DegreeEditorController', Account360DegreeEditorController);

})(appControllers);