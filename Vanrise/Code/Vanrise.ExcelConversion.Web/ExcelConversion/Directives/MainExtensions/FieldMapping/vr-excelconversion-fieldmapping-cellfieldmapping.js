(function (app) {

    'use strict';

    fieldmappingCellfieldmappingDirective.$inject = [];

    function fieldmappingCellfieldmappingDirective() {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                isrequired: '=',
                type: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var cellfieldmapping = new CellFieldMapping($scope, ctrl, $attrs);
                cellfieldmapping.initializeController();
            },
            controllerAs: "cellfieldmappingCtrl",
            bindToController: true,
            templateUrl: "/Client/Modules/ExcelConversion/Directives/MainExtensions/FieldMapping/Templates/CellFieldMappingTemplate.html"
        };

        function CellFieldMapping($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var context;
            function initializeController() {
                ctrl.updateRange = function () {
                    if (context != undefined) {
                        var range = context.getSelectedCell();
                        if (range != undefined) {
                            $scope.cellObject = {
                                row: range[0],
                                col: range[1],
                                sheet: context.getSelectedSheet(),
                            }
                        }

                    }

                }
                ctrl.validate = function () {
                    if ($scope.cellObject != undefined && context != undefined) {
                        var row = context.getFirstRowIndex();
                        if (row != undefined && $scope.cellObject != undefined && $scope.cellObject.sheet != row.sheet)
                            return "Error sheet index.";
                        if (row == undefined || $scope.cellObject.row != row.row)
                            return "Error row index.";

                    }
                    return null;
                }
                ctrl.selectCell = function () {
                    if (context != undefined && $scope.cellObject != undefined) {
                        context.setSelectedCell($scope.cellObject.row, $scope.cellObject.col, $scope.cellObject.sheet);
                    }
                }
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        context = payload.context;
                        if(payload.cellFieldData !=undefined)
                        {
                            $scope.cellObject = {
                                row: payload.cellFieldData.rowIndex,
                                col: payload.cellFieldData.cellIndex,
                                sheet: payload.cellFieldData.sheetIndex,
                            }
                        }
                    }

                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var data
                    if ($scope.cellObject != undefined) {
                        data = {
                            $type: "Vanrise.ExcelConversion.MainExtensions.FieldMappings.CellFieldMapping, Vanrise.ExcelConversion.MainExtensions",
                            SheetIndex: $scope.cellObject.sheet,
                            RowIndex: $scope.cellObject.row,
                            CellIndex: $scope.cellObject.col
                        };
                    }

                    return data;
                }
            }
        }
    }

    app.directive('vrExcelconversionFieldmappingCellfieldmapping', fieldmappingCellfieldmappingDirective);

})(app);