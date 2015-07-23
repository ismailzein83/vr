app.directive('vrDatagrid', ['UtilsService', 'SecurityService', '$compile', function (UtilsService, SecurityService, $compile) {

    'use strict';

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            datasource: '=',
            onReady: '=',
            maxheight: '@',
            hideheader: '=',
            noverticallines: '@',
            idfield: '@',
            onexport:'='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var loadMoreDataFunction;
            if ($attrs.loadmoredata != undefined)
                loadMoreDataFunction = $scope.$parent.$eval($attrs.loadmoredata);
           
            ctrl.clientSideFilterFunction;
            ctrl.rotateHeader = true;

            if ($attrs.clientsidefilter != undefined)
                ctrl.clientSideFilterFunction = $scope.$parent.$eval($attrs.clientsidefilter);
            if ($attrs.rotate == undefined) {
                ctrl.rotateHeader = false;
            }
            else
                ctrl.rotateHeader = $attrs.rotate;
            console.log(ctrl.rotateHeader);
            var hasActionMenu = $attrs.menuactions != undefined;
            var actionsAttribute = hasActionMenu ? $scope.$parent.$eval($attrs.menuactions) : undefined;
            var dataGridObj = new DataGrid(ctrl, $scope);
            dataGridObj.initializeController();
            dataGridObj.definePagingOnScroll($scope, loadMoreDataFunction);
            dataGridObj.defineExpandableRow();
            dataGridObj.defineMenuColumn(hasActionMenu, actionsAttribute);
            dataGridObj.calculateDataColumnsSectionWidth();
            dataGridObj.addActionTypeColumn();
            dataGridObj.defineAPI();
            ctrl.isExporting = false;
            ctrl.onExportClicked = function () {
                if (this.onexport != undefined && typeof (this.onexport) == 'function') {
                    var promise = this.onexport();
               
                    if (promise != undefined && promise != null) {
                        ctrl.isExporting = true;
                        promise.finally(function () {
                            ctrl.isExporting = false;
                        });
                    }
                }
            };
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
      + '<a ng-if="$parent.ctrl.isColumnClickable(colDef, dataItem)"  ng-class="::$parent.ctrl.getCellClass(colDef, dataItem)" ng-click="$parent.ctrl.onColumnClicked(colDef, dataItem)" style="cursor:pointer;"> {{::$parent.ctrl.getColumnValue(colDef, dataItem) #CELLFILTER#}}</a>'
      + '<span ng-if="(!$parent.ctrl.isColumnClickable(colDef, dataItem))" ng-class="::$parent.ctrl.getCellClass(colDef, dataItem)"> {{::$parent.ctrl.getColumnValue(colDef, dataItem) #CELLFILTER#}}</span>'
      + ''
   + '</div>';

    var summaryCellTemplate = '<div style="text-align: #TEXTALIGN#;width: 100%;overflow: hidden;text-overflow: ellipsis;white-space: nowrap;" >'
        + ''
      + '<a ng-if="ctrl.isColumnClickable(colDef, ctrl.summaryDataItem)"  ng-class="::ctrl.getCellClass(colDef, ctrl.summaryDataItem)" style="font-size:13px" ng-click="ctrl.onColumnClicked(colDef, ctrl.summaryDataItem)" style="cursor:pointer;"> {{ctrl.getColumnSummaryValue(colDef, ctrl.summaryDataItem) #CELLFILTER#}}</a>'
      + '<span ng-if="(!ctrl.isColumnClickable(colDef, ctrl.summaryDataItem))" ng-class="::ctrl.getCellClass(colDef, ctrl.summaryDataItem)" style="font-size:13px"> {{ctrl.getColumnSummaryValue(colDef, ctrl.summaryDataItem) #CELLFILTER#}}</span>'
      + ''
   + '</div>';


    var headerTemplate = '<div ng-click="colDef.onSort()" class="vr-datagrid-header-cell" >'
   + ' <div col-index="renderIndex">'
     + '   <div class="vr-datagrid-celltext"  ng-class="colDef.getAlign()" title="{{colDef.name}}" >'
       + '    <span ng-show="colDef.sortDirection==\'ASC\'">&uarr;</span>'
        + '   <span ng-show="colDef.sortDirection==\'DESC\'">&darr;</span>'
              +'<span ng-if="!colDef.rotateHeader"> {{colDef.name}}</span>'
              +'<p ng-if="colDef.rotateHeader" class="vr-rotate-header" >{{colDef.name}}</p>'
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
                summaryField: col.summaryField,
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
                getAlign: function () {
                    var align = '';
                    if (col.type == "Number") 
                        align = 'vr-grid-cell-number';                   
                        return align;

                },
                tag: col.tag,
                getcolor: col.getcolor,
                rotateHeader: ctrl.rotateHeader,
                nonHiddable: col.nonHiddable
            };

            var columnCellTemplate;
            if (col.cellTemplate != undefined && col.cellTemplate != null && col.cellTemplate != "") {
                columnCellTemplate = col.cellTemplate;
            }
            else {
                columnCellTemplate = getCellTemplateWithFilter(cellTemplate, col);
            }
            colDef.cellTemplate = columnCellTemplate;
            colDef.summaryCellTemplate = getCellTemplateWithFilter(summaryCellTemplate, col);

            if (col.isClickable != undefined) {
                colDef.isClickableAttr = col.isClickable;
                colDef.onClickedAttr = col.onClicked;                
            }
            if (ctrl.noverticallines == "true")
                colDef.borderRight = 'none';
            else            
                colDef.borderRight = 'none';

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

        function getCellTemplateWithFilter(template, col) {
            if (col.type == "Number") {
                template = template.replace("#TEXTALIGN#", "right;padding-right:2px");
                template = UtilsService.replaceAll(template, "#CELLFILTER#", "| number:2");
            }
            else {
                template = template.replace("#TEXTALIGN#", "left");
                if (col.type == "Datetime")
                    template = UtilsService.replaceAll(template, "#CELLFILTER#", "| date:'yyyy-MM-dd HH:mm'");
                if (col.type == "Date")
                    template = UtilsService.replaceAll(template, "#CELLFILTER#", "| date:'yyyy-MM-dd'");
                else
                    template = UtilsService.replaceAll(template, "#CELLFILTER#", "");
            }
            return template;
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

        function updateColumnHeader(colDef, headerText){
            colDef.name = headerText;
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
            if (item.isDeleted) {//delete the item from the original data source
                var deletedItemIndex = ctrl.datasource.indexOf(item);
                if (deletedItemIndex < 0 && ctrl.idfield != undefined)//if item is not found by object, try to find it by id
                    deletedItemIndex = UtilsService.getItemIndexByVal(ctrl.datasource, item[ctrl.idfield], ctrl.idfield);
                if (deletedItemIndex >= 0)
                    ctrl.datasource.splice(deletedItemIndex, 1);
            }

            if (ctrl.idfield != undefined) {
                //remove the item if exists in the updatedItems array
                var itemIndexInUpdatedItems = UtilsService.getItemIndexByVal(ctrl.updateItems, item[ctrl.idfield], ctrl.idfield);
                if(itemIndexInUpdatedItems >= 0)
                    ctrl.updateItems.splice(itemIndexInUpdatedItems, 1);

                //update the item in the datasource array if exists
                var itemIndexInDataSource = UtilsService.getItemIndexByVal(ctrl.datasource, item[ctrl.idfield], ctrl.idfield);
                if (itemIndexInDataSource >= 0)
                    ctrl.datasource[itemIndexInDataSource] = item;
            }            
            
            item.actionType = actionType;
            ctrl.updateItems.splice(0, 0, item);
        }

        function calculateDataColumnsSectionWidth() {
            var width = (100 - actionMenuWidth - expandableColumnWidth);
            //if (width < 100)
            //    width -= 1;
            ctrl.dataColumnsSectionWidth = width + '%';
        }

        function initializeController() {
            ctrl.updateItems = [];
            ctrl.columnDefs = [];
            ctrl.gridStyle = {};
            if (ctrl.maxheight != undefined) {
                setMaxHeight(ctrl.maxheight);
            }

            ctrl.headerStyle = {};
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
            ctrl.updateColumnHeader = updateColumnHeader;

            ctrl.getColumnValue = function (colDef, dataItem) {
                if (colDef == undefined || colDef.field == undefined)
                    return null;
                else
                    return eval('dataItem.' + colDef.field);
            };

            ctrl.getColumnSummaryValue = function (colDef, dataItem) {
                if (colDef == undefined || colDef.summaryField == undefined)
                    return null;
                else
                    return eval('dataItem.' + colDef.summaryField);
            };

            ctrl.isColumnClickable = function (colDef, dataItem) {
                if (colDef == undefined || colDef.isClickableAttr == undefined)
                    return false;
                else {
                    if (typeof (colDef.isClickableAttr) == 'function')
                        return colDef.isClickableAttr(dataItem);
                    else
                        return colDef.isClickableAttr;
                }
            };

            ctrl.onColumnClicked = function (colDef, dataItem) {
                if (colDef.onClickedAttr != undefined)
                    colDef.onClickedAttr(dataItem, colDef);
            };

            ctrl.getCellClass = function (colDef, dataItem) {
                if (colDef == undefined) return 'span-summary';
                if (colDef.getcolor != undefined)
                    return colDef.getcolor(dataItem, colDef);
                return 'span-summary';
            };
            
            var lastSelectedRow;
            ctrl.onRowClicked = function (evnt) {
                if (lastSelectedRow != undefined)
                    lastSelectedRow.removeClass('vr-datagrid-datacells-click');

                lastSelectedRow = angular.element(evnt.currentTarget);
                lastSelectedRow.addClass('vr-datagrid-datacells-click');
                ctrl.menuLeft = evnt.clientX;// evnt.offsetX == undefined ? evnt.originalEvent.layerX : evnt.offsetX;
                ctrl.menuTop = evnt.clientY;// evnt.offsetY == undefined ? evnt.originalEvent.layerY : evnt.offsetY;
               
            };

            ctrl.headerSortableListener = {
                onSort: function (event) {
                    ctrl.rowHtml += ' ';
                    scope.$apply(function () {
                        
                    });
                     
                }
            };

            ctrl.switchColumnVisibility = function (colDef) {
                colDef.isHidden = !colDef.isHidden;
                calculateColumnsWidth();
            }
        }
        
        function buildRowHtml() {
            ctrl.rowHtml = '';
            for (var i = 0; i < ctrl.columnDefs.length; i++) {
                var currentColumn = ctrl.columnDefs[i];
                var currentColumnHtml = '$parent.ctrl.columnDefs[' + i + ']';
                ctrl.rowHtml += '<div ng-if="!' + currentColumnHtml + '.isHidden" ng-style="{ \'width\': ' + currentColumnHtml + '.width, \'display\':\'inline-block\'' + (i != 0 ? (',\'border-left\': \'' + currentColumn.borderRight) + '\'' : '') + '}">'
                + '<div class="vr-datagrid-cell">'
                + '    <div class="vr-datagrid-celltext">'
                  + UtilsService.replaceAll(ctrl.columnDefs[i].cellTemplate, "colDef", currentColumnHtml)
                    + '</div>'
                + '</div>'

            + '</div>';
            }
            buildSummaryRowHtml();
        }

        function buildSummaryRowHtml() {           
            if (ctrl.summaryDataItem != undefined) {
                ctrl.summaryRowHtml = '';
                for (var i = 0; i < ctrl.columnDefs.length; i++) {
                    var currentColumn = ctrl.columnDefs[i];
                    var currentColumnHtml = '$parent.ctrl.columnDefs[' + i + ']';
                    ctrl.summaryRowHtml += '<div ng-if="!' + currentColumnHtml + '.isHidden" ng-style="{ \'width\': ' + currentColumnHtml + '.width, \'display\':\'inline-block\'' + (i != 0 ? (',\'border-left\': \'' + currentColumn.borderRight) + '\'' : '') + '}">'
                    + '<div class="vr-datagrid-cell">'
                    + '    <div class="vr-datagrid-celltext">'
                      + UtilsService.replaceAll(ctrl.columnDefs[i].summaryCellTemplate, "colDef", currentColumnHtml)
                        + '</div>'
                    + '</div>'

                + '</div>';
                }
            }
        }
        
        function defineAPI() {           
            gridApi.resetSorting = function () {
                if (lastSortColumnDef != undefined) {
                    lastSortColumnDef.sortDirection = undefined;
                    lastSortColumnDef = undefined;
                }
            };

            gridApi.itemAdded = function (item) {
                itemChanged(item, "Added");
            };

            gridApi.itemUpdated = function (item) {
                item.isUpdated = true;
                itemChanged(item, "Updated");
            };

            gridApi.itemDeleted = function (item) {
                item.isDeleted = true;
                itemChanged(item, "Deleted");
            };

            gridApi.getPageInfo = getPageInfo;

            gridApi.clearDataAndContinuePaging = function () {
                ctrl.datasource.length = 0;
                stopPagingOnScroll = false;
            };

            gridApi.addItemsToSource = function (items) {
                var itemsToAdd = [];//create a new array to avoid changing the original items
                angular.forEach(items, function (itm) {
                    itemsToAdd.push(itm);
                })
                addBatchItemsToSource(itemsToAdd);
            };

            gridApi.addItemsToBegin = function (items) {
                var itemsToAdd = [];//create a new array to avoid changing the original items
                angular.forEach(items, function (itm) {
                    itemsToAdd.unshift(itm);
                })
                addBatchItemsToBeginSource(itemsToAdd);
            };

            function addBatchItemsToBeginSource(items) {
                var numberOfItems = pagingOnScrollEnabled ? getPageSize() : 10;//if paging on scroll is enabled, take the page size
                for (var i = 0; i < numberOfItems; i++) {
                    if (items.length > 0) {
                        ctrl.datasource.unshift(items[0]);
                        items.splice(0, 1);
                    }
                }

                if (items.length > 0) {
                    setTimeout(function () {
                        addBatchItemsToBeginSource(items);
                        scope.$apply(function () {

                        });

                    }, 10);
                }
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

            gridApi.setSummary = function (dataItem) {
                ctrl.summaryDataItem = dataItem;
                buildSummaryRowHtml();
            };

            gridApi.clearSummary = function (dataItem) {
                ctrl.summaryDataItem = undefined;
                buildSummaryRowHtml();
            };

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
            return 25;
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
            actionMenuWidth = 0;//hasActionMenu ? 3 : 0;
            ctrl.actionsColumnWidth = actionMenuWidth + '%';

            ctrl.isActionMenuVisible = function (dataItem) {
                if (!hasActionMenu)
                    return false;
                if (dataItem.isDeleted)
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
                var arrayofActions = (typeof (actionsAttribute) == 'function' ? actionsAttribute(dataItem) : actionsAttribute);

                if (arrayofActions != undefined && arrayofActions != null)
                {
                    for(var i=arrayofActions.length -1; i >= 0; i--)
                    {
                        if (arrayofActions[i].permissions != undefined && arrayofActions[i].permissions != null)
                        {
                            var isAllowed = SecurityService.isAllowed(arrayofActions[i].permissions);
                            if(!isAllowed)
                            {
                                arrayofActions.splice(i, 1);
                            }
                        }
                    }
                }

                return arrayofActions;
            }

            ctrl.menuActionClicked = function (action, dataItem) {
                action.clicked(dataItem);
            };
        }

        function addActionTypeColumn() {
            var col = {
                headerText: "Action",
                field: "actionType",
                nonHiddable:true
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

