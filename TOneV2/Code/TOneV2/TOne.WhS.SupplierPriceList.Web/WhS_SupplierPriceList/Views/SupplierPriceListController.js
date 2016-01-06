SupplierPriceListController.$inject = ['$scope', 'WhS_SupPL_SupplierPriceListAPIService', 'WhS_BP_CreateProcessResultEnum', 'BusinessProcessService', 'VRUIUtilsService', 'UtilsService', 'WhS_SupPL_SupplierPriceListService'];

function SupplierPriceListController($scope, WhS_SupPL_SupplierPriceListAPIService, WhS_BP_CreateProcessResultEnum, BusinessProcessService, VRUIUtilsService, UtilsService, WhS_SupPL_SupplierPriceListService) {
    defineScope();
    var carrierAccountDirectiveAPI;
    var carrierAccountReadyPromiseDeferred;

    var currencyDirectiveAPI;
    var currencyReadyPromiseDeferred;

    loadParameters();
    load();
    function loadParameters() {
    }
    function defineScope() {
        $scope.selectedSupplier;
        $scope.selectedCurrency;
        $scope.effectiveDate = new Date();
        $scope.zoneList;
        $scope.upload = function () {
            var input = {
                SupplierId: $scope.selectedSupplier.CarrierAccountId,
                CurrencyId: $scope.selectedCurrency.CurrencyId,
                FileId: $scope.zoneList.fileId,
                EffectiveDate: $scope.effectiveDate
            };

            return WhS_SupPL_SupplierPriceListAPIService.UploadSupplierPriceList(input).then(function (response) {
                    if (response.Result == WhS_BP_CreateProcessResultEnum.Succeeded.value)
                        return BusinessProcessService.openProcessTracking(response.ProcessInstanceId);
                });
        }
        $scope.onCurrencyDirectiveReady = function (api) {
            currencyDirectiveAPI = api;
            var setLoader = function (value) { $scope.isLoadingCurrencies = value };
            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, currencyDirectiveAPI, undefined, setLoader, currencyReadyPromiseDeferred);
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

        $scope.effectiveDate = '';
        

    }
    function load() {
     
    }
 
   
};
appControllers.controller('WhS_SupPL_SupplierPriceListController', SupplierPriceListController);