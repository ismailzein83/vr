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
                if (options.hideHeader) {
                    gridOptions.showHeader = false;
                }
                else
                {
                    gridOptions.enableGridMenu = true;
                }
                gridOptions.useExternalSorting = true;
                
                gridOptions.enableHorizontalScrollbar = 0;
                if (options.showVerticalScroll)
                    gridOptions.enableVerticalScrollbar = 2;
                else
                    gridOptions.enableVerticalScrollbar = 0;
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
                    if (col.isClickable != undefined) {
                        colDef.cellTemplate = '/Client/Templates/Grid/CellTemplate.html';
                        colDef.isClickable = function (dataItem) {
                            return col.isClickable(dataItem);
                        };
                    }
                    if (col.type == "Number")
                        colDef.cellFilter = "number:2";
                    colDef.testModel = 'test gg';
                    gridOptions.columnDefs.push(colDef);
                });

                gridApi.data = gridOptions.data;
            }
            ctrl.getGridHeight = function (gridOptions) {
                var height;
                if (gridOptions.data.length == 0) {
                    height = gridOptions.lastHeight;
                }
                else {
                    var rowHeight = 30; // your row height
                    var headerHeight = 30; // your header height
                    var height = (gridOptions.data.length * rowHeight + headerHeight);
                }
                gridOptions.lastHeight = height;

                return {
                    height: height + "px"
                };
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