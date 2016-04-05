(function (app) {

    'use strict';

    fieldmappingCellfieldmappingDirective.$inject = ['UtilsService', 'VRUIUtilsService'];

    function fieldmappingCellfieldmappingDirective(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                isrequired: '='
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
                        $scope.row = range[0];
                        $scope.col = range[1];
                        $scope.sheetindex = 0; // range[1];
                    }

                }
                ctrl.selectCell = function ()
                {
                    if(context !=undefined)
                    {
                        context.setSelectedCell($scope.row, $scope.col);
                    }
                }
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    console.log(payload);
                    if (payload != undefined) {
                        context = payload.context;
                    }

                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var data;
                    return data;
                }
            }
        }
    }

    app.directive('excelconversionFieldmappingCellfieldmapping', fieldmappingCellfieldmappingDirective);

})(app);