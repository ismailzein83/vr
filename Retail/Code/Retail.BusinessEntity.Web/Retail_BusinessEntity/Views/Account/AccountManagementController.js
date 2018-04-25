(function (appControllers) {

    'use strict';

    AccountManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];
    function AccountManagementController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var viewId;

        var accountManagementDirectiveAPI;
        var accountManagementDirectiveReadyDeferred = UtilsService.createPromiseDeferred();
        
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != null) {
                viewId = parameters.viewId;
            }
        }
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.onAccountManagementDirectiveReady = function (api) {
                accountManagementDirectiveAPI = api;
                accountManagementDirectiveReadyDeferred.resolve();
            };
        }
        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
            function loadAllControls() {

                function loadAccountManagementDirective() {
                    var accountManagementDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                    accountManagementDirectiveReadyDeferred.promise.then(function () {
                        var payload = {
                            viewId: viewId
                        };

                        VRUIUtilsService.callDirectiveLoad(accountManagementDirectiveAPI, payload, accountManagementDirectiveLoadDeferred);
                    });
                    return accountManagementDirectiveLoadDeferred.promise;
                }

                return UtilsService.waitMultiplePromises([loadAccountManagementDirective()]).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
            }
        }
    }
    appControllers.controller('Retail_BE_AccountManagementController', AccountManagementController);

})(appControllers);