﻿'use strict';


app.directive('vrDatagrid', ['UtilsService', '$compile', function (UtilsService, $compile) {

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
            var expandableColumnWidth = 0;
            var expandableRowTemplate;
            ctrl.columnDefs = [];
            ctrl.addColumn = addColumn;
            ctrl.gridStyle = {};
            if (ctrl.maxheight != undefined) {
                ctrl.gridStyle['max-height'] = ctrl.maxheight;
            }

            ctrl.headerStyle = {};
            if ($attrs.issubgrid != undefined)
            {
                ctrl.headerStyle['background-color'] = '#93C572';
            }

            function defineExpandableRow() {
                ctrl.setExpandableRowTemplate = function (template) {
                    expandableRowTemplate = template;
                    expandableColumnWidth = 2;
                    ctrl.expandableColumnWidth = expandableColumnWidth + '%';
                    ctrl.expandableSectionWidth = (100 - expandableColumnWidth) + '%';
                    calculateDataColumnsSectionWidth();
                };                

                ctrl.expandRow = function (rowIndex, dataItem) {
                    dataItem.expandableRowTemplate = expandableRowTemplate;
                    //if (dataItem.expandableRowContext == undefined)
                    //{                        
                    //    var expandableRowDiv = $element.find("divExpandableRowContent_" + rowIndex)
                    //    expandableRowDiv.append(expandableRowTemplate);
                    //    //var divScope = $scope.$root.$new();
                    //    //divScope.dataItem = dataItem;
                    //    $compile($element.contents())($scope);
                    //    dataItem.expandableRowContext = {};
                    //    console.log(dataItem);
                    //}                    
                };

                ctrl.collapseRow = function (rowIndex) {

                };
            }

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
                ctrl.adjustPosition = function (e) {
                    var self = angular.element(e.currentTarget);
                    var selfHeight = $(this).parent().height() + 12;
                    var selfWidth = $(this).parent().width();
                    var selfOffset = $(self).offset();
                    var w = $(window);
                    var selfOffsetRigth = $(document).width() - selfOffset.left - selfWidth;
                    var dropDown = self.parent().find('ul');
                    $(dropDown).css({ position: 'fixed', top: (selfOffset.top - w.scrollTop() ) + selfHeight, left: 'auto' });
                }
                var actionsAttribute = $scope.$parent.$eval($attrs.menuactions);
                ctrl.getMenuActions = function(dataItem)
                {                    
                    return typeof (actionsAttribute) == 'function' ? actionsAttribute(dataItem) : actionsAttribute;
                }
            }

            function calculateDataColumnsSectionWidth() {
                var width = (100 - actionMenuWidth - expandableColumnWidth);
                //if (width < 100)
                //    width -= 1;
                ctrl.dataColumnsSectionWidth = width + '%';
            }

            defineExpandableRow();
            defineMenuColumn();
            calculateDataColumnsSectionWidth();

            function addColumn(col, columnIndex) {
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
                        columnCellTemplate = columnCellTemplate.replace("#TEXTALIGN#", "right;padding-right:2px");
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
                            col.onClicked(dataItem, colDef);
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

                if (columnIndex == undefined)
                    ctrl.columnDefs.push(colDef);
                else
                    ctrl.columnDefs.splice(columnIndex, 0, colDef);
                    calculateColumnsWidth();
                    return colDef;
            }

            ctrl.removeColumn = function (colDef) {
                var index = ctrl.columnDefs.indexOf(colDef);
                ctrl.columnDefs.splice(index, 1);
                calculateColumnsWidth();
            };

            function calculateColumnsWidth() {
                var totalWidthFactors = 0;
                angular.forEach(ctrl.columnDefs, function (col) {
                    totalWidthFactors += col.widthFactor;
                });
                var initialTotalWidth = 100;
                var totalWidth = initialTotalWidth;
                //if (actionMenuWidth > 0)
                //    totalWidth -= actionMenuWidth;
                //if (expandableColumnWidth > 0)
                //    totalWidth -= expandableColumnWidth;
                //if (totalWidth < initialTotalWidth)
                //    totalWidth -= 1;
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

    var cellTemplate = '<div style="text-align: #TEXTALIGN#;width: 100%;overflow: hidden;text-overflow: ellipsis;white-space: nowrap;" >'
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

