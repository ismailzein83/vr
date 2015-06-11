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

            var dataGridObj = new DataGrid(ctrl, $scope);
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
      + '<a ng-if="$parent.ctrl.isColumnClickable(colDef, dataItem)" class="span-summary" ng-click="$parent.ctrl.onColumnClicked(colDef, dataItem)" style="cursor:pointer;"> {{::$parent.ctrl.getColumnValue(colDef, dataItem) #CELLFILTER#}}</a>'
      + '<span ng-if="(!$parent.ctrl.isColumnClickable(colDef, dataItem))" class="span-summary"> {{::$parent.ctrl.getColumnValue(colDef, dataItem) #CELLFILTER#}}</span>'
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


    function DataGrid(ctrl, scope) {

        var gridApi = {};
        var maxHeight;
        var lastSortColumnDef;
        var actionMenuWidth;
        var expandableColumnWidth = 0;
        var expandableRowTemplate;
        var actionTypeColumn;
        var stopPagingOnScroll;
        var pagingOnScrollEnabled;

        function addColumn(col, columnIndex) {
            var colDef = {
                name: col.headerText != undefined ? col.headerText : col.field,
                headerCellTemplate: headerTemplate,//'/Client/Templates/Grid/HeaderTemplate.html',//template,
                field: col.field,
                enableSorting: col.enableSorting != undefined ? col.enableSorting : false,
                onSort: function () {
                    if (col.onSortChanged != undefined) {
                        var sortDirection = colDef.sortDirection != "ASC" ? "ASC" : "DESC";
                        var promise = col.onSortChanged(colDef, sortDirection);//this function should return a promise in case it is getting data
                        if (promise != undefined && promise != null)
                            promise.then(function () {
                                if (lastSortColumnDef != undefined)
                                    lastSortColumnDef.sortDirection = undefined;
                                colDef.sortDirection = sortDirection;
                                lastSortColumnDef = colDef;
                            });
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
                    if (col.type == "Datetime")
                        columnCellTemplate = UtilsService.replaceAll(columnCellTemplate, "#CELLFILTER#", "| date:'yyyy-MM-dd HH:mm'");
                    if (col.type == "Date")
                        columnCellTemplate = UtilsService.replaceAll(columnCellTemplate, "#CELLFILTER#", "| date:'yyyy-MM-dd'");
                    else
                        columnCellTemplate = UtilsService.replaceAll(columnCellTemplate, "#CELLFILTER#", "");
                }
            }
            colDef.cellTemplate = columnCellTemplate;
            if (col.isClickable != undefined) {
                colDef.isClickableAttr = col.isClickable;
                colDef.onClickedAttr = col.onClicked;
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
            buildRowHtml();
            return colDef;
        }

        function removeColumn(colDef) {
            var index = ctrl.columnDefs.indexOf(colDef);
            ctrl.columnDefs.splice(index, 1);
            calculateColumnsWidth();
            buildRowHtml();
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

        function initializeController(isSubgrid) {
            ctrl.updateItems = [];
            ctrl.columnDefs = [];

            ctrl.gridStyle = {};
            if (ctrl.maxheight != undefined) {
                setMaxHeight(ctrl.maxheight);
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

            ctrl.getColumnValue = function (colDef, dataItem) {
                return eval('dataItem.' + colDef.field);
            };

            ctrl.isColumnClickable = function (colDef, dataItem) {
                if (typeof (colDef.isClickableAttr) == 'function')
                    return colDef.isClickableAttr(dataItem);
                else
                    return colDef.isClickableAttr;
            };
            ctrl.onColumnClicked = function (colDef, dataItem) {
                if (colDef.onClickedAttr != undefined)
                    colDef.onClickedAttr(dataItem, colDef);
            };
            
        }
        
        function buildRowHtml() {
            ctrl.rowHtml = '';
            for (var i = 0; i < ctrl.columnDefs.length; i++) {
                var currentColumn = ctrl.columnDefs[i];
                var currentColumnHtml = '$parent.ctrl.columnDefs[' + i + ']';
                ctrl.rowHtml += '<div ng-if="!' + currentColumnHtml + '.isHidden" style="width: ' + currentColumn.width + '; display:inline-block' + (i != 0 ? (';border-left: ' + currentColumn.borderRight) : '') +'">'
                +'<div class="vr-datagrid-cell">'
                +'    <div class="vr-datagrid-celltext">'
                  + UtilsService.replaceAll(ctrl.columnDefs[i].cellTemplate, "colDef", currentColumnHtml)
                    +'</div>'
                +'</div>'

            +'</div>'
            }
        }
        
        function defineAPI() {
            gridApi.resetSorting = function () {
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

            gridApi.getPageInfo = getPageInfo;

            gridApi.clearDataAndContinuePaging = function () {
                ctrl.datasource.length = 0;
                stopPagingOnScroll = false;
            };

            gridApi.addItemsToSource = function (items) {
                addBatchItemsToSource(items);
            }

            function addBatchItemsToSource(items) {
                var numberOfItems = pagingOnScrollEnabled ? getPageSize() : 10;//if paging on scroll is enabled, take the page size
                for (var i = 0; i < numberOfItems; i++)
                {
                    if(items.length > 0) {
                        ctrl.datasource.push(items[0]);
                        items.splice(0, 1);
                    }
                }
                
                if (items.length > 0) {                    
                    setTimeout(function () {
                        addBatchItemsToSource(items);
                        scope.$apply(function () {
                           
                        });

                    }, 10);
                }
            }

            if (ctrl.onReady != null)
                ctrl.onReady(gridApi);
        }

        function setMaxHeight(maxHeight) {
            ctrl.gridStyle['max-height'] = maxHeight;
            ctrl.gridStyle['overflow-y'] = "auto";
            ctrl.gridStyle['overflow-x'] = "hidden";
        }

        function definePagingOnScroll($scope, loadMoreDataFunction) {
            ctrl.isLoadingMoreData = false;

            if (loadMoreDataFunction != undefined) {
                pagingOnScrollEnabled = true;
                setMaxHeight("500px");
            }

            //this event is called by the vrDatagridrows directive
            ctrl.onScrolling = function () {
                if (loadMoreDataFunction == undefined)
                    return;
                if (ctrl.isLoadingMoreData)
                    return;
                if (stopPagingOnScroll)
                    return;
                $scope.$apply(function () {
                    var initialLength = ctrl.datasource.length;
                    var promise = loadMoreDataFunction();//this function should return a promise in case it is getting data
                    if (promise != undefined && promise != null) {
                        ctrl.isLoadingMoreData = true;
                        promise.finally(function () {
                            if (ctrl.datasource.length - initialLength < getPageSize())
                                stopPagingOnScroll = true;
                            ctrl.isLoadingMoreData = false;
                        });
                    }
                });

            };
        }

        function getPageSize() {
            return 20;
        }

        function getPageInfo() {
            var fromRow = ctrl.datasource.length + 1;
            return {
                fromRow: fromRow,
                toRow: fromRow + getPageSize() - 1
            };
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

