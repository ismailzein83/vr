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

            UtilsService.waitMultiplePromises([emptyRateGridReadyDeferred.promise, zeroRateGridReadyDeferred.promise, negativeRateGridReadyDeferred.promise, duplicateRateGridReadyDeferred.promise]).then(function () {
                defineAPI();
            });
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

                loadMoreGridData($scope.scopeModel.emptyRates, emptyRates);
                loadMoreGridData($scope.scopeModel.zeroRates, zeroRates);
                loadMoreGridData($scope.scopeModel.negativeRates, negativeRates);
                loadMoreGridData($scope.scopeModel.duplicateRates, duplicateRates);
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