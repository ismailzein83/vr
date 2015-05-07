'use strict';


app.directive('vrDatagrid', ['UtilsService', function (UtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            datasource: '=',
            onReady: '=',
            maxheight: '@',
            hideheader: '=',
            noverticallines: '@'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var gridApi = {};
            var maxHeight;
            var lastSortColumnDef;
            var actionMenuWidth;
            ctrl.columnDefs = [];
            ctrl.addColumn = addColumn;
            ctrl.gridStyle = {};
            if (ctrl.maxheight != undefined) {
                ctrl.gridStyle['max-height'] = ctrl.maxheight;
            }
            
            defineMenuColumn();
            function defineMenuColumn() {
                var hasActionMenu = $attrs.menuactions != undefined;
                actionMenuWidth = hasActionMenu ? 3 : 0;
                ctrl.actionsColumnWidth = actionMenuWidth + '%';

                ctrl.isActionMenuVisible = function (dataItem) {
                    if (!hasActionMenu)
                        return false;
                    var actions = ctrl.getMenuActions(dataItem);
                    return actions != undefined && actions != null && actions.length > 0;
                }

                var actionsAttribute = $scope.$parent.$eval($attrs.menuactions);
                ctrl.getMenuActions = function(dataItem)
                {                    
                    return typeof (actionsAttribute) == 'function' ? actionsAttribute(dataItem) : actionsAttribute;
                }
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
                        columnCellTemplate = columnCellTemplate.replace("#TEXTALIGN#", "right;margin-right:5px");
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
                if (ctrl.noverticallines == "true")
                    colDef.borderRight = 'none';
                else
                    colDef.borderRight = '1px solid #D0D0D0';
               
                if (col.widthFactor == undefined)
                    colDef.widthFactor = 10;
                else
                    colDef.widthFactor = col.widthFactor;

                //if (ctrl.columnDefs.length == 0)
                    ctrl.columnDefs.push(colDef);
                //else
                //    ctrl.columnDefs.splice(ctrl.columnDefs.length - 1, 0, colDef);
                calculateColumnsWidth();
            }

            function calculateColumnsWidth() {
                var totalWidthFactors = 0;
                angular.forEach(ctrl.columnDefs, function (col) {
                    totalWidthFactors += col.widthFactor;
                });
                var totalWidth = 100;
                if (actionMenuWidth > 0)
                    totalWidth = 100 - actionMenuWidth - 1;
                angular.forEach(ctrl.columnDefs, function (col) {

                    col.width = (totalWidth * col.widthFactor / totalWidthFactors) + '%';
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
                    var ctrl = $scope.ctrl;
                    if (iAttrs.pagersettings != undefined) {
                        ctrl.pagerSettings = $scope.$parent.$eval(iAttrs.pagersettings);
                        ctrl.showPager = true;
                    }
                }
            }
        }

    };

    var cellTemplate = '<div style="text-align: #TEXTALIGN#">'
        + ''
      + '<a ng-show="colDef.isClickable(dataItem)" class="span-summary" ng-click="colDef.onClicked(dataItem)" style="cursor:pointer;"> {{colDef.getValue(dataItem) #CELLFILTER#}}</a>'
      + '<span ng-hide="colDef.isClickable(dataItem)" class="span-summary"> {{colDef.getValue(dataItem) #CELLFILTER#}}</span>'
      + ''
   + '</div>';

    var headerTemplate = '<div ng-click="colDef.onSort()" class="vr-datagrid-header-cell" >'
   + ' <div col-index="renderIndex">'
     + '   <div class="vr-datagrid-celltext" >'
       + '         <span ng-show="colDef.sortDirection==\'ASC\'">&uarr;</span>'
        + '        <span ng-show="colDef.sortDirection==\'DESC\'">&darr;</span>'
         + '{{colDef.name}}'
     + ' </div>'
+ '</div>'
+ '</div>';

    return directiveDefinitionObject;

   

}]);

