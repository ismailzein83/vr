'use strict';


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

            var isSubGrid = $attrs.issubgrid != undefined;
            var loadMoreDataFunction;
            if ($attrs.loadmoredata != undefined)
                loadMoreDataFunction = $scope.$parent.$eval($attrs.loadmoredata);
            var hasActionMenu = $attrs.menuactions != undefined;
            var actionsAttribute = hasActionMenu ? $scope.$parent.$eval($attrs.menuactions) : undefined;

            var dataGridObj = new DataGrid(ctrl);
            dataGridObj.initializeController(isSubGrid);
            dataGridObj.definePagingOnScroll($scope, loadMoreDataFunction);
            dataGridObj.defineExpandableRow();
            dataGridObj.defineMenuColumn(hasActionMenu, actionsAttribute);
            dataGridObj.calculateDataColumnsSectionWidth();           
            dataGridObj.addActionTypeColumn();
            dataGridObj.defineAPI();
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


    function DataGrid(ctrl) {

        var gridApi = {};
        var maxHeight;
        var lastSortColumnDef;
        var actionMenuWidth;
        var expandableColumnWidth = 0;
        var expandableRowTemplate;
        var actionTypeColumn;

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
                ctrl.columnDefs.splice(ctrl.columnDefs.length - 1, 0, colDef);//to insert before the actionType column
            else
                ctrl.columnDefs.splice(columnIndex, 0, colDef);
            calculateColumnsWidth();
            return colDef;
        }

        function removeColumn(colDef) {
            var index = ctrl.columnDefs.indexOf(colDef);
            ctrl.columnDefs.splice(index, 1);
            calculateColumnsWidth();
        }

        function hideColumn(colDef) {
            colDef.isHidden = true;
            calculateColumnsWidth();
        }

        function showColumn(colDef) {
            colDef.isHidden = false;
            calculateColumnsWidth();
        }

        function calculateColumnsWidth() {
            var totalWidthFactors = 0;
            angular.forEach(ctrl.columnDefs, function (col) {
                if (col.isHidden != true)
                    totalWidthFactors += col.widthFactor;
            });
            var initialTotalWidth = 100;
            var totalWidth = initialTotalWidth;
            angular.forEach(ctrl.columnDefs, function (col) {
                if (col.isHidden != true)
                    col.width = (totalWidth * col.widthFactor / totalWidthFactors) + '%';
            });
        }

        function itemChanged(item, actionType) {
            item.actionType = actionType;
            ctrl.updateItems.splice(0, 0, item);
        }

        function calculateDataColumnsSectionWidth() {
            var width = (100 - actionMenuWidth - expandableColumnWidth);
            //if (width < 100)
            //    width -= 1;
            ctrl.dataColumnsSectionWidth = width + '%';
        }

        function initializeController (isSubgrid) {
            ctrl.updateItems = [];
            ctrl.columnDefs = [];

            ctrl.gridStyle = {};
            if (ctrl.maxheight != undefined) {
                ctrl.gridStyle['max-height'] = ctrl.maxheight;
                ctrl.gridStyle['overflow-y'] = "auto";
                ctrl.gridStyle['overflow-x'] = "hidden";
            }

            ctrl.headerStyle = {};
            if (isSubgrid) {
                ctrl.headerStyle['background-color'] = '#93C572';
            }

            ctrl.isMainItemsShown = true;
            ctrl.viewVisibilityChanged = function () {
                if (!ctrl.isMainItemsShown) {
                    ctrl.showColumn(actionTypeColumn);
                }
                else {
                    ctrl.hideColumn(actionTypeColumn);
                }
            };


            ctrl.addColumn = addColumn;
            ctrl.removeColumn = removeColumn;
            ctrl.hideColumn = hideColumn;
            ctrl.showColumn = showColumn;
        }

        function defineAPI() {
            gridApi.reset = function () {
                if (lastSortColumnDef != undefined) {
                    lastSortColumnDef.sortDirection = undefined;
                    lastSortColumnDef = undefined;
                }
            }

            gridApi.itemAdded = function (item) {
                itemChanged(item, "Added");
            };

            gridApi.itemUpdated = function (item) {
                itemChanged(item, "Updated");
            };

            gridApi.itemDeleted = function (item) {
                itemChanged(item, "Deleted");
            };

            if (ctrl.onReady != null)
                ctrl.onReady(gridApi);
        }

        function definePagingOnScroll($scope, loadMoreDataFunction) {
            ctrl.isLoadingMoreData = false;

            //this event is called by the vrDatagridrows directive
            ctrl.onScrolling = function () {
                if (loadMoreDataFunction == undefined)
                    return;
                if (ctrl.isLoadingMoreData)
                    return;
                $scope.$apply(function () {
                    ctrl.isLoadingMoreData = true;
                    var asyncHandle = {
                        operationDone: clearLoadingMoreDataFlag
                    };
                    loadMoreDataFunction(asyncHandle);
                });

            };

            function clearLoadingMoreDataFlag() {
                //$scope.$apply(function () {
                ctrl.isLoadingMoreData = false;
                //});                    
            }
        }

        function defineExpandableRow () {
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

        function defineMenuColumn(hasActionMenu, actionsAttribute) {            
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
                $(dropDown).css({ position: 'fixed', top: (selfOffset.top - w.scrollTop()) + selfHeight, left: 'auto' });
            }           
            ctrl.getMenuActions = function (dataItem) {
                return typeof (actionsAttribute) == 'function' ? actionsAttribute(dataItem) : actionsAttribute;
            }

            ctrl.menuActionClicked = function (action, dataItem) {
                action.clicked(dataItem);
            };
        }

        function addActionTypeColumn() {
            var col = {
                headerText: "Action",
                field: "actionType"
            };
            actionTypeColumn = addColumn(col, ctrl.columnDefs.length);
            ctrl.hideColumn(actionTypeColumn);
        }

        this.initializeController = initializeController;
        this.defineAPI = defineAPI;
        this.definePagingOnScroll = definePagingOnScroll;
        this.defineExpandableRow = defineExpandableRow;   
        this.defineMenuColumn = defineMenuColumn;
        this.calculateDataColumnsSectionWidth = calculateDataColumnsSectionWidth;
        this.addActionTypeColumn = addActionTypeColumn;
    }


    return directiveDefinitionObject;
}]);

