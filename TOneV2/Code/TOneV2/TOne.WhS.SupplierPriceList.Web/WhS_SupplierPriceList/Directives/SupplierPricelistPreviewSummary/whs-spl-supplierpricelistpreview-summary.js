"use strict";

app.directive("whsSplSupplierpricelistpreviewSummary", ["WhS_SupPL_PreviewChangeTypeEnum", "WhS_SupPL_PreviewGroupedBy", "UtilsService", "VRUIUtilsService", "VRNotificationService", "WhS_SupPL_SupplierPriceListPreviewPIService","VRCommon_FileAPIService","VRCommon_CurrencyAPIService","WhS_SupPL_SupplierPriceListTypeEnum",
function (WhS_SupPL_PreviewChangeTypeEnum, WhS_SupPL_PreviewGroupedBy, UtilsService, VRUIUtilsService, VRNotificationService, WhS_SupPL_SupplierPriceListPreviewPIService, VRCommon_FileAPIService, VRCommon_CurrencyAPIService, WhS_SupPL_SupplierPriceListTypeEnum) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var supplierPriceListPreviewSummary = new SupplierPriceListPreviewSummary($scope, ctrl, $attrs);
            supplierPriceListPreviewSummary.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {
        },
        templateUrl: "/Client/Modules/WhS_SupplierPriceList/Directives/SupplierPriceListPreviewSummary/Templates/SupplierPriceListPreviewSummary.html"
    };

    function SupplierPriceListPreviewSummary($scope, ctrl, $attrs) {

        var processInstanceId;
        var currencyId;
        var fileId;

        var validationMessageHistoryGridAPI;
        var currencyReadyPromiseDeferred = UtilsService.createPromiseDeferred();


        this.initializeController = initializeController;

        function initializeController() {

            $scope.scopeModel = {};

            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {

                if (payload != null) {
                    processInstanceId = payload.processInstanceId;
                    //if (pricelistDate != null) {
                    //    pricelistDate = pricelistDate.replace("T", " ");
                    //}
                    currencyId = payload.currencyId;
                    fileId = payload.fileID;

                    $scope.scopeModel.pricelistDate = payload.pricelistDate;

                    

                    if (payload.supplierPricelistType != null) {
                        var pricelistTypesOptions = UtilsService.getArrayEnum(WhS_SupPL_SupplierPriceListTypeEnum);
                        $scope.scopeModel.supplierPricelistType = UtilsService.getItemByVal(pricelistTypesOptions, payload.supplierPricelistType, 'value').description;
                    }

                   
                }

                return UtilsService.waitMultipleAsyncOperations([loadSupplierPricelistPreviewSummary,loadCurrencySymbol,loadFileName])
                     .catch(function (error) {
                           VRNotificationService.notifyException(error);
                });
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function loadCurrencySymbol() {
            var currencyReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            if (currencyId != null) {
                VRCommon_CurrencyAPIService.GetCurrency(currencyId).then(function (currency) {
                    $scope.scopeModel.currencySymbol = currency.Symbol;
                    currencyReadyPromiseDeferred.resolve();
                });
            }
            else
                currencyReadyPromiseDeferred.resolve();

            return currencyReadyPromiseDeferred.promise;
        }

        function loadFileName() {
            return VRCommon_FileAPIService.GetFileName(fileId).then(function (response) {
                $scope.scopeModel.fileName = response;
            });
        }

        function loadSupplierPricelistPreviewSummary() {

            return WhS_SupPL_SupplierPriceListPreviewPIService.GetSupplierPricelistPreviewSummary(processInstanceId).then(function (previewSummary) {

                $scope.scopeModel.numberOfNewRates = previewSummary.NumberOfNewRates;
                $scope.scopeModel.numberOfIncreasedRates = previewSummary.NumberOfIncreasedRates;
                $scope.scopeModel.numberOfDecreasedRates = previewSummary.NumberOfDecreasedRates;
                $scope.scopeModel.numberOfNewOtherRates = previewSummary.NumberOfNewOtherRates;
                $scope.scopeModel.numberOfIncreasedOtherRates = previewSummary.NumberOfIncreasedOtherRates;
                $scope.scopeModel.numberOfDecreasedOtherRates = previewSummary.NumberOfDecreasedOtherRates;
                $scope.scopeModel.numberOfNewZones = previewSummary.NumberOfNewZones;
                $scope.scopeModel.numberOfRenamedZones = previewSummary.NumberOfRenamedZones;
                $scope.scopeModel.numberOfClosedZones = previewSummary.NumberOfClosedZones;
                $scope.scopeModel.numberOfNewCodes = previewSummary.NumberOfNewCodes;
                $scope.scopeModel.numberOfMovedCodes = previewSummary.NumberOfMovedCodes;
                $scope.scopeModel.numberOfClosedCodes = previewSummary.NumberOfClosedCodes;
                $scope.scopeModel.numberOfZonesWithChangeServices = previewSummary.NumberOfZonesWithChangedServices;
            });
        }
    }

    return directiveDefinitionObject;
}]);
