(function (app) {

    'use strict';

    fieldmappingCellfieldmappingDirective.$inject = ['UtilsService', 'VRUIUtilsService'];

    function fieldmappingCellfieldmappingDirective(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                isrequired: '=',
                customvalidate: '=',
                type:'='
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
                ctrl.updateRange = function ()
                {
                    if (context != undefined) {
                        var range = context.getSelectedCell();
                        if (range != undefined)
                        {
                            $scope.cellObject = {
                                row: range[0],
                                col: range[1],
                                sheet: context.getSelectedSheet(),
                            }
                        }
                        
                    }

                }
                ctrl.selectCell = function ()
                {
                    if (context != undefined && $scope.cellObject!=undefined)
                    {
                        context.setSelectedCell($scope.cellObject.row, $scope.cellObject.col);
                    }
                }
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        context = payload.context;
                    } 

                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var data
                    if ($scope.cellObject != undefined)
                    {
                        data = {
                            $type: "ExcelConversion.MainExtensions.FieldMappings.CellFieldMapping, ExcelConversion.MainExtensions",
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

    app.directive('excelconversionFieldmappingCellfieldmapping', fieldmappingCellfieldmappingDirective);

})(app);