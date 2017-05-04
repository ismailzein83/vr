﻿"use strict";

app.directive("vrWhsSalesMarginratecalculation", ['WhS_Sales_BulkActionUtilsService', 'WhS_Sales_MarginRateCalculationMethodType', 'UtilsService', 'VRUIUtilsService', function (WhS_Sales_BulkActionUtilsService, WhS_Sales_MarginRateCalculationMethodType, UtilsService, VRUIUtilsService) {

    return {
        restrict: "E",
        scope: {
            onReady: "=",
            normalColNum: '@',
            isrequired: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var mixedRateCalculation = new MarginRateCalculation(ctrl, $scope);
            mixedRateCalculation.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/WhS_Sales/Directives/Extensions/RateCalculation/Templates/MarginRateCalculationTemplate.html"
    };

    function MarginRateCalculation(ctrl, $scope) {

        this.initializeController = initializeController;

        var bulkActionContext;

        var typeSelectorAPI;
        var typeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var costColumnSelectorAPI;
        var costColumnSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var optionNumberSelectorAPI;
        var optionNumberSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var supplierSelectorAPI;
        var supplierSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        function initializeController() {

            $scope.scopeModel = {};

            $scope.scopeModel.optionNumbers = [{ value: 1, description: '1' }, { value: 2, description: '2' }, { value: 3, description: '3' }];

            $scope.scopeModel.showMarginLabel = true;
            $scope.scopeModel.showMarginPercentageLabel = false;

            $scope.scopeModel.types = UtilsService.getArrayEnum(WhS_Sales_MarginRateCalculationMethodType);
            $scope.scopeModel.costColumns = [];

            $scope.scopeModel.onTypeSelectorReady = function (api) {
                typeSelectorAPI = api;
                typeSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onTypeSelected = function (selectedType) {
                $scope.scopeModel.showCostColumnSelector = false;
                $scope.scopeModel.showOptionNumberSelector = false;
                $scope.scopeModel.showSupplierSelector = false;

                switch (selectedType.value) {
                    case WhS_Sales_MarginRateCalculationMethodType.Cost.value:
                        $scope.scopeModel.showCostColumnSelector = true;
                        break;
                    case WhS_Sales_MarginRateCalculationMethodType.RPRouteOption.value:
                        $scope.scopeModel.showOptionNumberSelector = true;
                        break;
                    case WhS_Sales_MarginRateCalculationMethodType.Supplier.value:
                        $scope.scopeModel.showSupplierSelector = true;
                        break;
                }
            };

            $scope.scopeModel.onMarginChanged = function () {
                WhS_Sales_BulkActionUtilsService.onBulkActionChanged(bulkActionContext);
            };

            $scope.scopeModel.onIsPercentageChanged = function () {
                WhS_Sales_BulkActionUtilsService.onBulkActionChanged(bulkActionContext);
                $scope.scopeModel.showMarginLabel = !$scope.scopeModel.showMarginLabel;
                $scope.scopeModel.showMarginPercentageLabel = !$scope.scopeModel.showMarginPercentageLabel;
            };

            $scope.scopeModel.onCostColumnSelectorReady = function (api) {
                costColumnSelectorAPI = api;
                costColumnSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onOptionNumberSelectorReady = function (api) {
                optionNumberSelectorAPI = api;
                optionNumberSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onOptionNumberChanged = function () {
                WhS_Sales_BulkActionUtilsService.onBulkActionChanged(bulkActionContext);
            };

            $scope.scopeModel.onSupplierSelectorReady = function (api) {
                supplierSelectorAPI = api;
                supplierSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.validateMargin = function () {
                if ($scope.scopeModel.margin == undefined)
                    return;
                if ($scope.scopeModel.isPercentage) {
                    if ($scope.scopeModel.margin < 0 || $scope.scopeModel.margin > 100)
                        return 'Margin % must be between 0 and 100';
                }
                else {
                    if ($scope.scopeModel.margin < 0)
                        return 'Margin must be a postive number';
                }
                return null;
            };

            UtilsService.waitMultiplePromises([typeSelectorReadyDeferred.promise, costColumnSelectorReadyDeferred.promise, optionNumberSelectorReadyDeferred.promise, supplierSelectorReadyDeferred.promise]).then(function () {
                defineAPI();
            });
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {

                var rateCalculationMethod;

                if (payload != undefined) {
                    rateCalculationMethod = payload.rateCalculationMethod;
                    bulkActionContext = payload.bulkActionContext;
                }

                var selectedCostColumnConfigId;
                var selectedOptionNumberValue;
                var selectedSupplierId;

                if (rateCalculationMethod != undefined) {
                    $scope.scopeModel.margin = rateCalculationMethod.Margin;
                    $scope.scopeModel.isPercentage = rateCalculationMethod.IsPercentage;

                    selectedCostColumnConfigId = rateCalculationMethod.CostCalculationMethodConfigId;
                    selectedOptionNumberValue = rateCalculationMethod.RPRouteOptionNumber;
                    selectedSupplierId = rateCalculationMethod.SupplierId;
                }

                loadCostColumnSelector();
                loadOptionNumberSelector();

                var promises = [];

                var loadSupplierSelectorPromise = loadSupplierSelector();
                promises.push(loadSupplierSelectorPromise);

                function loadCostColumnSelector() {
                    if (bulkActionContext != undefined && bulkActionContext.costCalculationMethods != undefined) {
                        for (var i = 0; i < bulkActionContext.costCalculationMethods.length; i++) {
                            $scope.scopeModel.costColumns.push(bulkActionContext.costCalculationMethods[i]);
                        }
                        if (selectedCostColumnConfigId != undefined)
                            $scope.scopeModel.selectedCostColumn = UtilsService.getItemByVal($scope.scopeModel.costColumns, selectedCostColumnConfigId, 'ConfigId');
                    }
                }
                function loadOptionNumberSelector() {
                    if (selectedOptionNumberValue != undefined)
                        $scope.scopeModel.selectedOptionNumber = UtilsService.getItemByVal($scope.scopeModel.optionNumbers, selectedOptionNumberValue, 'value');
                }
                function loadSupplierSelector() {
                    var supplierSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                    var supplierSelectorPayload = {
                        selectedIds: selectedSupplierId
                    };
                    VRUIUtilsService.callDirectiveLoad(supplierSelectorAPI, supplierSelectorPayload, supplierSelectorLoadDeferred);

                    return supplierSelectorLoadDeferred.promise;
                }

                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function () {
                var data = {
                    $type: "TOne.WhS.Sales.MainExtensions.RateCalculation.MarginRateCalculationMethod, TOne.WhS.Sales.MainExtensions",
                    Margin: $scope.scopeModel.margin,
                    IsPercentage: $scope.scopeModel.isPercentage,
                    Type: $scope.scopeModel.selectedType.value,
                    SupplierId: supplierSelectorAPI.getSelectedIds()
                };
                if ($scope.scopeModel.selectedCostColumn != undefined)
                    data.CostCalculationMethodConfigId = $scope.scopeModel.selectedCostColumn.ConfigId;
                if ($scope.scopeModel.selectedOptionNumber != undefined)
                    data.RPRouteOptionNumber = $scope.scopeModel.selectedOptionNumber.value;
                return data;
            };

            api.getDescription = function () {
                return $scope.scopeModel.margin;
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);