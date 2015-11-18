SupplierPriceListController.$inject = ['$scope', 'WhS_SupPL_SupplierPriceListAPIService', 'WhS_BP_CreateProcessResultEnum', 'BusinessProcessService','VRUIUtilsService','UtilsService'];

function SupplierPriceListController($scope, WhS_SupPL_SupplierPriceListAPIService, WhS_BP_CreateProcessResultEnum, BusinessProcessService, VRUIUtilsService, UtilsService) {
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
            return WhS_SupPL_SupplierPriceListAPIService.UploadSupplierPriceList($scope.selectedSupplier.CarrierAccountId,$scope.selectedCurrency.CurrencyId, $scope.zoneList.fileId, $scope.effectiveDate == undefined ? {} : $scope.effectiveDate).then(function (response) {
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

    }
    function load() {
     
    }
 
   
};
appControllers.controller('WhS_SupPL_SupplierPriceListController', SupplierPriceListController);