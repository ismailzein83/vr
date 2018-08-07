"use strict";

app.directive("whsSplSupplierpricelistpreviewSection", ["WhS_SupPL_PreviewChangeTypeEnum", "WhS_SupPL_PreviewGroupedBy", "UtilsService", "VRUIUtilsService", "VRNotificationService",
function (WhS_SupPL_PreviewChangeTypeEnum, WhS_SupPL_PreviewGroupedBy, UtilsService, VRUIUtilsService, VRNotificationService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var supplierPriceListPreviewSection = new SupplierPriceListPreviewSection($scope, ctrl, $attrs);
            supplierPriceListPreviewSection.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {
        },
        templateUrl: "/Client/Modules/WhS_SupplierPriceList/Directives/SupplierPriceListPreviewSection/Templates/SupplierPriceListPreviewSectionTemplate.html"
    };

    function SupplierPriceListPreviewSection($scope, ctrl, $attrs) {

        var processInstanceId;
        var taskData;
        var requireWarningConfirmation;
        var pricelistDate;
        var currencySymbol;
        var supplierPricelistType;
        var fileId;
        var currencyId;
        var changeType = true;

        var validationMessageHistoryGridAPI;
        var validationMessageHistoryReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var supplierPricelistPreviewSummaryAPI;
        var supplierPricelistPreviewSummaryReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var supplierPricelistPreviewExcludedDataAPI;
        var supplierPricelistPreviewExcludedDataReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var supplierPricelistPreviewDataAPI;
        var supplierPricelistPreviewDataReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        this.initializeController = initializeController;

        function initializeController() {

            $scope.onViewChangeTypeSelectItem = function (dataItem) {
                if (dataItem != undefined) {
                    directiveWrapperReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                    directiveWrapperReadyPromiseDeferred.resolve();
                    changeType = dataItem.value;
                    return loadPreviewDataSection();
                }
            };

            $scope.onValidationMessageHistoryGridReady = function (api) {
                validationMessageHistoryGridAPI = api;
                validationMessageHistoryReadyPromiseDeferred.resolve();
            };

            $scope.onSupplierPricelistPreviewSummaryReady = function (api) {
                supplierPricelistPreviewSummaryAPI = api;
                supplierPricelistPreviewSummaryReadyPromiseDeferred.resolve();
            };

            $scope.onSupplierPricelistPreviewDataReady = function (api) {
                supplierPricelistPreviewDataAPI = api;
                supplierPricelistPreviewDataReadyPromiseDeferred.resolve();
            };
            $scope.onSupplierPricelistPreviewExcludedDataReady = function (api) {
                supplierPricelistPreviewExcludedDataAPI = api;
                supplierPricelistPreviewExcludedDataReadyPromiseDeferred.resolve();
            };
            defineAPI();
        }

        function defineAPI() {
            var api = {};
            api.load = function (payload) {

                if (payload != null) {
                    processInstanceId = payload.processInstanceId;
                    pricelistDate = payload.pricelistDate;
                    currencyId = payload.currencyId;
                    supplierPricelistType = payload.supplierPricelistType;
                    fileId = payload.fileId;
                    requireWarningConfirmation = payload.requireWarningConfirmation;
                }
                return UtilsService.waitMultipleAsyncOperations([loadValidationMessageHistory, loadSupplierPricelistPreviewSummary, loadSupplierPricelistPreviewData, loadSupplierPricelistPreviewExcludedData])
                          .catch(function (error) {
                              VRNotificationService.notifyException(error);
                          });
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }


        function loadSupplierPricelistPreviewSummary() {
            var loadSupplierPricelistPreviewSummaryPromiseDeferred = UtilsService.createPromiseDeferred();

            supplierPricelistPreviewSummaryReadyPromiseDeferred.promise.then(function () {
                var payload = {
                    processInstanceId: processInstanceId,
                    pricelistDate: pricelistDate, 
                    fileID: fileId,
                    currencyId: currencyId, 
                    supplierPricelistType: supplierPricelistType,
                };
                VRUIUtilsService.callDirectiveLoad(supplierPricelistPreviewSummaryAPI, payload, loadSupplierPricelistPreviewSummaryPromiseDeferred);
            });

            return loadSupplierPricelistPreviewSummaryPromiseDeferred.promise;
        }
        function loadSupplierPricelistPreviewData() {
            var loadSupplierPricelistPreviewDataPromiseDeferred = UtilsService.createPromiseDeferred();

            supplierPricelistPreviewDataReadyPromiseDeferred.promise.then(function () {
                var payload = {
                    processInstanceId: processInstanceId,
                };
                VRUIUtilsService.callDirectiveLoad(supplierPricelistPreviewDataAPI, payload, loadSupplierPricelistPreviewDataPromiseDeferred);
            });

            return loadSupplierPricelistPreviewDataPromiseDeferred.promise;
        }


        function loadSupplierPricelistPreviewExcludedData() {
            var loadSupplierPricelistPreviewExcludedDataPromiseDeferred = UtilsService.createPromiseDeferred();

            supplierPricelistPreviewExcludedDataReadyPromiseDeferred.promise.then(function () {
                var payload = {
                    processInstanceId: processInstanceId,
                    isExcluded : true
                };
                VRUIUtilsService.callDirectiveLoad(supplierPricelistPreviewExcludedDataAPI, payload, loadSupplierPricelistPreviewExcludedDataPromiseDeferred);
            });

            return loadSupplierPricelistPreviewExcludedDataPromiseDeferred.promise;
        }
        function loadValidationMessageHistory() {
            var loadValidationMessageHistoryPromiseDeferred = UtilsService.createPromiseDeferred();

            validationMessageHistoryReadyPromiseDeferred.promise.then(function () {

                var payload = {
                    BPInstanceID: processInstanceId,
                    requireWarningConfirmation: requireWarningConfirmation,
                };

                VRUIUtilsService.callDirectiveLoad(validationMessageHistoryGridAPI, payload, loadValidationMessageHistoryPromiseDeferred);
            });

            return loadValidationMessageHistoryPromiseDeferred.promise;
        }

    }

    return directiveDefinitionObject;
}]);
