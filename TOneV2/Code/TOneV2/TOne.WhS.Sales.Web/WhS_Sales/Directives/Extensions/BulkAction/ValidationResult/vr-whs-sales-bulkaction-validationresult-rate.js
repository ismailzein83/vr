'use strict';

app.directive('vrWhsSalesBulkactionValidationresultRate', ['UtilsService', 'VRUIUtilsService', function (UtilsService, VRUIUtilsService) {
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

            defineAPI();
        }
        function defineAPI() {

            var api = {};

            api.load = function (payload) {
                var bulkActionValidationResult;

                if (payload != undefined) {
                    bulkActionValidationResult = payload.bulkActionValidationResult;
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

                return UtilsService.waitMultiplePromises(gridReadyPromises);
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
    }
}]);