'use strict';

app.directive('vrWhsSalesBulkactionValidationresultRate', ['WhS_Sales_RatePlanUtilsService', 'UtilsService', 'VRUIUtilsService', function (WhS_Sales_RatePlanUtilsService, UtilsService, VRUIUtilsService) {
    return {
        restrict: "E",
        scope: {
            onReady: "=",
            normalColNum: '@'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var rateValidationResult = new RateValidationResult($scope, ctrl, $attrs);
            rateValidationResult.initializeController();
        },
        controllerAs: "rateValidationResultCtrl",
        bindToController: true,
        templateUrl: '/Client/Modules/WhS_Sales/Directives/Extensions/BulkAction/ValidationResult/Template/RateValidationResultTemplate.html'
    };

    function RateValidationResult($scope, ctrl, $attrs) {

        this.initializeController = initializeController;

        var rateBulkAction;
        var rateBulkActionBED;

        var pricingSettings;
        var newRateDayOffset;
        var increasedRateDayOffset;
        var decreasedRateDayOffset;

        var pageSize = 10;

        var emptyRates;
        var zeroRates;
        var negativeRates;
        var duplicateRates;

        var emptyRateGridReadyDeferred = UtilsService.createPromiseDeferred();
        var zeroRateGridReadyDeferred = UtilsService.createPromiseDeferred();
        var negativeRateGridReadyDeferred = UtilsService.createPromiseDeferred();
        var duplicateRateGridReadyDeferred = UtilsService.createPromiseDeferred();

        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.emptyRates = [];
            $scope.scopeModel.zeroRates = [];
            $scope.scopeModel.negativeRates = [];
            $scope.scopeModel.duplicateRates = [];

            $scope.scopeModel.onEmptyRateGridReady = function (api) {
                emptyRateGridReadyDeferred.resolve();
            };
            $scope.scopeModel.loadMoreEmptyRates = function () {
                loadMoreGridData($scope.scopeModel.emptyRates, emptyRates);
            };

            $scope.scopeModel.onZeroRateGridReady = function (api) {
                zeroRateGridReadyDeferred.resolve();
            };
            $scope.scopeModel.loadMoreZeroRates = function () {
                loadMoreGridData($scope.scopeModel.zeroRates, zeroRates);
            };

            $scope.scopeModel.onNegativeRateGridReady = function (api) {
                negativeRateGridReadyDeferred.resolve();
            };
            $scope.scopeModel.loadMoreNegativeRates = function () {
                loadMoreGridData($scope.scopeModel.negativeRates, negativeRates);
            };

            $scope.scopeModel.onDuplicateRateGridReady = function (api) {
                duplicateRateGridReadyDeferred.resolve();
            };
            $scope.scopeModel.loadMoreDuplicateRates = function () {
                loadMoreGridData($scope.scopeModel.duplicateRates, duplicateRates);
            };

            $scope.scopeModel.validateCorrectedRate = function (dataItem) {
                if (!isCorrectedRateSet(dataItem))
                    return null;
                var correctedRate = parseFloat(dataItem.correctedRate);
                if (correctedRate <= 0)
                    return 'Fixed rate must be greater than zero';
                if (dataItem.Entity.CurrentRate != null && dataItem.Entity.CurrentRate == correctedRate)
                    return 'Fixed rate must be different than the current one';
                return null;
            };
            $scope.scopeModel.calculateCorrectedRateBED = function (dataItem) {

                if (dataItem.correctedRate == undefined || dataItem.correctedRate == null || dataItem.correctedRate == '') {
                    dataItem.correctedRateBED = undefined;
                }
                else if (rateBulkActionBED != undefined) {
                    dataItem.correctedRateBED = rateBulkActionBED;
                }
                else {
                    var zoneBED = UtilsService.createDateFromString(dataItem.Entity.ZoneBED);
                    var countryBED = (dataItem.Entity.CountryBED != null) ? UtilsService.createDateFromString(dataItem.Entity.CountryBED) : undefined;
                    var correctedRate = parseFloat(dataItem.correctedRate);
                    dataItem.correctedRateBED =
                        WhS_Sales_RatePlanUtilsService.getNewRateBED(zoneBED, countryBED, dataItem.Entity.IsCountryNew, dataItem.Entity.CurrentRate, correctedRate, newRateDayOffset, increasedRateDayOffset, decreasedRateDayOffset);
                }
            };

            defineAPI();
        }
        function defineAPI() {

            var api = {};

            api.load = function (payload) {
                var bulkActionValidationResult;

                if (payload != undefined) {
                    bulkActionValidationResult = payload.bulkActionValidationResult;
                    rateBulkAction = payload.bulkAction;
                    pricingSettings = payload.pricingSettings;
                    $scope.scopeModel.longPrecision = payload.longPrecision;
                }

                if (bulkActionValidationResult != undefined) {
                    emptyRates = bulkActionValidationResult.EmptyRates;
                    zeroRates = bulkActionValidationResult.ZeroRates;
                    negativeRates = bulkActionValidationResult.NegativeRates;
                    duplicateRates = bulkActionValidationResult.DuplicateRates;
                }

                var gridReadyPromises = [];

                if (emptyRates != undefined && emptyRates.length > 0) {
                    $scope.scopeModel.showEmptyRateGrid = true;
                    gridReadyPromises.push(emptyRateGridReadyDeferred.promise);
                    loadMoreGridData($scope.scopeModel.emptyRates, emptyRates);
                }

                if (zeroRates != undefined && zeroRates.length > 0) {
                    $scope.scopeModel.showZeroRateGrid = true;
                    gridReadyPromises.push(zeroRateGridReadyDeferred.promise);
                    loadMoreGridData($scope.scopeModel.zeroRates, zeroRates);
                }

                if (negativeRates != undefined && negativeRates.length > 0) {
                    $scope.scopeModel.showNegativeRateGrid = true;
                    gridReadyPromises.push(negativeRateGridReadyDeferred.promise);
                    loadMoreGridData($scope.scopeModel.negativeRates, negativeRates);
                }

                if (duplicateRates != undefined && duplicateRates.length > 0) {
                    $scope.scopeModel.showDuplicateRateGrid = true;
                    gridReadyPromises.push(duplicateRateGridReadyDeferred.promise);
                    loadMoreGridData($scope.scopeModel.duplicateRates, duplicateRates);
                }

                setRateBulkActionBED();
                setDayOffsets();

                function setRateBulkActionBED() {
                    if (rateBulkAction != undefined && rateBulkAction.BED != null)
                        rateBulkActionBED = UtilsService.createDateFromString(rateBulkAction.BED);
                }
                function setDayOffsets() {
                    if (pricingSettings != undefined) {
                        newRateDayOffset = (pricingSettings.NewRateDayOffset != null) ? pricingSettings.NewRateDayOffset : 0;
                        increasedRateDayOffset = (pricingSettings.IncreasedRateDayOffset != null) ? pricingSettings.IncreasedRateDayOffset : 0;
                        decreasedRateDayOffset = (pricingSettings.DecreasedRateDayOffset != null) ? pricingSettings.DecreasedRateDayOffset : 0;
                    }
                }

                return UtilsService.waitMultiplePromises(gridReadyPromises);
            };

            api.getData = function () {
                var zoneCorrectedRates = [];
                addZoneCorrectedRates(zoneCorrectedRates, $scope.scopeModel.emptyRates);
                addZoneCorrectedRates(zoneCorrectedRates, $scope.scopeModel.zeroRates);
                addZoneCorrectedRates(zoneCorrectedRates, $scope.scopeModel.negativeRates);
                addZoneCorrectedRates(zoneCorrectedRates, $scope.scopeModel.duplicateRates);
                return {
                    $type: 'TOne.WhS.Sales.MainExtensions.RateBulkActionCorrectedData, TOne.WhS.Sales.MainExtensions',
                    ZoneCorrectedRates: zoneCorrectedRates
                };
            };

            if (ctrl.onReady != null) {
                ctrl.onReady(api);
            }
        }

        function loadMoreGridData(gridArray, sourceArray) {
            if (sourceArray == undefined)
                return;
            if (gridArray.length < sourceArray.length) {
                for (var i = 0; i < sourceArray.length && i < pageSize; i++) {
                    gridArray.push({
                        Entity: sourceArray[i]
                    });
                }
            }
        }
        function isCorrectedRateSet(dataItem) {
            return (dataItem.correctedRate !== undefined && dataItem.correctedRate !== null && dataItem.correctedRate !== '');
        }
        function addZoneCorrectedRates(zoneCorrectedRates, sourceArray) {
            for (var i = 0; i < sourceArray.length; i++) {
                var arrayElement = sourceArray[i];
                if (isCorrectedRateSet(arrayElement)) {
                    zoneCorrectedRates.push({
                        ZoneId: arrayElement.Entity.ZoneId,
                        CorrectedRate: arrayElement.correctedRate,
                        CorrectedRateBED: arrayElement.correctedRateBED
                    });
                }
            }
        }
    }
}]);