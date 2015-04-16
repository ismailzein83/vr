'use strict';


app.directive('vrDatagrid', ['DataGridDirService', '$interval', function (DataGridDirService, $interval) {
    var cellTemplate = '<div class="ui-grid-cell-contents">'
    +'<a ng-show="col.colDef.isClickable(row.entity)" class="span-summary" ng-click="col.colDef.onClicked(row.entity)" style="cursor:pointer;"> {{row.entity[col.field]}}</a>'
    + '<span ng-hide="col.colDef.isClickable(row.entity)" class="span-summary"> {{row.entity[col.field]}}</span>'
+ '</div>';

    var headerTemplate = '<div ng-class="{ \'sortable\': col.colDef.enableSorting }" class="header-custom" ng-click="col.colDef.onSort()" style="background-color: #829EBF;color:#FFF">'
   +' <div class="ui-grid-cell-contents" col-index="renderIndex">'
     +'   <span>'
       +'         <span ng-show="col.colDef.sortDirection==\'ASC\'">&uarr;</span>'
        +'        <span ng-show="col.colDef.sortDirection==\'DESC\'">&darr;</span>'
    +'{{col.name}}'
   +' </span>'
+'</div>'
+'<div class="ui-grid-column-menu-button" ng-if="grid.options.enableColumnMenus && !col.isRowHeader  && col.colDef.enableColumnMenu !== false" class="ui-grid-column-menu-button" ng-click="toggleMenu($event)">'
+'    <i class="ui-grid-icon-angle-down">&nbsp;</i>'
+'</div>'
+'</div>';
    var directiveDefinitionObject = {

        restrict: 'E',
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element) {
            var ctrl = this;
            var gridApi = {};
            var maxHeight;
            var lastSortColumnDef;
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
                    gridOptions.enableVerticalScrollbar = 1;
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
                        headerCellTemplate: headerTemplate,//'/Client/Templates/Grid/HeaderTemplate.html',//template,
                        enableColumnMenu: false,
                        //enableHiding: false,
                        field: col.field,
                        enableSorting: col.enableSorting != undefined ? col.enableSorting : false,
                        onSort: function () {
                            if (col.onSortChanged != undefined) {
                                var sortDirection = colDef.sortDirection != "ASC" ? "ASC" : "DESC";
                                var sortChangedHandle = {
                                    operationDone: function (isSucceeded) {
                                        if (isSucceeded == true) {
                                            if (lastSortColumnDef != undefined)
                                                lastSortColumnDef.sortDirection = undefined;
                                            colDef.sortDirection = sortDirection;
                                            lastSortColumnDef = colDef;
                                        }
                                    }
                                };
                                col.onSortChanged(sortDirection, sortChangedHandle);
                            }
                        }
                    };
                    
                    if (col.isClickable != undefined) {
                        colDef.cellTemplate = cellTemplate;//'/Client/Templates/Grid/CellTemplate.html';
                        colDef.isClickable = function (dataItem) {
                            return col.isClickable(dataItem);
                        };
                        colDef.onClicked = function (dataItem) {
                            if (col.onClicked != undefined)
                                col.onClicked(dataItem);
                        };
                    }
                    if (col.type == "Number")
                        colDef.cellFilter = "number:2";
                    colDef.testModel = 'test gg';
                    gridOptions.columnDefs.push(colDef);
                });
                maxHeight = options.maxHeight;
                if (maxHeight == undefined)
                    maxHeight = 500;


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
                if (height > maxHeight)
                    height = maxHeight;
                gridOptions.lastHeight = height;

                return {
                    height: height + "px"
                };
            };


            gridApi.defineGrid = defineGrid;
            gridApi.reset = function () {
                if (lastSortColumnDef != undefined) {
                    lastSortColumnDef.sortDirection = undefined;
                    lastSortColumnDef = undefined;
                }
            }
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