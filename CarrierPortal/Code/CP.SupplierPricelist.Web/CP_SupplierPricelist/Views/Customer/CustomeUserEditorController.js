(function (appControllers) {

    "use strict";

    function customeUserEditorController($scope, utilsService, vrNotificationService, vrNavigationService, vruiUtilsService, customerManagmentApiService) {

        var customerId;
        var userDirectiveApi;
        var userReadyPromiseDeferred = utilsService.createPromiseDeferred();

        function loadParameters() {
            var parameters = vrNavigationService.getParameters($scope);
            if (parameters != undefined) {
                customerId = parameters.customerId;
            }

        }

        loadParameters();

        function buildCustomerFromScope() {
            var customerObject = {
                CustomerId: (customerId != undefined) ? customerId : 0,
                UserId: userDirectiveApi.getSelectedIds()
            }
            return customerObject;
        }

        function insertCustomerUSer() {
            var object = buildCustomerFromScope();
            return customerManagmentApiService.AddUser(object)
                .then(function (response) {
                    if (vrNotificationService.notifyOnItemAdded("User", response)) {
                        if ($scope.onCustomerUserAdded != undefined)
                            $scope.onCustomerUserAdded(response.UpdatedObject);
                        $scope.modalContext.closeModal();
                    }
                }).catch(function (error) {
                    vrNotificationService.notifyException(error, $scope);
                });

        }

        function defineScope() {
            $scope.carriers = [];
            $scope.selectedCarriers = [];
            $scope.close = function () {
                $scope.modalContext.closeModal();
            }

            $scope.onUserDirectiveReady = function (api) {
                userDirectiveApi = api;
                userReadyPromiseDeferred.resolve();
            }
            $scope.close = function () {
                $scope.modalContext.closeModal();
            }
            $scope.Save = function () {
                insertCustomerUSer();

            };
        }

        defineScope();

        function setTitle() {
            $scope.title = utilsService.buildTitleForAddEditor("User");
        }

        function loadUser() {
            var userLoadPromiseDeferred = utilsService.createPromiseDeferred();
            userReadyPromiseDeferred.promise.then(function () {

                vruiUtilsService.callDirectiveLoad(userDirectiveApi, undefined, userLoadPromiseDeferred);
            });
            return userLoadPromiseDeferred.promise;
        }

        function loadAllControls() {
            return utilsService.waitMultipleAsyncOperations([setTitle, loadUser])
                .catch(function (error) {
                    vrNotificationService.notifyExceptionWithClose(error, $scope);
                })
                .finally(function () {
                    $scope.isLoading = false;
                });
        }

        function load() {
            loadAllControls();
        }

        load();
    }

    customeUserEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'CP_SupplierPricelist_CustomerManagmentAPIService'];
    appControllers.controller('CP_SupplierPricelist_CustomeUserEditorController', customeUserEditorController);
})(appControllers);