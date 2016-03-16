(function (appControllers) {

    'use strict';

    SupplierPriceListController.$inject = ['$scope', 'WhS_SupPL_SupplierPriceListAPIService', 'WhS_BP_CreateProcessResultEnum', 'WhS_BE_CarrierAccountAPIService', 'BusinessProcessService', 'VRUIUtilsService', 'UtilsService', 'WhS_SupPL_SupplierPriceListService'];

    function SupplierPriceListController($scope, WhS_SupPL_SupplierPriceListAPIService, WhS_BP_CreateProcessResultEnum, WhS_BE_CarrierAccountAPIService, BusinessProcessService, VRUIUtilsService, UtilsService, WhS_SupPL_SupplierPriceListService) {
        var carrierAccountDirectiveAPI;
        var carrierAccountReadyPromiseDeferred;

        var currencyDirectiveAPI;
        var currencyReadyPromiseDeferred;

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
                var input = {
                    SupplierId: $scope.selectedSupplier.CarrierAccountId,
                    CurrencyId: currencyDirectiveAPI.getSelectedIds(),
                    FileId: $scope.zoneList.fileId,
                    PriceListDate: $scope.priceListDate
                };

                return WhS_SupPL_SupplierPriceListAPIService.UploadSupplierPriceList(input).then(function (response) {
                    if (response.Result == WhS_BP_CreateProcessResultEnum.Succeeded.value)
                        return BusinessProcessService.openProcessTracking(response.ProcessInstanceId);
                });
            }

            $scope.onCurrencyDirectiveReady = function (api) {
                currencyDirectiveAPI = api;
            }

            $scope.onCarrierAccountDirectiveReady = function (api) {
                carrierAccountDirectiveAPI = api;
                var setLoader = function (value) { $scope.isLoadingSuppliers = value };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, carrierAccountDirectiveAPI, undefined, setLoader, carrierAccountReadyPromiseDeferred);
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
                    WhS_BE_CarrierAccountAPIService.GetCarrierAccount(selectedCarrierAccountId).then(function (response) {

                        var setLoader = function (value) { $scope.isLoadingCurrencies = value };

                        var payload = {
                            selectedIds: response.CarrierAccountSettings != undefined ? response.CarrierAccountSettings.CurrencyId : undefined
                        }

                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, currencyDirectiveAPI, payload, setLoader, currencyReadyPromiseDeferred);
                    })
                }

            }
        }

        function load() {
     
        }
    }

    appControllers.controller('WhS_SupPL_SupplierPriceListController', SupplierPriceListController);

})(appControllers);
