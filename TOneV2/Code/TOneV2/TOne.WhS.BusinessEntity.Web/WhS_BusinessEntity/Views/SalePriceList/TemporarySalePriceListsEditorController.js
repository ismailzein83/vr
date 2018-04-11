(function (appControllers) {

    "use strict";

    temporarySalePriceListEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'VRNavigationService', 'WhS_BE_SalePriceListChangeAPIService'];

    function temporarySalePriceListEditorController($scope, UtilsService, VRNotificationService, vruiUtilsService, VRNavigationService, WhS_BE_SalePriceListChangeAPIService) {
        var customers;

        var processInstanceId;

        var doesCustomerTemporaryPricelistsExists;

        var pricelistGridApi;
        var pricelistGridReadyDeferred = UtilsService.createPromiseDeferred();

       
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                processInstanceId = parameters.processInstanceId;
            }
        }

        function defineScope() {
            $scope.onPricelistGridReady = function (api) {
                pricelistGridApi = api;
                pricelistGridReadyDeferred.resolve();
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.isLoading = true;
            WhS_BE_SalePriceListChangeAPIService.DoCustomerTemporaryPricelistsExists(processInstanceId).then(function (response) {
                doesCustomerTemporaryPricelistsExists = response;
                $scope.doCustomerPricelistsExist = doCustomerPricelistsExist;
                loadAllControls();
            })
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            })
           .finally(function () {
               $scope.isLoading = false;
           });
        }


        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadPricelistGrid])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }

        function setTitle() {
            $scope.title = "Sale Pricelists";
        }
        function loadStaticData() {
        }
        function loadPricelistGrid() {
            var pricelistGridLoadDeferred = UtilsService.createPromiseDeferred();
            pricelistGridReadyDeferred.promise.then(function () {
                var pricelistGridPayload = {
                    query: {
                        ProcessInstanceId: processInstanceId
                    }
                };
                vruiUtilsService.callDirectiveLoad(pricelistGridApi, pricelistGridPayload, pricelistGridLoadDeferred);
            });
            return pricelistGridLoadDeferred.promise;
        }
        function doCustomerPricelistsExist() {
            return doesCustomerTemporaryPricelistsExists;
        }


    }

    appControllers.controller('WhS_BE_TemporarySalePriceListsEditorController', temporarySalePriceListEditorController);
})(appControllers);
