(function (appControllers) {

    'use strict';

    SupplierPriceListController.$inject = ['$scope', 'WhS_SupPL_SupplierPriceListAPIService', 'WhS_BP_CreateProcessResultEnum', 'WhS_BE_CarrierAccountAPIService', 'BusinessProcess_BPInstanceService', 'VRUIUtilsService', 'UtilsService', 'WhS_SupPL_SupplierPriceListService', 'BusinessProcess_BPInstanceAPIService'];

    function SupplierPriceListController($scope, WhS_SupPL_SupplierPriceListAPIService, WhS_BP_CreateProcessResultEnum, WhS_BE_CarrierAccountAPIService, BusinessProcess_BPInstanceService, VRUIUtilsService, UtilsService, WhS_SupPL_SupplierPriceListService, BusinessProcess_BPInstanceAPIService) {
        var carrierAccountDirectiveAPI;
        var carrierAccountReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var currencyDirectiveAPI;
        var currencyReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();

        defineScope();

        load();

        function loadParameters() {
        }

        function defineScope() {

            $scope.selectedSupplier;

            $scope.zoneList;

            $scope.priceListDate = new Date();

            $scope.upload = function () {
                var inputArguments = {
                    $type: "TOne.WhS.SupplierPriceList.BP.Arguments.SupplierPriceListProcessInput, TOne.WhS.SupplierPriceList.BP.Arguments",
                    SupplierAccountId: carrierAccountDirectiveAPI.getSelectedIds(),
                    CurrencyId: currencyDirectiveAPI.getSelectedIds(),
                    FileId: $scope.zoneList.fileId,
                    DeletedCodesDate: $scope.priceListDate
                };
                var input = {
                    InputArguments: inputArguments
                };

                return BusinessProcess_BPInstanceAPIService.CreateNewProcess(input).then(function (response) {
                    if (response.Result == WhS_BP_CreateProcessResultEnum.Succeeded.value)
                        return BusinessProcess_BPInstanceService.openProcessTracking(response.ProcessInstanceId);
                });
            }

            $scope.onCurrencyDirectiveReady = function (api) {
                currencyDirectiveAPI = api;
                currencyReadyPromiseDeferred.resolve();
            }

            $scope.onCarrierAccountDirectiveReady = function (api) {
                carrierAccountDirectiveAPI = api;
                carrierAccountReadyPromiseDeferred.resolve();
            }

            $scope.downloadTemplate = function () {
                return WhS_SupPL_SupplierPriceListAPIService.DownloadSupplierPriceListTemplate().then(function (response) {
                    UtilsService.downloadFile(response.data, response.headers);
                });
            }

            $scope.previewSupplierPriceList = function () {
                WhS_SupPL_SupplierPriceListService.previewSupplierPriceList(228);
            }       


            $scope.carrierAccountSelectItem = function (dataItem) {

                var selectedCarrierAccountId = dataItem.CarrierAccountId;

                if (selectedCarrierAccountId != undefined) {
                    $scope.isLoadingCurrencySelector = true;
                    WhS_BE_CarrierAccountAPIService.GetCarrierAccountCurrency(selectedCarrierAccountId).then(function (currencyId) {

                       currencyDirectiveAPI.selectedCurrency(currencyId);
                       $scope.isLoadingCurrencySelector = false;
                   })
                }
            }
        }

        function load() {
            $scope.isLoadingFilter = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadCarrierAccountSelector, loadCurrencySelector])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
               .finally(function () {
                   $scope.isLoadingFilter = false;
               });
        }


        function loadCarrierAccountSelector() {
            var loadCarrierAccountPromiseDeferred = UtilsService.createPromiseDeferred();

            carrierAccountReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(carrierAccountDirectiveAPI, undefined, loadCarrierAccountPromiseDeferred)


            })

            return loadCarrierAccountPromiseDeferred.promise;

        }



        function loadCurrencySelector() {
            var loadCurrencySelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            currencyReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(currencyDirectiveAPI, undefined, loadCurrencySelectorPromiseDeferred)

            })
            return loadCurrencySelectorPromiseDeferred.promise;

        }

    }

    appControllers.controller('WhS_SupPL_SupplierPriceListController', SupplierPriceListController);

})(appControllers);
