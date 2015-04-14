'use strict';


app.directive('vrDatagrid', ['DataGridDirService', '$interval', function (DataGridDirService, $interval) {

    var directiveDefinitionObject = {

        restrict: 'E',
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element) {
            var ctrl = this;
            var gridApi = {};
            ctrl.gridOptions = { data: [], columnDefs: [] };


            function defineGrid(options) {
                var gridOptions = ctrl.gridOptions;

                gridOptions.onRegisterApi = function (api) {
                    gridApi = api;

                    // call resize every 200 ms for 2 s after modal finishes opening - usually only necessary on a bootstrap modal
                    $interval(function () {
                        gridApi.core.handleWindowResize();
                    }, 10, 500);
                }

                gridOptions.useExternalSorting = true;
                gridOptions.enableGridMenu = true;
                gridOptions.enableHorizontalScrollbar = 0;
                gridOptions.enableVerticalScrollbar = 2;
                //gridOptions.minRowsToShow = 30;
                //gridOptions.enableFiltering = false;
                //gridOptions.saveFocus = false;
                //gridOptions.saveScroll = true;
                gridOptions.enableColumnResizing = true;

                angular.forEach(options.columns, function (col) {
                    var colDef = {
                        name: col.headerText != undefined ? col.headerText : col.field,
                        headerCellTemplate: '/Client/Templates/Grid/HeaderTemplate.html',//template,
                        enableColumnMenu: false,
                        //enableHiding: false,
                        field: col.field
                    };
                    if (col.type == "Number")
                        colDef.cellFilter = "number:2";
                    gridOptions.columnDefs.push(colDef);
                });

                gridApi.data = gridOptions.data;
            }

            ctrl.getGridHeight = function (gridOptions) {
                return "400px";
            };


            gridApi.defineGrid = defineGrid;
            ctrl.onReady(gridApi);

        },
        controllerAs: 'ctrl',
        bindToController: true,
        //compile: function (element, attrs) {

        //    return {
        //        pre: function ($scope, iElem, iAttrs, ctrl) {
        //            $scope.gridOptions = { data: [], columnDefs: [] };

        //        }
        //    }
        //},
        //link: function ($scope, $element, $attrs, $tabsCtrl) {
        //    $scope.gridOptions = { columnDefs: [], data: []  };

        //},
        templateUrl: function (element, attrs) {
            return DataGridDirService.dTemplate;
        }

    };

    return directiveDefinitionObject;

}]);