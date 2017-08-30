'use strict';

app.directive('vrGenericdataDatarecordtypefieldsFormulaDatetimeround', ['UtilsService', 'VR_GenericData_DateTimeRecordFilterComparisonPartEnum', 'VR_GenericData_TimeRoundingIntervalEnum',
    function (UtilsService, VR_GenericData_DateTimeRecordFilterComparisonPartEnum, VR_GenericData_TimeRoundingIntervalEnum) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new dateTimeRoundCtor(ctrl, $scope);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: function (element, attrs) {
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/DataRecordFieldFormulas/Templates/DateTimeRoundFieldFormulaTemplate.html';
            }
        };

        function dateTimeRoundCtor(ctrl, $scope) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.fields = [];
                $scope.scopeModel.showRoundingIntervalInMinutes = false;

                $scope.scopeModel.onComparisonPartSelectionChanged = function (selectedComparisonPart) {
                    $scope.scopeModel.showRoundingIntervalInMinutes = selectedComparisonPart != undefined && selectedComparisonPart.value == 1;
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    $scope.scopeModel.comparisonParts = UtilsService.getArrayEnum(VR_GenericData_DateTimeRecordFilterComparisonPartEnum);
                    $scope.scopeModel.timeRoundingInterval = UtilsService.getArrayEnum(VR_GenericData_TimeRoundingIntervalEnum);

                    if (payload != undefined) {
                        var context = payload.context;
                        if (context != undefined && context.getFields != undefined) {
                            $scope.scopeModel.fields = context.getFields();
                        }

                        if (payload.formula != undefined) {
                            $scope.scopeModel.selectedDateTimeFieldName = UtilsService.getItemByVal($scope.scopeModel.fields, payload.formula.DateTimeFieldName, "fieldName");
                            $scope.scopeModel.selectedComparisonPart = UtilsService.getItemByVal($scope.scopeModel.comparisonParts, payload.formula.ComparisonPart, "value");
                            $scope.scopeModel.selectedTimeRoundingInterval = UtilsService.getItemByVal($scope.scopeModel.timeRoundingInterval, payload.formula.TimeRoundingInterval, "value");
                        }
                    }
                };

                api.getData = function () {

                    return {
                        $type: "Vanrise.GenericData.MainExtensions.DataRecordFieldFormulas.DateTimeRoundFieldFormula, Vanrise.GenericData.MainExtensions",
                        DateTimeFieldName: $scope.scopeModel.selectedDateTimeFieldName.fieldName,
                        ComparisonPart: $scope.scopeModel.selectedComparisonPart.value,
                        TimeRoundingInterval: $scope.scopeModel.showRoundingIntervalInMinutes ? $scope.scopeModel.selectedTimeRoundingInterval.value : undefined
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

        }

        return directiveDefinitionObject;
    }
]);