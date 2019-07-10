'use strict';

app.directive('vrWhsSalesBulkactionValidationresultSuppliertargetmatchimport', ['WhS_Sales_RatePlanUtilsService', 'UtilsService', 'VRUIUtilsService', '$filter', 'LabelColorsEnum', function (WhS_Sales_RatePlanUtilsService, UtilsService, VRUIUtilsService, $filter,LabelColorsEnum) {
    return {
        restrict: "E",
        scope: {
            onReady: "=",
            normalColNum: '@'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var targetMatchValidationResult = new TargetMatchValidationResult($scope, ctrl, $attrs);
            targetMatchValidationResult.initializeController();
        },
        controllerAs: "rateValidationResultCtrl",
        bindToController: true,
        templateUrl: '/Client/Modules/WhS_Sales/Directives/Extensions/BulkAction/ValidationResult/Template/SupplierTargetMatchImportValidationResultTemplate.html'
    };

    function TargetMatchValidationResult($scope, ctrl, $attrs) {

        this.initializeController = initializeController;
        var pageSize = 10;
        var invalidImportedRows;
      

        function initializeController() {
            $scope.scopeModel = {};
            $scope.scopeModel.invalidImportedRows = [];

            $scope.scopeModel.onGridReady = function (api) {
                defineAPI();
            };

            $scope.scopeModel.loadMoreGridData = function () {
                loadMoreGridData($scope.scopeModel.invalidImportedRows, invalidImportedRows);
            };
        }
        function defineAPI() {

            var api = {};

            api.load = function (payload) {

                var bulkActionValidationResult;

                if (payload != undefined) {
                    bulkActionValidationResult = payload.bulkActionValidationResult;
                }

                if (bulkActionValidationResult != undefined) {
                    invalidImportedRows = bulkActionValidationResult.InvalidImportedRows;
                    $scope.scopeModel.errorMessage = bulkActionValidationResult.ErrorMessage;
                    $scope.scopeModel.errorMessageColor = LabelColorsEnum.DangerFont.color;
                }

                loadMoreGridData($scope.scopeModel.invalidImportedRows, invalidImportedRows);
            };

            api.getData = function () {
                return null;
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