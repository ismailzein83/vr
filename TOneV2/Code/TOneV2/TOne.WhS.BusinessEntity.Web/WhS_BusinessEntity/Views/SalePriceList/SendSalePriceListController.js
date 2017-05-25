(function (appControllers) {

    'use strict';

    SendSalePriceListController.$inject = ['$scope', 'WhS_BE_SalePricelistAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function SendSalePriceListController($scope, WhS_BE_SalePricelistAPIService, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var processInstanceId;

        var salePriceListGridAPI;
        var salePriceListGridReadyDeferred = UtilsService.createPromiseDeferred();

        var customerPriceListIds;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters) {
                processInstanceId = parameters.processInstanceId;
            }
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onSalePriceListGridReady = function (api) {
                salePriceListGridAPI = api;
                salePriceListGridReadyDeferred.resolve();
            };

            $scope.scopeModel.sendAll = function () {
                if (!doCustomerPriceListsExist())
                    return;

                $scope.scopeModel.isLoading = true;
                var promises = [];

                var sendCustomerPriceListsPromise = sendCustomerPriceLists();
                promises.push(sendCustomerPriceListsPromise);

                var loadSalePriceListGridDeferred = UtilsService.createPromiseDeferred();
                promises.push(loadSalePriceListGridDeferred.promise);

                sendCustomerPriceListsPromise.then(function () {
                    loadSalePriceListGrid().then(function () {
                        loadSalePriceListGridDeferred.resolve();
                    }).catch(function (error) {
                        loadSalePriceListGridDeferred.reject(error);
                    });
                });

                return UtilsService.waitMultiplePromises(promises).then(function () {
                    $scope.scopeModel.isSendAllButtonDisabled = true;
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }
        function load() {
            $scope.scopeModel.isLoading = true;

            getSalePriceListIdsByProcessInstanceId().then(function () {
                $scope.scopeModel.isSendAllButtonDisabled = !doCustomerPriceListsExist();
                loadAllControls();
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.scopeModel.isLoading = false;
            });
        }

        function getSalePriceListIdsByProcessInstanceId() {
            return WhS_BE_SalePricelistAPIService.GetSalePriceListIdsByProcessInstanceId(processInstanceId).then(function (response) {
                if (response != undefined) {
                    customerPriceListIds = [];
                    for (var i = 0; i < response.length; i++) {
                        customerPriceListIds.push(response[i]);
                    }
                }
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadSalePriceListGrid]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function setTitle() {
            $scope.title = 'Send Pricelists';
        }
        function loadSalePriceListGrid() {
            var salePriceListGridLoadDeferred = UtilsService.createPromiseDeferred();

            salePriceListGridReadyDeferred.promise.then(function () {
                var salePriceListGridPayload = {
                    query: {
                        IncludedSalePriceListIds: customerPriceListIds
                    }
                };
                VRUIUtilsService.callDirectiveLoad(salePriceListGridAPI, salePriceListGridPayload, salePriceListGridLoadDeferred);
            });

            return salePriceListGridLoadDeferred.promise;
        }

        function sendCustomerPriceLists() {
            return WhS_BE_SalePricelistAPIService.SendCustomerPriceLists(customerPriceListIds);
        }

        function doCustomerPriceListsExist() {
            return (customerPriceListIds != undefined && customerPriceListIds.length > 0);
        }
    }

    appControllers.controller('WhS_BE_SendSalePriceListController', SendSalePriceListController);

})(appControllers);