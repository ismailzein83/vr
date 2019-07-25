'use strict';

app.directive('vrWhsSalesBulkactionValidationresultCustomertargetmatchimport', ['WhS_Sales_RatePlanUtilsService', 'UtilsService', 'VRUIUtilsService', '$filter', 'LabelColorsEnum', 'WhS_Sales_CustomerTargetMatchImportedRowStatus',
    function (WhS_Sales_RatePlanUtilsService, UtilsService, VRUIUtilsService, $filter, LabelColorsEnum, WhS_Sales_CustomerTargetMatchImportedRowStatus) {
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
            templateUrl: '/Client/Modules/WhS_Sales/Directives/Extensions/BulkAction/ValidationResult/Template/CustomerTargetMatchImportValidationResultTemplate.html'
        };

        function TargetMatchValidationResult($scope, ctrl, $attrs) {

            this.initializeController = initializeController;
            var pageSize = 10;
            var invalidImportedRows;


            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.hasIncludableZones = false;
                $scope.scopeModel.invalidImportedRows = [];

                $scope.scopeModel.onGridReady = function (api) {
                    defineAPI();
                };

                $scope.scopeModel.loadMoreGridData = function () {
                    loadMoreGridData($scope.scopeModel.invalidImportedRows, invalidImportedRows);
                };

                $scope.scopeModel.toggleAllIncludableZonesSelection = function (value) {
                    if (invalidImportedRows == undefined)
                        return;
                    for (var i = 0; i < invalidImportedRows.length && i < invalidImportedRows.length; i++) {
                        if (invalidImportedRows[i].CanBeIncluded)
                            invalidImportedRows[i].Include = value;
                    }
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
                    checkIfThereIsIncludableZones();
                    loadMoreGridData($scope.scopeModel.invalidImportedRows, invalidImportedRows);
                };

                api.getData = function () {

                    return {
                        $type: 'TOne.WhS.Sales.MainExtensions.CustomerTargetMatchBulkActionCorrectedData, TOne.WhS.Sales.MainExtensions',
                        IncludedZones: getIncludedZoneRates()
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
                        sourceArray[i].CanBeIncluded = sourceArray[i].Status == WhS_Sales_CustomerTargetMatchImportedRowStatus.InvalidDueExpectedRateViolation.value;
                        gridArray.push({
                            Entity: sourceArray[i]
                        });
                    }
                }
            }

            function checkIfThereIsIncludableZones() {
                if (invalidImportedRows == undefined)
                    return false;
                for (var i = 0; i < invalidImportedRows.length; i++) {
                    if (invalidImportedRows[i].Status == WhS_Sales_CustomerTargetMatchImportedRowStatus.InvalidDueExpectedRateViolation.value) {
                        $scope.scopeModel.hasIncludableZones = true;
                        break;
                    }
                }
            }

            function getIncludedZoneRates() {
                var includedZones = [];
                for (var i = 0; i < invalidImportedRows.length; i++) {
                    var element = invalidImportedRows[i];
                    if (element.Include) {
                        includedZones.push({
                            ZoneId: element.ZoneId,
                            ZoneName: element.ImportedRow.Zone,
                            Rate: element.ImportedRow.Rate,
                            TargetVolume: element.ImportedRow.TargetVolume
                        });
                    }
                }
                return includedZones;
            }

        }
    }]);