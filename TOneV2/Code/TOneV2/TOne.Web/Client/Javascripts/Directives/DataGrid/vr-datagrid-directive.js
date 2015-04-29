'use strict';


app.directive('vrDatagrid', ['UtilsService', function (UtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            datasource: '=',
            onReady: '=',
            maxheight: '@',
            hideheader: '='
        },
        controller: function ($scope, $element) {
            var ctrl = this;
            var gridApi = {};
            var maxHeight;
            var lastSortColumnDef;
            ctrl.columnDefs = [];
            ctrl.addColumn = addColumn;
            ctrl.gridStyle = {};

            if (ctrl.maxheight != undefined) {
                ctrl.gridStyle['max-height'] = ctrl.maxheight;
            }

            function addColumn(col) {
                var colDef = {
                    name: col.headerText != undefined ? col.headerText : col.field,
                    headerCellTemplate: headerTemplate,//'/Client/Templates/Grid/HeaderTemplate.html',//template,
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
                            col.onSortChanged(colDef, sortDirection, sortChangedHandle);
                        }
                    },
                    tag: col.tag
                };

                colDef.getValue = function (dataItem) {
                    return eval('dataItem.' + colDef.field);
                };

                var columnCellTemplate;
                if (col.cellTemplate != undefined && col.cellTemplate != null && col.cellTemplate != "") {                    
                    columnCellTemplate = col.cellTemplate;
                }
                else {
                    columnCellTemplate = cellTemplate;
                    if (col.type == "Number") {
                        columnCellTemplate = columnCellTemplate.replace("#TEXTALIGN#", "right;margin-right:10px");
                        columnCellTemplate = UtilsService.replaceAll(columnCellTemplate, "#CELLFILTER#", "| number:2");
                    }
                    else {
                        columnCellTemplate = columnCellTemplate.replace("#TEXTALIGN#", "left");
                        columnCellTemplate = UtilsService.replaceAll(columnCellTemplate, "#CELLFILTER#", "");
                    }
                }
                colDef.cellTemplate = columnCellTemplate;
                if (col.isClickable != undefined) {
                    colDef.isClickable = function (dataItem) {
                        if (typeof (col.isClickable) == 'function')
                            return col.isClickable(dataItem);
                        else
                            return col.isClickable;
                    };
                    colDef.onClicked = function (dataItem) {
                        if (col.onClicked != undefined)
                            col.onClicked(dataItem);
                    };
                }

                ctrl.columnDefs.push(colDef);
                calculateColumnsWidth();
            }

            function calculateColumnsWidth() {
                angular.forEach(ctrl.columnDefs, function (col) {

                    col.width = (100 / ctrl.columnDefs.length) + '%';
                });
            }

            gridApi.reset = function () {
                if (lastSortColumnDef != undefined) {
                    lastSortColumnDef.sortDirection = undefined;
                    lastSortColumnDef = undefined;
                }
            }
            if (ctrl.onReady != null)
                ctrl.onReady(gridApi);

        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {
            element.append('<vr-datagridrows></vr-datagridrows>');  
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {                    
                }
            }
        }

    };

    var cellTemplate = '<div class="ui-grid-cell-contents" style="text-align: #TEXTALIGN#">'
      + '<a ng-show="colDef.isClickable(dataItem)" class="span-summary" ng-click="colDef.onClicked(dataItem)" style="cursor:pointer;"> {{colDef.getValue(dataItem) #CELLFILTER#}}</a>'
      + '<span ng-hide="colDef.isClickable(dataItem)" class="span-summary"> {{colDef.getValue(dataItem) #CELLFILTER#}}</span>'
   + '</div>';

    var headerTemplate = '<div class="header-custom" ng-click="colDef.onSort()" style="background-color: #829EBF;color:#FFF">'
   + ' <div class="ui-grid-cell-contents" col-index="renderIndex">'
     + '   <span>'
       + '         <span ng-show="colDef.sortDirection==\'ASC\'">&uarr;</span>'
        + '        <span ng-show="colDef.sortDirection==\'DESC\'">&darr;</span>'
    + '{{colDef.name}}'
   + ' </span>'
+ '</div>'
+ '</div>';

    return directiveDefinitionObject;

   

}]);

