'use strict';

app.directive('vrDatagrid', ['UtilsService', 'SecurityService', 'DataRetrievalResultTypeEnum', '$compile', 'VRModalService', 'DataGridRetrieveDataEventType', 'UISettingsService', 'VRLocalizationService',
    function (UtilsService, SecurityService, DataRetrievalResultTypeEnum, $compile, VRModalService, DataGridRetrieveDataEventType, UISettingsService, VRLocalizationService) {

        var paddingDirection = VRLocalizationService.isLocalizationRTL() ? "'padding-left'" : "'padding-right'";
        var normaltextDirection = VRLocalizationService.isLocalizationRTL() ? "right" : "left";
        var numbertextDirection = "right";
        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                datasource: '=',
                onReady: '=',
                getrowstyle: '=',
                isdynamicrowstyle: '@',
                maxheight: '@',
                hideheader: '=',
                noverticallines: '@',
                idfield: '@',
                onexport: '=',
                showexpand: '=',
                norowhighlightonclick: '=',
                gridmenuactions: '=',
                margin: '=',
                dragdropsetting: '='
            },
            controller: function ($scope, $element, $attrs) {
                $scope.$on("$destroy", function () {
                    $('.vr-grid-menu').parents('div').unbind('scroll', hideGridColumnsMenu);
                    $(window).unbind('scroll', hideGridColumnsMenu);
                    $(document).unbind('click', bindClickOutSideGridMenu);
                    $element.remove();
                });
                var ctrl = this;
                ctrl.itemsSortable = { handle: '.handeldrag', animation: 150 };
                if (ctrl.dragdropsetting != undefined && typeof (ctrl.dragdropsetting) == 'object') {
                    ctrl.itemsSortable.group = {
                        name: ctrl.dragdropsetting.groupCorrelation.getGroupName(),
                        pull: ctrl.dragdropsetting.canSend == true && ctrl.dragdropsetting.copyOnSend == true ? "clone" : ctrl.dragdropsetting.canSend,
                        put: ctrl.dragdropsetting.canReceive
                    };
                    ctrl.itemsSortable.sort = ctrl.dragdropsetting.enableSorting;
                    ctrl.itemsSortable.onAdd = function (/**Event*/evt) {
                        var obj = evt.model;
                        if (ctrl.dragdropsetting.onItemReceived != undefined && typeof (ctrl.dragdropsetting.onItemReceived) == 'function')
                            obj = ctrl.dragdropsetting.onItemReceived(evt.model, evt.models);
                        evt.models[evt.newIndex] = obj;
                    };
                }

                ctrl.layoutOption = UISettingsService.getGridLayoutOptions();

                ctrl.readOnly = UtilsService.isContextReadOnly($scope) || $attrs.readonly != undefined;

                var loadMoreDataFunction;
                if ($attrs.loadmoredata != undefined)
                    loadMoreDataFunction = $scope.$parent.$eval($attrs.loadmoredata);

                var retrieveDataFunction;
                if ($attrs.dataretrievalfunction != undefined)
                    retrieveDataFunction = $scope.$parent.$eval($attrs.dataretrievalfunction);

                var pagingType;
                if ($attrs.pagingtype != undefined)
                    pagingType = $scope.$parent.$eval($attrs.pagingtype);

                //var defaultSortDirection;
                //if ($attrs.defaultsortdirection != undefined)
                //    defaultSortDirection = $scope.$parent.$eval($attrs.defaultsortdirection);

                ctrl.hideGridMenu = ($attrs.hidegridmenu != undefined);

                ctrl.showgmenu = false;

                $scope.$on("hide-all-menu", function (event, args) {
                    ctrl.showgmenu = false;
                    $('.vr-grid-menu').removeClass("open-grid-menu");
                    $(document).unbind('click', bindClickOutSideGridMenu);
                });

                ctrl.toggelGridMenu = function (e) {
                    var self = angular.element(e.currentTarget);

                    var menu = self.parent().find('.vr-grid-menu')[0];
                    if (ctrl.showgmenu == false) {
                        setTimeout(function () {
                            var selfHeight = $(self).height();
                            var selfOffset = $(self).offset();
                            $(menu).css({ display: 'block' });
                            $(menu).addClass("open-grid-menu");
                            $(menu).css({ position: 'fixed', top: selfOffset.top - $(window).scrollTop() + 5, left: 'auto' });
                            ctrl.showgmenu = true;
                            $scope.$root.$digest();
                        }, 1);
                        $(document).bind("click", bindClickOutSideGridMenu);

                    }
                    else {
                        ctrl.showgmenu = false;
                        $(menu).removeClass("open-grid-menu");
                        $(document).unbind('click', bindClickOutSideGridMenu);
                    }
                };
                function bindClickOutSideGridMenu(e) {
                    if (!$('out-div').is(e.target) && $('out-div').has(e.target).length === 0 && $('.open-grid-menu').has(e.target).length === 0) {
                        $('out-div').removeClass("open-grid-menu");
                        ctrl.showgmenu = false;
                        $scope.$root.$digest();
                    }
                }
                setTimeout(function () {
                    $('.vr-grid-menu').parents('div').on('scroll', hideGridColumnsMenu);
                    $(window).on('scroll', hideGridColumnsMenu);
                }, 1);

                function hideGridColumnsMenu() {
                    var menu = $('.vr-grid-menu');
                    menu.css({ display: 'none' });
                    $('out-div').removeClass("open-grid-menu");
                    if (ctrl.showgmenu == true) {
                        ctrl.showgmenu = false;
                        $scope.$root.$digest();
                    }

                };
                ctrl.hidePagingInfo = ($attrs.hidepaginginfo != undefined);
                ctrl.rotateHeader = true;
                ctrl.cellLayoutStyle = $attrs.normalcell != undefined ? { 'white-space': 'normal' } : { 'white-space': 'nowrap' };

                if ($attrs.rotate == undefined) {
                    ctrl.rotateHeader = false;
                }
                else
                    ctrl.rotateHeader = $attrs.rotate;
                var hasActionMenu = $attrs.menuactions != undefined;
                var actionsAttribute = hasActionMenu ? $scope.$parent.$eval($attrs.menuactions) : undefined;
                var enableDraggableRow = $attrs.enabledraggablerow != undefined ? $scope.$parent.$eval($attrs.enabledraggablerow) : false;
                var deleteRowFunction = $attrs.ondeleterow != undefined ? $scope.$parent.$eval($attrs.ondeleterow) : undefined;
                var dataGridObj = new DataGrid(ctrl, $scope, $attrs, $element);
                dataGridObj.initializeController();
                if (retrieveDataFunction != undefined)
                    dataGridObj.defineRetrieveData(retrieveDataFunction, pagingType);
                if (loadMoreDataFunction != undefined)
                    dataGridObj.definePagingOnScroll($scope, loadMoreDataFunction);
                dataGridObj.defineExpandableRow();
                dataGridObj.defineMenuColumn(hasActionMenu, actionsAttribute);
                dataGridObj.defineDraggableRow(enableDraggableRow);
                dataGridObj.defineDeleteRowAction(deleteRowFunction);
                dataGridObj.calculateDataColumnsSectionWidth();
                dataGridObj.defineAPI();
                dataGridObj = null;

            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                element.append('<vr-datagridrows></vr-datagridrows>');

                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {
                        $scope.$on('$destroy', function () {
                            iElem.find('vr-datagridrows').remove();
                        });

                        var ctrl = $scope.ctrl;
                        if (iAttrs.pagersettings != undefined) {
                            ctrl.pagerSettings = $scope.$parent.$eval(iAttrs.pagersettings);
                            ctrl.showPager = true;
                        }
                    }
                };
            }

        };

        var cellTemplateNormalContent = '<span ng-style="::$parent.ctrl.cellLayoutStyle" ng-class="#CELLCLASS#"> {{#CELLVALUE# #CELLFILTER#}}#PERCENTAGE#</span>';
        var cellTemplateClickableContent = '<a ng-class="#CELLCLASS#" ng-click="$parent.ctrl.onColumnClicked(colDef, dataItem , $event)" style="cursor:pointer;"> {{#CELLVALUE# #CELLFILTER#}}#PERCENTAGE#</a>';
        var cellTemplateExpendableContent = '<a ng-class="#CELLCLASS#" ng-click="$parent.ctrl.onDescriptionClicked(colDef, dataItem)" style="cursor:pointer;"> {{#CELLVALUE# #CELLFILTER#}}#PERCENTAGE#</a>';

        var cellTemplate = '<div style="text-align: #TEXTALIGN#;width: 100%;overflow: hidden;text-overflow: ellipsis;white-space: nowrap;" title="{{#CELLTOOLTIP# #TOOLTIPFILTER#}}" >'
                            + '#CELLCONTENT#'
                         + '</div>';

        var summaryCellTemplate = '<div style="text-align: #TEXTALIGN#;width: 100%;overflow: hidden;text-overflow: ellipsis;white-space: nowrap;" >'
            + ''
          + '<span class="span-summary" style="font-size:13px"> {{#COLUMNVALUES#.dataValue #CELLFILTER#}}</span>'
          + ''
       + '</div>';


        var headerTemplate = '<div ng-click="colDef.onSort()" class="vr-datagrid-header-cell" >'
       + ' <div col-index="renderIndex" style="width:100%">'
         + '   <div class="vr-datagrid-celltext" style="overflow: hidden;"  ng-class="::colDef.textAlignmentClass" title="{{colDef.description}}" >'
           + '    <span ng-show="colDef.sortDirection==\'ASC\'">&uarr;</span>'
            + '   <span ng-show="colDef.sortDirection==\'DESC\'">&darr;</span>'
                  + '<span ng-if="!colDef.rotateHeader"> {{colDef.name}}</span>'
                  + '<p ng-if="colDef.rotateHeader" class="vr-rotate-header" >{{colDef.name}}</p>'
           + ' </div>'
          + '</div>'
    + '</div>';


        function DataGrid(ctrl, scope, attrs, elem) {

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
            //var defaultSortDirection;
            var lastAddedColumnId = 0;

            function addColumn(col, columnIndex) {
                var colDef = {
                    name: col.headerText != undefined ? col.headerText : col.field,
                    description: (col.headerDescription != undefined) ? col.headerDescription : (col.headerText != undefined ? col.headerText : col.field),
                    headerCellTemplate: headerTemplate,//'/Client/Templates/Grid/HeaderTemplate.html',//template,
                    field: col.field,
                    isFieldDynamic: col.isFieldDynamic,
                    summaryField: col.summaryField,
                    tooltipField: col.tooltipField,
                    enableSorting: col.enableSorting != undefined ? col.enableSorting : false,
                    type: col.type,
                    numberPrecision: col.numberPrecision,
                    tag: col.tag,
                    getcolor: col.getcolor,
                    rotateHeader: ctrl.rotateHeader,
                    nonHiddable: col.nonHiddable,
                    expendableColumn: col.expendableColumn,
                    expendableColumnTitle: col.expendableColumnTitle,
                    expendableColumnDescription: col.expendableColumnDescription,
                    fixedWidth: col.fixedWidth,
                    invisibleHeader: col.invisibleheader,
                    cssClass: col.cssclass


                };
                lastAddedColumnId++;
                colDef.columnId = lastAddedColumnId;

                colDef.textAlignmentClass = colDef.type == "Number" ? 'vr-grid-cell-number' : '';
                if (col.onSortChanged == undefined) {
                    col.onSortChanged = function (colDef_internal, sortDirection_internal) {
                        sortColumn = colDef_internal;
                        sortDirection = sortDirection_internal;
                        return retrieveData(false, false, true, DataGridRetrieveDataEventType.Sorting);
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
                    columnCellTemplate = getCellTemplateWithFilter(cellTemplate, colDef);
                }
                colDef.cellTemplate = columnCellTemplate;
                colDef.summaryCellTemplate = getCellTemplateWithFilter(summaryCellTemplate, colDef);

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

                if (col.fixedWidth == undefined)
                    col.fixedWidth = 0;
                else
                    col.fixedWidth = col.fixedWidth;
                if (columnIndex == undefined)
                    ctrl.columnDefs.splice(ctrl.columnDefs.length, 0, colDef);//to insert before the actionType column
                else {
                    colDef.columnIndex = columnIndex;
                    var columnIndexToInsert = 0;
                    for (var i = 0; i < ctrl.columnDefs.length + 1; i++) {//+ 1 because the column is not yet added to ctrl.columnDefs 
                        columnIndexToInsert = i;
                        var iColDef = i < ctrl.columnDefs.length ? ctrl.columnDefs[i] : undefined;
                        if (i == columnIndex || (iColDef != undefined && iColDef.columnIndex != undefined && iColDef.columnIndex >= columnIndex)) {
                            break;
                        }

                    }

                    ctrl.columnDefs.splice(columnIndexToInsert, 0, colDef);
                }

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

            function getCellTemplateWithFilter(template, colDef) {
                if (colDef.type == "Number") {
                    var numberPrecision = UISettingsService.getNormalPrecision() || 2;
                    if (colDef.numberPrecision != undefined) {
                        if (colDef.numberPrecision == "NoDecimal")
                            numberPrecision = 0;
                        else if (colDef.numberPrecision == "LongPrecision")
                            numberPrecision = UISettingsService.getUIParameterValue('LongPrecision') || 4;
                        else if (Number.isInteger(colDef.numberPrecision))
                            numberPrecision = parseInt(colDef.numberPrecision);
                    }
                    template = template.replace("#TEXTALIGN#", numbertextDirection + ";" + paddingDirection + ":2px");
                    template = UtilsService.replaceAll(template, "#CELLFILTER#", "| vrtextOrNumber:" + numberPrecision);
                    template = UtilsService.replaceAll(template, "#PERCENTAGE#", "");
                }
                else if (colDef.type == "Progress" || colDef.type == "MultiProgress") {
                    template = template.replace("#TEXTALIGN#", "center;position:absolute;color:#666;");
                    template = UtilsService.replaceAll(template, "#CELLFILTER#", "| vrtextOrNumber:1");
                    template = UtilsService.replaceAll(template, "#PERCENTAGE#", "%");
                }
                else {
                    template = UtilsService.replaceAll(template, "#PERCENTAGE#", "");
                    template = template.replace("#TEXTALIGN#", normaltextDirection);
                    if (colDef.type == "LongDatetime")
                        template = UtilsService.replaceAll(template, "#CELLFILTER#", " | date:'yyyy-MM-dd HH:mm:ss'");
                    else if (colDef.type == "Datetime")
                        template = UtilsService.replaceAll(template, "#CELLFILTER#", " | date:'yyyy-MM-dd HH:mm'");
                    else if (colDef.type == "Date")
                        template = UtilsService.replaceAll(template, "#CELLFILTER#", " | date:'yyyy-MM-dd'");
                    else if (colDef.type == "Yearmonth")
                        template = UtilsService.replaceAll(template, "#CELLFILTER#", " | date:'yyyy-MM'");
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

            function updateColumnHeader(colDef, headerText) {
                colDef.name = headerText;
            }

            function calculateDataColumnsSectionWidth() {
                defineActionTypeColumn();
                var otherSectionsWidth = actionMenuWidth + expandableColumnWidth + draggableRowIconWidth + deleteRowColumnWidth + actionTypeColumnWidth;
                //if (width < 100)
                //    width -= 1;
                ctrl.dataColumnsSectionWidth = "calc(100% - " + otherSectionsWidth + "px)";
            }
            scope.$on("$destroy", function () {
                $(window).unbind('resize', calculateColumnsWidthResize);
            });
            $(window).on('resize', calculateColumnsWidthResize);
            function calculateColumnsWidthResize() {
                setTimeout(function () {
                    calculateColumnsWidth();
                }, 100);
            }
            function calculateColumnsWidth() {
                var totalWidthFactors = 0;
                var totalfixedWidth = 0;
                var normalColCount = 0;
                angular.forEach(ctrl.columnDefs, function (col) {
                    if (col.isHidden != true) {
                        if (col.fixedWidth != undefined) {
                            totalfixedWidth += parseInt(col.fixedWidth);
                        }
                        else {
                            totalWidthFactors += parseInt(col.widthFactor);
                            normalColCount++;
                        }

                    }
                });
                var initialTotalWidth = 100;
                var totalWidth = initialTotalWidth;

                if (ctrl.margin != undefined && innerWidth > 1366) {
                    totalWidth = initialTotalWidth - (ctrl.margin * (innerWidth / 1920));
                }


                //if(ctrl.margin !=undefined &&  innerWidth>1920)
                //   totalWidth = initialTotalWidth - (ctrl.margin * (innerWidth / 1920));

                angular.forEach(ctrl.columnDefs, function (col) {
                    if (col.isHidden != true) {
                        if (col.fixedWidth != undefined) {
                            col.width = col.fixedWidth + 'px';
                        }
                        else {
                            col.width = "calc(" + (totalWidth * col.widthFactor / totalWidthFactors) + "% - " + (totalfixedWidth * (totalWidth * col.widthFactor / totalWidthFactors) / 100) + "px)";
                        }

                    }
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
                    if (itemIndexInUpdatedItems >= 0)
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
                if (ctrl.maxheight != undefined && ctrl.maxheight != '') {
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

                ctrl.onColumnClicked = function (colDef, dataItem, event) {
                    if (colDef.onClickedAttr != undefined)
                        colDef.onClickedAttr(dataItem, colDef, event);
                };
                ctrl.onDescriptionClicked = function (colDef, dataItem) {
                    var modalSettings = {
                        autoclose: true
                    };

                    modalSettings.onScopeReady = function (modalScope) {
                        if (colDef.expendableColumnTitle != undefined)
                            modalScope.messageTitle = eval('dataItem.' + colDef.expendableColumnTitle);

                        if (colDef.expendableColumnDescription != undefined)
                            modalScope.value = eval('dataItem.' + colDef.expendableColumnDescription);
                        else
                            modalScope.value = ctrl.getCellValue(dataItem, colDef);

                        if (modalScope.messageTitle != "" && (modalScope.value == "" || modalScope.value == null)) {
                            modalScope.messageTitle = undefined;
                            modalScope.value = eval('dataItem.' + colDef.expendableColumnTitle);
                        }

                    };

                    VRModalService.showModal("/Client/Javascripts/Directives/DataGrid/ExpendableColumnPopup.html", null, modalSettings);

                };

                var lastSelectedRow;
                ctrl.onRowClicked = function (evnt) {
                    $('.vr-datagrid-row').removeClass('vr-datagrid-datacells-click');
                    if (ctrl.norowhighlightonclick == undefined || !ctrl.norowhighlightonclick) {
                        if (lastSelectedRow != undefined)
                            lastSelectedRow.removeClass('vr-datagrid-datacells-click');
                        $(evnt.target).closest('.vr-datagrid-row').addClass('vr-datagrid-datacells-click');
                    }
                    ctrl.menuLeft = (VRLocalizationService.isLocalizationRTL()) ? evnt.clientX - 90 : evnt.clientX;// evnt.offsetX == undefined ? evnt.originalEvent.layerX : evnt.offsetX;
                    ctrl.menuTop = evnt.clientY;// evnt.offsetY == undefined ? evnt.originalEvent.layerY : evnt.offsetY;

                };
                ctrl.isDraggableColumns = attrs.disabledraggablecolumns == undefined;
                ctrl.headerSortableListener = {
                    handle: '.dragcol',
                    onSort: function (event) {
                        buildRowHtml();
                    }
                };

                ctrl.switchColumnVisibility = function (colDef) {
                    colDef.isHidden = !colDef.isHidden;
                    calculateColumnsWidth();
                };

                ctrl.isExporting = false;
                ctrl.onExportClicked = function () {
                    var promise;
                    if (this.onexport != undefined && typeof (this.onexport) == 'function') {
                        promise = this.onexport();
                    }
                    else {
                        promise = retrieveData(false, true, false, DataGridRetrieveDataEventType.Export);
                    }

                    if (promise != undefined && promise != null) {
                        ctrl.isExporting = true;
                        promise.finally(function () {
                            ctrl.isExporting = false;
                        });
                    }
                };

                ctrl.showExportAction = function () {
                    return (attrs.enableautoexport != undefined) || this.onexport != undefined;
                };

                ctrl.viewSelectionChanged = function () {
                    calculateDataColumnsSectionWidth();
                    fixHeaderLayout();
                };

                ctrl.getCellValue = function (dataItem, colDef) {
                    if (colDef == undefined)
                        return;
                    return eval('dataItem.' + colDef.field);
                };

                ctrl.getCellTooltip = function (dataItem, colDef) {
                    if (colDef == undefined)
                        return;
                    if (colDef.tooltipField != undefined)
                        return eval('dataItem.' + colDef.tooltipField);
                    else if (colDef.field != undefined)
                        return eval('dataItem.' + colDef.field);
                };

                ctrl.getCellClass = function (dataItem, colDef) {
                    if (colDef == undefined)
                        return;
                    if (colDef.getcolor != undefined) {
                        var color = colDef.getcolor(dataItem, colDef);
                        if (typeof color === 'string' || color instanceof String) {
                            return color;
                        }
                        else if (typeof color === 'object' || color instanceof Object) {
                            switch (color.UniqueName) {
                                case "VR_AccountBalance_StyleFormating_CSSClass": return color.ClassName;
                            }
                        }
                    }

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

                ctrl.hasExpendableColumn = function (colDef) {
                    if (colDef == undefined || colDef.expendableColumn == undefined)
                        return false;
                    else
                        return true;
                };

                ctrl.getGridMenuActions = function () {
                    return ctrl.gridmenuactions != undefined ? ctrl.gridmenuactions : [];
                };

                ctrl.executeGridAction = function (menuAction) {
                    if (menuAction.onClicked != undefined) {
                        menuAction.isExecuting = true;
                        UtilsService.convertToPromiseIfUndefined(menuAction.onClicked())
                            .finally(function () {
                                menuAction.isExecuting = false;
                            });
                    }
                };

                ctrl.getRowCSSClass = function (dataItem) {
                    if (ctrl.isdynamicrowstyle)
                        return getRowCSSClass(dataItem);
                    else
                        return dataItem.CssClass;
                };
                var odd = false;
                ctrl.getCellContainerClass = function (dataItem, colDef) {
                    var object = ctrl.getrowstyle(dataItem);
                    return object == null ? colDef && colDef.cssClass : "";
                };

                function fixHeaderLayout() {
                    var div = $(elem).find("#gridBodyContainer")[0];// need real DOM Node, not jQuery wrapper
                    var mh = $(div).css('max-height');
                    var dataSourceLength = ctrl.isMainItemsShown ? ctrl.datasource.length : ctrl.updateItems.length;
                    mh = mh && parseInt(mh.substring(0, mh.length - 1)) || 0;
                    if (dataSourceLength * 25 < mh || dataSourceLength == 0 || mh == 0) {
                        $(div).css({ "overflow-y": 'auto', "overflow-x": 'hidden' });

                        if (VRLocalizationService.isLocalizationRTL())
                            ctrl.headerStyle = {
                                "padding-left": "0px"
                            };
                        else
                            ctrl.headerStyle = {
                                "padding-right": "0px"
                            };
                    }
                    else {
                        $(div).css({ "overflow-y": 'auto', "overflow-x": 'hidden' });
                        if (VRLocalizationService.isLocalizationRTL())
                            ctrl.headerStyle = {
                                "padding-left": getScrollbarWidth() + "px"
                            };
                        else
                            ctrl.headerStyle = {
                                "padding-right": getScrollbarWidth() + "px"
                            };
                    }
                };

                function getRowCSSClass(dataItem) {
                    if (ctrl.getrowstyle != undefined && typeof (ctrl.getrowstyle) == 'function') {
                        var object = ctrl.getrowstyle(dataItem);
                        if (object != null)
                            return object.CssClass;
                    }
                }

                var datasourceWatch = scope.$watchCollection('ctrl.datasource', onDataSourceChanged);
                var updateItems = scope.$watchCollection('ctrl.updateItems', onDataSourceChanged);

                scope.$on('$destroy', function () {
                    datasourceWatch();
                    updateItems();
                    ctrl.datasource.length = 0;
                });

                function onDataSourceChanged(newDataSource, oldNames) {
                    for (var i = 0; i < newDataSource.length; i++) {
                        var dataItem = newDataSource[i];
                        if (!dataItem.isColumnValuesFilled) {
                            for (var j = 0; j < ctrl.columnDefs.length; j++) {
                                var colDef = ctrl.columnDefs[j];
                                filldataItemColumnValues(dataItem, colDef);
                                dataItem.CssClass = getRowCSSClass(dataItem);
                            }
                            dataItem.isColumnValuesFilled = true;
                        }
                    }
                    if (newDataSource.length != oldNames.length)
                        fixHeaderLayout();
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
                    scope.$apply();
                }, 100);
            }

            function buildRowHtml() {
                ctrl.rowHtml = '';
                var gridvalue;
                for (var i = 0; i < ctrl.columnDefs.length; i++) {
                    var currentColumn = ctrl.columnDefs[i];
                    var currentColumnHtml = '$parent.ctrl.columnDefs[' + i + ']';
                    var dataItemColumnPropertyPath = "dataItem.columnsValues." + getDataItemColumnProperty(currentColumn);

                    ctrl.rowHtml += '<div ng-if="!' + currentColumnHtml + '.isHidden" ng-style="{ \'width\': ' + currentColumnHtml + '.width, \'display\':\'inline-flex\', \'vertical-align\':\'top\'' + (i != 0 ? (',\'border-left\': \'' + currentColumn.borderRight) + '\'' : '') + '}"" ng-class="ctrl.getCellContainerClass(dataItem, ' + currentColumnHtml + ')" class="vr-datagrid-cell-container">';
                    if (currentColumn.type == "MultiProgress") {
                        var values = currentColumn.field.split("|");
                        ctrl.rowHtml += '<vr-progressbar gridvalue="';
                        for (var j = 0; j < values.length; j++) {
                            ctrl.rowHtml += ('{{::dataItem.' + values[j] + '}}');
                            if (j < values.length - 1)
                                ctrl.rowHtml += "|";

                        }
                        ctrl.rowHtml += '"></vr-progressbar></div>';

                    }
                    else if (currentColumn.type == "Progress") {
                        gridvalue = "{{::" + dataItemColumnPropertyPath + ".dataValue}}";
                        ctrl.rowHtml += '<vr-progressbar gridvalue="' + gridvalue + '" ></vr-progressbar></div>';
                    } else {
                        var tooltipFilter = "";
                        if (currentColumn.type == "Number") {
                            var numberPrecision = UISettingsService.getNormalPrecision() || 2;
                            if (currentColumn.numberPrecision == "NoDecimal")
                                numberPrecision = 0;
                            else if (currentColumn.numberPrecision == "LongPrecision")
                                numberPrecision = UISettingsService.getUIParameterValue('LongPrecision') || 4;
                            tooltipFilter = " | vrtextOrNumber:" + numberPrecision;
                        }
                        else {
                            if (currentColumn.type == "LongDatetime")
                                tooltipFilter = " | date:'yyyy-MM-dd HH:mm:ss'";
                            else if (currentColumn.type == "Datetime")
                                tooltipFilter = " | date:'yyyy-MM-dd HH:mm'";
                            else if (currentColumn.type == "Date")
                                tooltipFilter = " | date:'yyyy-MM-dd'";
                            else if (currentColumn.type == "Yearmonth")
                                tooltipFilter = " | date:'yyyy-MM'";

                        }

                        var cellTemplate = currentColumn.cellTemplate;
                        if (ctrl.hasExpendableColumn(currentColumn))
                            cellTemplate = UtilsService.replaceAll(cellTemplate, "#CELLCONTENT#", cellTemplateExpendableContent);
                        else if (currentColumn.isClickableAttr != undefined)
                            cellTemplate = UtilsService.replaceAll(cellTemplate, "#CELLCONTENT#", cellTemplateClickableContent);
                        else
                            cellTemplate = UtilsService.replaceAll(cellTemplate, "#CELLCONTENT#", cellTemplateNormalContent);

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
                        cellTemplate = UtilsService.replaceAll(cellTemplate, "#TOOLTIPFILTER#", tooltipFilter);

                        cellTemplate = UtilsService.replaceAll(cellTemplate, "colDef", currentColumnHtml);
                        cellTemplate = getCellTemplateWithFilter(cellTemplate, currentColumn);
                        ctrl.rowHtml += '<div class="vr-datagrid-cell">'
                            + '    <div class="vr-datagrid-celltext ">'
                           + cellTemplate
                         + '</div>'
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

            function setMainItemsViewVisible() {
                ctrl.selectedView = 0;
                ctrl.isMainItemsShown = true;
                ctrl.viewSelectionChanged();
            }

            function setUpdatedItemsViewVisible() {
                ctrl.selectedView = 1;
                ctrl.isMainItemsShown = false;
                ctrl.viewSelectionChanged();
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
                    setUpdatedItemsViewVisible();
                    ctrl.expandRow(item);
                };

                gridApi.itemUpdated = function (item) {
                    item.isUpdated = true;
                    itemChanged(item, "Updated");
                    //ctrl.expandRow(item);
                };

                gridApi.itemDeleted = function (item) {
                    item.isDeleted = true;
                    itemChanged(item, "Deleted");
                };
                gridApi.clearDataSource = function () {
                    ctrl.datasource.length = 0;
                    ctrl.pagerSettings.totalDataCount = 0;
                };
                gridApi.clearAll = function () {
                    ctrl.datasource.length = 0;
                    ctrl.pagerSettings.totalDataCount = 0;
                    ctrl.summaryDataItem = undefined;
                    buildSummaryRowHtml();
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
                    });
                    addBatchItemsToSource(itemsToAdd);
                };

                gridApi.addItemsToBegin = function (items) {
                    var itemsToAdd = [];//create a new array to avoid changing the original items
                    angular.forEach(items, function (itm) {
                        itemsToAdd.unshift(itm);
                    });
                    addBatchItemsToBeginSource(itemsToAdd);
                };

                gridApi.showLoader = function () {
                    ctrl.isLoadingMoreData = true;
                };

                gridApi.hideLoader = function () {
                    ctrl.isLoadingMoreData = false;
                };

                gridApi.refreshGrid = function () {
                    setMainItemsViewVisible();
                    retrieveDataResultKey = null;
                    return retrieveData(false, false, false, DataGridRetrieveDataEventType.ExternalTrigger);
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
                    var numberOfItems = pagingOnScrollEnabled ? getPageSize() : 50;//if paging on scroll is enabled, take the page size
                    for (var i = 0; i < numberOfItems; i++) {
                        if (items.length > 0) {
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
                    setMainItemsViewVisible();
                    retrieveDataInput = {
                        Query: query
                    };
                    return retrieveData(true, false, false, DataGridRetrieveDataEventType.ExternalTrigger);
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

                gridApi.clearUpdatedItems = function () {
                    if (ctrl.updateItems != undefined) {
                        ctrl.isMainItemsShown = true;
                        ctrl.updateItems.length = 0;
                    }
                };

                gridApi.getPageSize = function () {
                    if (pagingOnScrollEnabled)
                        return getPageSize();
                    else if (ctrl.showPager)
                        return ctrl.pagerSettings.itemsPerPage;
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

                    colValuesObj.expendableColumn = ctrl.hasExpendableColumn(colDef);

                    if (dataItem.columnsValues == undefined)
                        dataItem.columnsValues = {};

                    dataItem.columnsValues[getDataItemColumnProperty(colDef)] = colValuesObj;
                }
                catch (ex) {

                }
            }

            function fillSummaryDataItemColumnValues(colDef) {
                try {
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
                //ctrl.gridStyle['min-height'] = "60px";
                ctrl.gridStyle['overflow-y'] = "auto";
                ctrl.gridStyle['overflow-x'] = "hidden";
            }

            function definePagingOnScroll($scope, loadMoreDataFunction) {

                ctrl.isLoadingMoreData = false;
                if (loadMoreDataFunction != undefined) {
                    pagingOnScrollEnabled = true;
                    if (ctrl.maxheight != undefined && ctrl.maxheight != '') {
                        setMaxHeight(ctrl.maxheight);
                    }

                    else {
                        var sh = innerHeight;
                        var h = 28;
                        if (isInModal() == true)
                            h += sh * 0.5;
                        else
                            h += sh - 332;

                        h = h < 30 ? 30 : h;
                        setMaxHeight(h + "px");
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
                            });
                        }
                    });

                };
            }
            function isInModal() {
                return ($(elem).find("#gridBodyContainer").parents('.modal-body').length > 0);
            }

            function getPageSize() {
                var h;
                if (isInModal() == true)
                    h = innerHeight * 0.3;
                else
                    h = innerHeight * 0.55;

                var pagesize = (Math.ceil(parseInt((h / 25) * 1.5) / 10) * 10) < 30 ? 30 : (Math.ceil(parseInt((h / 25) * 1.5) / 10) * 10);
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
                    if (dataItem.isDeleted)
                        return false;
                    if (ctrl.showexpand != undefined && typeof (ctrl.showexpand) == 'function') {
                        var showExpand = ctrl.showexpand(dataItem);
                        return showExpand;
                    }

                    return true;

                };
                ctrl.expandRow = function (dataItem) {
                    if (expandableRowTemplate != undefined) {
                        dataItem.expandableRowTemplate = expandableRowTemplate;
                        dataItem.isRowExpanded = true;
                    }
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
                    return (actions != undefined && actions != null && actions.length > 0);
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

                var menuActionsArrays = [];

                ctrl.getMenuActions = function (dataItem) {

                    if (dataItem.menuActionObj == undefined) {
                        dataItem.menuActionObj = {};

                        var arrayOfActions = (typeof (actionsAttribute) == 'function' ? actionsAttribute(dataItem) : actionsAttribute);
                        if (arrayOfActions != undefined) {
                            checkMenuActionPermission(arrayOfActions, dataItem);
                        }
                        dataItem.menuActionObj.menuActions = arrayOfActions;
                    }

                    return dataItem.menuActionObj.menuActions;

                    function checkMenuActionPermission(arrayOfActions, dataItem) {
                        var indexOfActionsInArray = menuActionsArrays.indexOf(arrayOfActions);
                        if (indexOfActionsInArray < 0) {
                            for (var i = 0; i < arrayOfActions.length; i++) {
                                invokeHasPermission(arrayOfActions[i], dataItem);
                            }
                            menuActionsArrays.push(arrayOfActions);
                        }

                        function invokeHasPermission(menuAction, dataItem) {
                            if (menuAction.haspermission == undefined || menuAction.haspermission == null) {
                                return;
                            }
                            menuAction.disable = true;
                            UtilsService.convertToPromiseIfUndefined(menuAction.haspermission(dataItem)).then(function (isAllowed) {
                                if (isAllowed) {
                                    menuAction.disable = false;
                                }
                            });
                        }
                    }
                };



                ctrl.menuActionClicked = function (action, dataItem) {
                    action.clicked(dataItem, gridApi);
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

            //function defineRetrieveData(retrieveDataFunc, pagingType, defaultSortDirection_local) {
            function defineRetrieveData(retrieveDataFunc, pagingType) {
                retrieveDataFunction = retrieveDataFunc;
                //defaultSortDirection = defaultSortDirection_local;

                var retrieveDataOnPaging = function () {
                    if (retrieveDataInput == undefined)
                        return;
                    return retrieveData(false, false, false, DataGridRetrieveDataEventType.Paging);
                };
                switch (pagingType) {
                    case "Pager":
                        ctrl.showPager = true;
                        ctrl.pagerSettings = {
                            currentPage: 1,
                            totalDataCount: 0,
                            pageChanged: retrieveDataOnPaging
                        };
                        break;
                    case "PagingOnScroll":
                        definePagingOnScroll(scope, retrieveDataOnPaging);
                        break;
                }
            }

            function retrieveData(clearBeforeRetrieve, isExport, isSorting, eventType) {
                if (!isGridReady)
                    return;

                var defaultSortDirection;
                if (attrs.defaultsortdirection != undefined)
                    defaultSortDirection = scope.$parent.$eval(attrs.defaultsortdirection);

                var defaultSortByFieldName;
                if (attrs.defaultsortbyfieldname != undefined)
                    defaultSortByFieldName = scope.$parent.$eval(attrs.defaultsortbyfieldname);

                if (clearBeforeRetrieve) {
                    retrieveDataResultKey = null;

                    if (defaultSortByFieldName != undefined) {
                        sortColumn = undefined;
                    }
                    else {
                        for (var colIndex = 0; colIndex < ctrl.columnDefs.length && sortColumn == undefined; colIndex++) {
                            sortColumn = ctrl.columnDefs[colIndex];
                            if (sortColumn.isHidden)
                                sortColumn = undefined;
                        }
                    }

                    sortDirection = defaultSortDirection != undefined ? defaultSortDirection : "ASC";
                }
                if (clearBeforeRetrieve || isSorting) {
                    if (ctrl.showPager)
                        ctrl.pagerSettings.currentPage = 1;
                }
                ctrl.exportIsClickable = retrieveDataInput != undefined;
                if (retrieveDataInput == undefined)
                    return;
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
                    if (ctrl.showPager && ctrl.pagerSettings != undefined)
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

                        if (response != undefined) {
                            gridApi.addItemsToSource(response.Data);//response should be of type Vanrise.Entities.BigResult<T>
                            retrieveDataResultKey = response.ResultKey;
                            if (ctrl.pagerSettings != undefined && ctrl.pagerSettings != null)
                                ctrl.pagerSettings.totalDataCount = response.TotalCount;
                        }
                    }
                };

                var retrieveDataContext = { eventType: eventType, isDataSorted: sortColumn != undefined };
                var promise = retrieveDataFunction(retrieveDataInput, onResponseReady, retrieveDataContext);//this function should return a promise in case it is getting data

                if (promise != undefined && promise != null) {
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

