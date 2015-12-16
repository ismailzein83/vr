﻿app.directive('vrDatagrid', ['UtilsService', 'SecurityService', 'DataRetrievalResultTypeEnum', '$compile', function (UtilsService, SecurityService, DataRetrievalResultTypeEnum, $compile) {

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
            onexport: '=',
            showexpand: '=',
            norowhighlightonclick: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            ctrl.itemsSortable = { handle: '.handeldrag', animation: 150 };
            var loadMoreDataFunction;
            if ($attrs.loadmoredata != undefined)
                loadMoreDataFunction = $scope.$parent.$eval($attrs.loadmoredata);
           
            var retrieveDataFunction
            if ($attrs.dataretrievalfunction != undefined)
                retrieveDataFunction = $scope.$parent.$eval($attrs.dataretrievalfunction);

            var pagingType;
            if ($attrs.pagingtype != undefined)
                pagingType = $scope.$parent.$eval($attrs.pagingtype);
                       
            var defaultSortDirection;
            if ($attrs.defaultsortdirection != undefined)
                defaultSortDirection = $scope.$parent.$eval($attrs.defaultsortdirection);

            ctrl.hideGridMenu = ($attrs.hidegridmenu != undefined);

            ctrl.showgmenu = false;
            ctrl.toggelGridMenu = function (e, bool) {
                if (bool != undefined)
                    ctrl.showgmenu = bool;
                else {
                    if (ctrl.showgmenu == false) {
                        setTimeout(function () {
                           
                            var self = angular.element(e.currentTarget);
                            var selfHeight = $(self).height();
                            var selfOffset = $(self).offset();
                            var menu = self.parent().find('.vr-grid-menu')[0];
                            $(menu).css({ position: 'fixed', top: selfOffset.top - $(window).scrollTop()+ 5, left: 'auto' });
                            $scope.$apply(function () {
                                ctrl.showgmenu = true;
                            })
                        }, 1);
                    }
                    else
                        ctrl.showgmenu = false;
                }
            }

            setTimeout(function () {
                $('.vr-grid-menu').parents('div').scroll(function () {
                        var menu = $(window).find('.vr-grid-menu')[0];
                        $(menu).css({ display: 'none' });
                        if (ctrl.showgmenu == true) {
                            ctrl.showgmenu = false;
                            $scope.$apply();
                        }
                    });
                $(window).on('scroll', function () {
                        var menu = $(window).find('.vr-grid-menu')[0];
                        $(menu).css({ display: 'none' });
                        if (ctrl.showgmenu == true) {
                            ctrl.showgmenu = false;
                            $scope.$apply();
                        }
                    });

            }, 1);
            
            ctrl.hidePagingInfo = ($attrs.hidepaginginfo != undefined);
            ctrl.rotateHeader = true;
            ctrl.el = $element;

            ctrl.cellLayoutStyle = $attrs.normalcell != undefined ? { 'white-space': 'normal' } : { 'white-space': 'nowrap' };
            
            if ($attrs.rotate == undefined) {
                ctrl.rotateHeader = false;
            }
            else
                ctrl.rotateHeader = $attrs.rotate;
            //console.log(ctrl.showexpand)
           // ctrl.expandabelcol = $attrs.showexpand != undefined? $scope.$eval(ctrl.showexpand) : undefined;
            var hasActionMenu = $attrs.menuactions != undefined;
            var actionsAttribute = hasActionMenu ? $scope.$parent.$eval($attrs.menuactions) : undefined;
            var enableDraggableRow = $attrs.enabledraggablerow != undefined ? $scope.$parent.$eval($attrs.enabledraggablerow) : false;
            var deleteRowFunction = $attrs.ondeleterow != undefined ? $scope.$parent.$eval($attrs.ondeleterow) : undefined;
            var dataGridObj = new DataGrid(ctrl, $scope, $attrs);
            dataGridObj.initializeController();
            if (retrieveDataFunction != undefined)
                dataGridObj.defineRetrieveData(retrieveDataFunction, pagingType, defaultSortDirection);

            if (loadMoreDataFunction != undefined)
                dataGridObj.definePagingOnScroll($scope, loadMoreDataFunction);
            dataGridObj.defineExpandableRow();
            dataGridObj.defineMenuColumn(hasActionMenu, actionsAttribute);
            dataGridObj.defineDraggableRow(enableDraggableRow);
            dataGridObj.defineDeleteRowAction(deleteRowFunction);
            dataGridObj.calculateDataColumnsSectionWidth();
            //dataGridObj.addActionTypeColumn();
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
 
    var cellTemplate = '<div style="text-align: #TEXTALIGN#;width: 100%;overflow: hidden;text-overflow: ellipsis;white-space: nowrap;" title="{{#CELLTOOLTIP#}}" >'
        + ''
      + '<a ng-if="#ISCLICKABLE#"  ng-class="#CELLCLASS#" ng-click="$parent.ctrl.onColumnClicked(colDef, dataItem)" style="cursor:pointer;"> {{#CELLVALUE# #CELLFILTER#}}#PERCENTAGE#</a>'
      + '<span ng-if="!#ISCLICKABLE#" ng-style="::$parent.ctrl.cellLayoutStyle" ng-class="#CELLCLASS#"> {{#CELLVALUE# #CELLFILTER#}}#PERCENTAGE#</span>'
      + ''
   + '</div>';

    var summaryCellTemplate = '<div style="text-align: #TEXTALIGN#;width: 100%;overflow: hidden;text-overflow: ellipsis;white-space: nowrap;" >'
        + ''
      + '<span class="span-summary" style="font-size:13px"> {{#COLUMNVALUES#.dataValue #CELLFILTER#}}</span>'
      + ''
   + '</div>';


    var headerTemplate = '<div ng-click="colDef.onSort()" class="vr-datagrid-header-cell" >'
   + ' <div col-index="renderIndex">'
     + '   <div class="vr-datagrid-celltext" style="overflow: hidden;"  ng-class="::colDef.textAlignmentClass" title="{{colDef.description}}" >'
       + '    <span ng-show="colDef.sortDirection==\'ASC\'">&uarr;</span>'
        + '   <span ng-show="colDef.sortDirection==\'DESC\'">&darr;</span>'
              +'<span ng-if="!colDef.rotateHeader"> {{colDef.name}}</span>'
              +'<p ng-if="colDef.rotateHeader" class="vr-rotate-header" >{{colDef.name}}</p>'
       + ' </div>'
      + '</div>'
+ '</div>';


    function DataGrid(ctrl, scope, attrs) {

        var gridApi = {};
        var maxHeight;
        var lastSortColumnDef;
        var actionMenuWidth;
        var expandableColumnWidth = 0;
        var draggableRowIconWidth = 0;
        var deleteRowColumnWidth = 0;
        var actionTypeColumnWidth;
        var expandableRowTemplate;
        //var actionTypeColumn;
        var stopPagingOnScroll;
        var pagingOnScrollEnabled;
        var retrieveDataFunction;
        var retrieveDataInput;
        var retrieveDataResultKey;
        var sortColumn;
        var isGridReady;
        var sortDirection;
        var defaultSortDirection;
        var lastAddedColumnId = 0;

        function addColumn(col, columnIndex) {
            var colDef = {
                name: col.headerText != undefined ? col.headerText : col.field,
                description:(col.headerDescription != undefined)? col.headerDescription :(  col.headerText != undefined ? col.headerText : col.field),
                headerCellTemplate: headerTemplate,//'/Client/Templates/Grid/HeaderTemplate.html',//template,
                field: col.field,
                isFieldDynamic: col.isFieldDynamic,
                summaryField: col.summaryField,
                tooltipField: col.tooltipField,
                enableSorting: col.enableSorting != undefined ? col.enableSorting : false,
                type:col.type,
                tag: col.tag,
                getcolor: col.getcolor,
                rotateHeader: ctrl.rotateHeader,
                nonHiddable: col.nonHiddable
            };
            lastAddedColumnId++;
            colDef.columnId = lastAddedColumnId;

            colDef.textAlignmentClass = colDef.type == "Number" ? 'vr-grid-cell-number' : '';
            if (col.onSortChanged == undefined) {
                col.onSortChanged = function (colDef_internal, sortDirection_internal) {
                    sortColumn = colDef_internal;
                    sortDirection = sortDirection_internal;
                    return retrieveData(false, false, true);
                };
            }

            colDef.onSort = function () {
                if (col.onSortChanged != undefined && col.disableSorting == false) {
                    
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
                ctrl.columnDefs.splice(ctrl.columnDefs.length , 0, colDef);//to insert before the actionType column
            else
                ctrl.columnDefs.splice(columnIndex, 0, colDef);

            if (ctrl.datasource != undefined) {
                for (var i = 0; i < ctrl.datasource.length; i++) {
                    filldataItemColumnValues(ctrl.datasource[i], colDef);
                }
            }
            if (ctrl.updateItems != undefined) {
                for (var i = 0; i < ctrl.updateItems.length; i++) {
                    filldataItemColumnValues(ctrl.updateItems[i], colDef);
                }
            }
            if (ctrl.summaryDataItem != undefined)
                fillSummaryDataItemColumnValues(colDef);
           
            calculateColumnsWidth();
            scheduleRowHtmlBuild();
            return colDef;
        }

        function getCellTemplateWithFilter(template, col) {
            if (col.type == "Number") {
                var numberPrecision = 2;
                if (col.numberPrecision == "NoDecimal")
                    numberPrecision = 0;
                else if (col.numberPrecision == "LongPrecision")
                    numberPrecision = 4;
                template = template.replace("#TEXTALIGN#", "right;padding-right:2px");
                template = UtilsService.replaceAll(template, "#CELLFILTER#", "| number:" + numberPrecision);
                template = UtilsService.replaceAll(template, "#PERCENTAGE#", "");
            }
            else if (col.type == "Progress" || col.type == "MultiProgress") {
                template = template.replace("#TEXTALIGN#", "center;position:absolute;color:#666;");
                template = UtilsService.replaceAll(template, "#CELLFILTER#", "| number:1");
                template = UtilsService.replaceAll(template, "#PERCENTAGE#", "%");
            }
            else {
                template = UtilsService.replaceAll(template, "#PERCENTAGE#", "");
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
            scheduleRowHtmlBuild();
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

        function calculateDataColumnsSectionWidth() {
            defineActionTypeColumn();
            var otherSectionsWidth = actionMenuWidth + expandableColumnWidth + draggableRowIconWidth + deleteRowColumnWidth + actionTypeColumnWidth;
            //if (width < 100)
            //    width -= 1;
            ctrl.dataColumnsSectionWidth = "calc(100% - " + otherSectionsWidth + "px)";
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
                    deletedItemIndex = UtilsService.getItemIndexByVal(ctrl.datasource, eval('item.' + ctrl.idfield), ctrl.idfield);
                if (deletedItemIndex >= 0)
                    ctrl.datasource.splice(deletedItemIndex, 1);
            }

            if (ctrl.idfield != undefined) {
                //remove the item if exists in the updatedItems array
                var itemIndexInUpdatedItems = UtilsService.getItemIndexByVal(ctrl.updateItems, eval('item.' + ctrl.idfield), ctrl.idfield);
                if(itemIndexInUpdatedItems >= 0)
                    ctrl.updateItems.splice(itemIndexInUpdatedItems, 1);

                //update the item in the datasource array if exists
                var itemIndexInDataSource = UtilsService.getItemIndexByVal(ctrl.datasource, eval('item.' + ctrl.idfield), ctrl.idfield);
                if (itemIndexInDataSource >= 0)
                    ctrl.datasource[itemIndexInDataSource] = item;
            }            
            
            item.actionType = actionType;
            ctrl.updateItems.splice(0, 0, item);
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
            //ctrl.viewVisibilityChanged = function () {
            //    if (!ctrl.isMainItemsShown) {
            //        ctrl.showColumn(actionTypeColumn);
            //    }
            //    else {
            //        ctrl.hideColumn(actionTypeColumn);
            //    }
            //};


            ctrl.addColumn = addColumn;
            ctrl.removeColumn = removeColumn;
            ctrl.hideColumn = hideColumn;
            ctrl.showColumn = showColumn;
            ctrl.updateColumnHeader = updateColumnHeader;

            ctrl.onColumnClicked = function (colDef, dataItem) {
                if (colDef.onClickedAttr != undefined)
                    colDef.onClickedAttr(dataItem, colDef);
            };

            var lastSelectedRow;
            ctrl.onRowClicked = function (evnt) {
                if (ctrl.norowhighlightonclick == undefined || !ctrl.norowhighlightonclick) {
                    if (lastSelectedRow != undefined)
                        lastSelectedRow.removeClass('vr-datagrid-datacells-click');

                    lastSelectedRow = angular.element(evnt.currentTarget);
                    lastSelectedRow.addClass('vr-datagrid-datacells-click');
                }
                ctrl.menuLeft = evnt.clientX;// evnt.offsetX == undefined ? evnt.originalEvent.layerX : evnt.offsetX;
                ctrl.menuTop = evnt.clientY;// evnt.offsetY == undefined ? evnt.originalEvent.layerY : evnt.offsetY;
               
            };

            ctrl.headerSortableListener = {
                onSort: function (event) {
                    buildRowHtml();
                }
            };

            ctrl.switchColumnVisibility = function (colDef) {
                colDef.isHidden = !colDef.isHidden;
                calculateColumnsWidth();
            }

            ctrl.isExporting = false;
            ctrl.onExportClicked = function () {
                var promise;
                if (this.onexport != undefined && typeof (this.onexport) == 'function') {
                    promise = this.onexport();
                }
                else {
                    promise = retrieveData(false, true);
                }

                if (promise != undefined && promise != null) {
                    ctrl.isExporting = true;
                    promise.finally(function () {
                        ctrl.isExporting = false;
                    });
                }
            };

            ctrl.viewSelectionChanged = function () {
                calculateDataColumnsSectionWidth();
            };

            ctrl.getCellValue = function (dataItem, colDef) {
                return eval('dataItem.' + colDef.field);
            };

            ctrl.getCellTooltip = function (dataItem, colDef) {
                if (colDef.tooltipField != undefined)
                    return eval('dataItem.' + colDef.tooltipField);
                else if(colDef.field != undefined)
                    return eval('dataItem.' + colDef.field);
            };

            ctrl.getCellClass = function (dataItem, colDef) {
                if (colDef.getcolor != undefined)
                    return colDef.getcolor(dataItem, colDef);
                if (colDef.cellClass == undefined)
                    return 'span-summary';
            };

            ctrl.isColumnClickable = function (dataItem, colDef) {
                if (colDef == undefined || colDef.isClickableAttr == undefined)
                    return false;
                else {
                    if (typeof (colDef.isClickableAttr) == 'function')
                        return colDef.isClickableAttr(dataItem);
                    else
                        return colDef.isClickableAttr;
                }
            };

            scope.$watchCollection('ctrl.datasource', onDataSourceChanged);
            scope.$watchCollection('ctrl.updateItems', onDataSourceChanged);

            function onDataSourceChanged(newDataSource, oldNames) {
                for (var i = 0; i < newDataSource.length; i++) {
                    var dataItem = newDataSource[i];
                    if (!dataItem.isColumnValuesFilled){                        
                        for (var j = 0; j < ctrl.columnDefs.length; j++) {
                            var colDef = ctrl.columnDefs[j];
                            filldataItemColumnValues(dataItem, colDef);
                        }
                        dataItem.isColumnValuesFilled = true;
                    }
                }
            }
                        
        }

        function getDataItemColumnProperty(colDef) {
            return 'col_' + colDef.columnId;
        }
        
        var isRowHtmlBuildScheduled;
        function scheduleRowHtmlBuild() {
            if (isRowHtmlBuildScheduled)
                return;
            isRowHtmlBuildScheduled = true;
            setTimeout(function () {
                isRowHtmlBuildScheduled = false;
                buildRowHtml();
                
            }, 100);
        }

        function buildRowHtml() {
            ctrl.rowHtml = '';
            var gridvalue;
            for (var i = 0; i < ctrl.columnDefs.length; i++) {
                var currentColumn = ctrl.columnDefs[i];
                var currentColumnHtml = '$parent.ctrl.columnDefs[' + i + ']';
                var dataItemColumnPropertyPath = "dataItem.columnsValues." + getDataItemColumnProperty(currentColumn);
                
                ctrl.rowHtml += '<div ng-if="!' + currentColumnHtml + '.isHidden" ng-style="{ \'width\': ' + currentColumnHtml + '.width, \'display\':\'inline-block\'' + (i != 0 ? (',\'border-left\': \'' + currentColumn.borderRight) + '\'' : '') + '}"">';
                if (currentColumn.type == "MultiProgress") {
                    var values = currentColumn.field.split("|");
                    ctrl.rowHtml += '<vr-progressbar gridvalue="';
                    for (var j = 0; j < values.length; j++) {
                        ctrl.rowHtml += ('{{::dataItem.' + values[j] + '}}');
                        if (j < values.length-1)
                            ctrl.rowHtml += "|";

                    }
                    ctrl.rowHtml += '"></vr-progressbar></div>';
               
                }
                else if (currentColumn.type == "Progress") {
                    gridvalue = "{{::" + dataItemColumnPropertyPath + ".dataValue}}";
                    ctrl.rowHtml += '<vr-progressbar gridvalue="' + gridvalue + '" ></vr-progressbar></div>';
                }else
                {
                    var cellTemplate = currentColumn.cellTemplate;
                    if (currentColumn.isFieldDynamic) {
                        cellTemplate = UtilsService.replaceAll(cellTemplate, "#CELLVALUE#", "ctrl.getCellValue(dataItem, colDef)");
                        cellTemplate = UtilsService.replaceAll(cellTemplate, "#CELLTOOLTIP#", "ctrl.getCellTooltip(dataItem, colDef)");
                        cellTemplate = UtilsService.replaceAll(cellTemplate, "#CELLCLASS#", "ctrl.getCellClass(dataItem, colDef)");
                        cellTemplate = UtilsService.replaceAll(cellTemplate, "#ISCLICKABLE#", "ctrl.isColumnClickable(dataItem, colDef)"); 
                    }
                    else {
                        cellTemplate = UtilsService.replaceAll(cellTemplate, "#CELLVALUE#", "::" + dataItemColumnPropertyPath + ".dataValue");
                        cellTemplate = UtilsService.replaceAll(cellTemplate, "#CELLTOOLTIP#", "::" + dataItemColumnPropertyPath + ".tooltip");
                        cellTemplate = UtilsService.replaceAll(cellTemplate, "#CELLCLASS#", "::" + dataItemColumnPropertyPath + ".cellClass");
                        cellTemplate = UtilsService.replaceAll(cellTemplate, "#ISCLICKABLE#", dataItemColumnPropertyPath + ".isClickable");
                    }
                    cellTemplate = UtilsService.replaceAll(cellTemplate, "colDef", currentColumnHtml);
                    ctrl.rowHtml += '<div class="vr-datagrid-cell">'
                        + '    <div class="vr-datagrid-celltext ">'
                       + cellTemplate
                     +'</div>'
                     + '</div>'
                        + '</div>'; 
                }
                
            }        
            buildSummaryRowHtml();
        }

        function buildSummaryRowHtml() {           
            if (ctrl.summaryDataItem != undefined) {
                ctrl.summaryRowHtml = '';
                for (var i = 0; i < ctrl.columnDefs.length; i++) {
                    var currentColumn = ctrl.columnDefs[i];
                    var currentColumnHtml = '$parent.ctrl.columnDefs[' + i + ']';
                    var dataItemColumnPropertyPath = "ctrl.summaryDataItem.columnsValues." + getDataItemColumnProperty(currentColumn);
                    ctrl.summaryRowHtml += '<div ng-if="!' + currentColumnHtml + '.isHidden" ng-style="{ \'width\': ' + currentColumnHtml + '.width, \'display\':\'inline-block\'' + (i != 0 ? (',\'border-left\': \'' + currentColumn.borderRight) + '\'' : '') + '}">'
                    + '<div class="vr-datagrid-cell">'
                    + '    <div class="vr-datagrid-celltext">'
                      + UtilsService.replaceAll(currentColumn.summaryCellTemplate, "#COLUMNVALUES#", dataItemColumnPropertyPath)
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
                if (pagingOnScrollEnabled) {
                    // to rigth padding in old data loading methode in bi
                    if (ctrl.datasource.length <= 11)
                        ctrl.headerStyle = {
                            "padding-right": "0px"
                        }
                    else
                        ctrl.headerStyle = {
                            "padding-right": getScrollbarWidth() + "px"
                        }
                }
            }

            gridApi.setSummary = function (dataItem) {
                ctrl.summaryDataItem = dataItem;
                
                for (var j = 0; j < ctrl.columnDefs.length; j++) {
                    var colDef = ctrl.columnDefs[j];
                    fillSummaryDataItemColumnValues(colDef);
                }

                buildSummaryRowHtml();
            };

            gridApi.clearSummary = function (dataItem) {
                ctrl.summaryDataItem = undefined;
                buildSummaryRowHtml();
            };
            
            gridApi.retrieveData = function (query) {
                //retrieveDataInput should be of type Vanrise.Entities.RetrieveDataInput<T>
                retrieveDataInput = {
                    Query: query                    
                };
                return retrieveData(true);
            };
            setTimeout(function () {

                isGridReady = true;
                if (ctrl.onReady != null)
                    ctrl.onReady(gridApi);
            }, 100);

            gridApi.expandRow = function (dataItem) {
                ctrl.expandRow(dataItem);
            };

            gridApi.collapseRow = function (dataItem) {
                ctrl.collapseRow(dataItem);
            };            
        }

        function filldataItemColumnValues(dataItem, colDef) {
            try {

                var colValuesObj = {};
                if (colDef.field != undefined)
                    colValuesObj.dataValue = ctrl.getCellValue(dataItem, colDef);
                colValuesObj.tooltip = ctrl.getCellTooltip(dataItem, colDef);

                colValuesObj.cellClass = ctrl.getCellClass(dataItem, colDef);

                colValuesObj.isClickable = ctrl.isColumnClickable(dataItem, colDef);

                if (dataItem.columnsValues == undefined)
                    dataItem.columnsValues = {};

                dataItem.columnsValues[getDataItemColumnProperty(colDef)] = colValuesObj;
            }
            catch (ex) {

            }
        }

        function fillSummaryDataItemColumnValues(colDef) {
            try{
                if (ctrl.summaryDataItem == undefined)
                    return;
                if (ctrl.summaryDataItem.columnsValues == undefined)
                    ctrl.summaryDataItem.columnsValues = {};
                var colValuesObj = {};
                if (colDef.field != undefined)
                    colValuesObj.dataValue = eval('ctrl.summaryDataItem.' + colDef.summaryField);

                ctrl.summaryDataItem.columnsValues[getDataItemColumnProperty(colDef)] = colValuesObj;
            }
            catch (ex) {

            }
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
                if (ctrl.maxheight1=undefined)
                    setMaxHeight(ctrl.maxheight);
                else {

                    var sh = screen.height;
                    var h;
                    if (isInModal() == true)
                        h = screen.height * 0.3;
                    else
                        h = screen.height * 0.55;

                    setMaxHeight(h+"px");
                }
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
                            var div = $(ctrl.el).find("#gridBodyContainer")[0];// need real DOM Node, not jQuery wrapper
                            var hasVerticalScrollbar = div.scrollHeight > div.clientHeight;
                            if (hasVerticalScrollbar)
                                ctrl.headerStyle = {
                                    "padding-right": getScrollbarWidth()+"px"
                                }
                            else ctrl.headerStyle = {
                                "padding-right": "0px"
                            }
                        });
                    }
                });

            };
        }
        function isInModal() {
            return ($(ctrl.el).find("#gridBodyContainer").parents('.modal-body').length > 0)
        }

        function getPageSize() {
            var h;
            if (isInModal() == true)
                h = screen.height * 0.3;
            else
                h = screen.height * 0.55;

            var pagesize = (Math.ceil(parseInt((h / 25) * 1.5) / 10) * 10) < 25 ? 25 : (Math.ceil(parseInt((h / 25) * 1.5) / 10) * 10);
            return pagesize;
        }
        function getScrollbarWidth() {            
            var outer = document.createElement("div");
            outer.style.visibility = "hidden";
            outer.style.width = "100px";
            outer.style.msOverflowStyle = "scrollbar"; // needed for WinJS apps

            document.body.appendChild(outer);

            var widthNoScroll = outer.offsetWidth;
            // force scrollbars
            outer.style.overflow = "scroll";

            // add innerdiv
            var inner = document.createElement("div");
            inner.style.width = "100%";
            outer.appendChild(inner);

            var widthWithScroll = inner.offsetWidth;

            // remove divs
            outer.parentNode.removeChild(outer);

            return widthNoScroll - widthWithScroll;
        }

        function getPageInfo(startFromBeginning) {
            var fromRow = startFromBeginning ? 1 : ctrl.datasource.length + 1;
            return {
                fromRow: fromRow,
                toRow: fromRow + getPageSize() - 1
            };
        }

        function defineExpandableRow() {
            ctrl.setExpandableRowTemplate = function (template, isexpandble) {
                expandableRowTemplate = template;
                expandableColumnWidth = 20;
                ctrl.expandableColumnWidth = expandableColumnWidth + 'px';
                ctrl.expandableSectionWidth = "calc(100% - " + expandableColumnWidth + "px)";
                calculateDataColumnsSectionWidth();
            };
            ctrl.showExpandCollapseIcon = function (dataItem) {
                if (ctrl.showexpand != undefined && typeof (ctrl.showexpand) == 'function') {
                    var showExpand = ctrl.showexpand(dataItem);
                    return showExpand;
                }

                return true;
               
            }
            ctrl.expandRow = function (dataItem) {
                
                dataItem.expandableRowTemplate = expandableRowTemplate;
                dataItem.isRowExpanded = true;
            };

            ctrl.collapseRow = function (dataItem) {
                dataItem.isRowExpanded = false;
            };
        }

        function defineMenuColumn(hasActionMenu, actionsAttribute) {
            actionMenuWidth = 0;//hasActionMenu ? 3 : 0;
            ctrl.actionsColumnWidth = actionMenuWidth + 'px';

            ctrl.isActionMenuVisible = function (dataItem) {
                if (!hasActionMenu)
                    return false;
                if (dataItem.isDeleted)
                    return false;
                var actions = ctrl.getMenuActions(dataItem);
                return actions != undefined && actions != null && actions.length > 0;
            };

            ctrl.adjustPosition = function (e) {
                var self = angular.element(e.currentTarget);
                var selfHeight = $(this).parent().height() + 12;
                var selfWidth = $(this).parent().width();
                var selfOffset = $(self).offset();
                var w = $(window);
                var selfOffsetRigth = $(document).width() - selfOffset.left - selfWidth;
                var dropDown = self.parent().find('ul');
                $(dropDown).css({ position: 'fixed', top: (selfOffset.top - w.scrollTop()) + selfHeight, left: 'auto' });
            };

            ctrl.getMenuActions = function (dataItem) {
                var arrayofActions = (typeof (actionsAttribute) == 'function' ? actionsAttribute(dataItem) : actionsAttribute);

                if (arrayofActions != undefined && arrayofActions != null) {
                    for (var i = arrayofActions.length - 1; i >= 0; i--) {
                        if (arrayofActions[i].permissions != undefined && arrayofActions[i].permissions != null) {
                            var isAllowed = SecurityService.isAllowed(arrayofActions[i].permissions);
                            if (!isAllowed) {
                                arrayofActions.splice(i, 1);
                            }
                        }
                    }
                }

                return arrayofActions;
            };

           

            ctrl.menuActionClicked = function (action, dataItem) {
                action.clicked(dataItem);
            };
        }

        function defineDraggableRow(enableDraggableRow) {
            if (enableDraggableRow) {
                draggableRowIconWidth = 17;
                ctrl.draggableRowIconWidth = draggableRowIconWidth + "px";
                ctrl.showDraggableRowSection = true;
            }
        }

        function defineDeleteRowAction(deleteRowFunction) {
            if (deleteRowFunction != undefined) {
                deleteRowColumnWidth = 17;
                ctrl.deleteRowColumnWidth = deleteRowColumnWidth + "px";
                ctrl.showDeleteRowSection = true;

                ctrl.deleteRowClicked = function (dataItem) {
                    deleteRowFunction(dataItem);
                };
            }
        }

        function defineActionTypeColumn() {
            if (ctrl.isMainItemsShown)
                actionTypeColumnWidth = 0;
            else
                actionTypeColumnWidth = 60;
            ctrl.actionTypeColumnWidth = actionTypeColumnWidth + "px";
        }

        //function addActionTypeColumn() {
        //    var col = {
        //        headerText: "Action",
        //        field: "actionType",
        //        nonHiddable:true
        //    };
        //    actionTypeColumn = addColumn(col, ctrl.columnDefs.length);
        //    //ctrl.hideColumn(actionTypeColumn);
        //}

        function defineRetrieveData(retrieveDataFunc, pagingType, defaultSortDirection_local) {
            retrieveDataFunction = retrieveDataFunc;
            defaultSortDirection = defaultSortDirection_local;

            switch(pagingType)
            {
                case "Pager":
                    ctrl.showPager = true;
                    ctrl.pagerSettings = {
                        currentPage: 1,
                        totalDataCount: 0,
                        pageChanged: retrieveData
                    }
                    break;
                case "PagingOnScroll":
                    definePagingOnScroll(scope, retrieveData);
                    break;
            }
        }
        
        function retrieveData(clearBeforeRetrieve, isExport, isSorting) {
            if (!isGridReady)
                return;

            var defaultSortByFieldName;
            if (attrs.defaultsortbyfieldname != undefined)
                defaultSortByFieldName = scope.$parent.$eval(attrs.defaultsortbyfieldname);
            
            if (clearBeforeRetrieve) {
                retrieveDataResultKey = null;                
                sortColumn = defaultSortByFieldName != undefined ? undefined : ctrl.columnDefs[0];
                sortDirection = defaultSortDirection != undefined ? defaultSortDirection : "ASC";
            }
            if (clearBeforeRetrieve || isSorting) {
                if (ctrl.showPager)
                    ctrl.pagerSettings.currentPage = 1;
            }           

            retrieveDataInput.SortByColumnName = sortColumn != undefined ? sortColumn.field : defaultSortByFieldName;
            if (retrieveDataInput.SortByColumnName == undefined)
                return;
            retrieveDataInput.IsSortDescending = (sortDirection == "DESC");
            
            retrieveDataInput.ResultKey = retrieveDataResultKey;//retrieveDataInput should be of type Vanrise.Entities.RetrieveDataInput<T>
            if (isExport) {
                retrieveDataInput.DataRetrievalResultType = DataRetrievalResultTypeEnum.Excel.value;
                retrieveDataInput.FromRow = null;
                retrieveDataInput.ToRow = null;
            }
            else {
                retrieveDataInput.DataRetrievalResultType = DataRetrievalResultTypeEnum.Normal.value;
                var pageInfo;
                if (ctrl.showPager)
                    pageInfo = ctrl.pagerSettings.getPageInfo();
                else if (pagingOnScrollEnabled)
                    pageInfo = getPageInfo(clearBeforeRetrieve || isSorting);

                if (pageInfo != undefined) {
                    retrieveDataInput.FromRow = pageInfo.fromRow;
                    retrieveDataInput.ToRow = pageInfo.toRow;
                }
            }

            

            var onResponseReady = function (response) {
                if (isExport) {
                    UtilsService.downloadFile(response.data, response.headers);
                }
                else {
                    if (clearBeforeRetrieve) 
                        gridApi.resetSorting();                    
                    else if (ctrl.showPager)
                        ctrl.datasource.length = 0;

                    if (clearBeforeRetrieve || isSorting)
                        gridApi.clearDataAndContinuePaging();

                    gridApi.addItemsToSource(response.Data);//response should be of type Vanrise.Entities.BigResult<T>
                    retrieveDataResultKey = response.ResultKey;
                    if (ctrl.pagerSettings != undefined && ctrl.pagerSettings != null)
                        ctrl.pagerSettings.totalDataCount = response.TotalCount;

                }
            };
              
           
            var promise = retrieveDataFunction(retrieveDataInput, onResponseReady);//this function should return a promise in case it is getting data
            
            if (!pagingOnScrollEnabled && promise != undefined && promise != null) {
                ctrl.isLoadingMoreData = true;
                promise.finally(function () {
                    ctrl.isLoadingMoreData = false;
                });
            }
            return promise;            
        }

        this.initializeController = initializeController;
        this.defineAPI = defineAPI;
        this.definePagingOnScroll = definePagingOnScroll;
        this.defineExpandableRow = defineExpandableRow;
        this.defineMenuColumn = defineMenuColumn;
        this.defineDraggableRow = defineDraggableRow;
        this.calculateDataColumnsSectionWidth = calculateDataColumnsSectionWidth;
        //this.addActionTypeColumn = addActionTypeColumn;
        this.defineRetrieveData = defineRetrieveData;
        this.defineDeleteRowAction = defineDeleteRowAction;
    }


    return directiveDefinitionObject;
}]);

