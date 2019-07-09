﻿'use strict';

app.directive('vrDatagrid', ['UtilsService', 'SecurityService', 'DataRetrievalResultTypeEnum', '$compile', 'VRModalService', 'DataGridRetrieveDataEventType', 'UISettingsService', 'VRLocalizationService', 'MobileService', 'VRDataGridService',
	function (UtilsService, SecurityService, DataRetrievalResultTypeEnum, $compile, VRModalService, DataGridRetrieveDataEventType, UISettingsService, VRLocalizationService, MobileService, VRDataGridService) {

	    var paddingDirection = VRLocalizationService.isLocalizationRTL() ? "'padding-left'" : "'padding-right'";
	    var normaltextDirection = VRLocalizationService.isLocalizationRTL() ? "right" : "left";
	    var numbertextDirection = "right";
	    var isMobileView = MobileService.isMobile();
	    var columnVisibilities = [];
	    var baseColumnVisibilities = [];
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
	            norowhighlightonhover: '=',
	            gridmenuactions: '=',
	            margin: '=',
	            dragdropsetting: '=',
	            mobilegridlayout: '=',
	            alternativecolor: '=',
	            rowviewsetting: '='
	        },
	        controller: function ($scope, $element, $attrs) {
	            columnVisibilities = [];
	            baseColumnVisibilities = [];
	            $scope.$on("$destroy", function () {
	                $('.vr-grid-menu').parents('div').unbind('scroll', hideGridColumnsMenu);
	                $(window).unbind('scroll', hideGridColumnsMenu);
	                $(document).unbind('click', bindClickOutSideGridMenu);
	                $element.remove();
	                if (ctrl.objectType) {
	                    VRDataGridService.removeObjectType(ctrl.objectType);
	                }
	            });
	            var ctrl = this;
	            ctrl.isMobile = MobileService.isMobile();
	            ctrl.isRtl = VRLocalizationService.isLocalizationRTL();
	            if (ctrl.rowviewsetting != undefined) {
	                ctrl.expandAsfullScreen = ctrl.rowviewsetting.expandAsFullScreen == true;
	                if (ctrl.rowviewsetting.objectType) {
	                    ctrl.objectType = {
	                        objectTypeName: ctrl.rowviewsetting.objectType
	                    };
	                }
	                ctrl.viewDirective = ctrl.rowviewsetting.viewDirective || undefined;
	                ctrl.directiveSettings = ctrl.rowviewsetting.directiveSettings || undefined;
	            }

	            ctrl.mobileGridView = ctrl.mobilegridlayout != undefined ? ctrl.mobilegridlayout : false;
	            ctrl.mobileSwitchBtn = true;
	            ctrl.itemsSortable = {
	                handle: '.handeldrag', animation: 150
	            };
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
	                        obj = ctrl.dragdropsetting.onItemReceived(evt.model, evt.models, evt.source);
	                    evt.models[evt.newIndex] = obj;
	                };
	            }


	            ctrl.pagerReadyPromiseDefferred = undefined;

	            ctrl.pagerSettingOnReady = function () {
	                ctrl.pagerReadyPromiseDefferred.resolve();
	            };


	            ctrl.layoutOption = UISettingsService.getGridLayoutOptions();
	            if (ctrl.alternativecolor != undefined)
	                ctrl.layoutOption.alternativeColor = ctrl.alternativecolor;

	            if (ctrl.isMobile)
	                ctrl.layoutOption.verticalLine = false;

	            ctrl.readOnly = UtilsService.isContextReadOnly($scope) || $attrs.readonly != undefined;
	            ctrl.rowClickDetails = false;
	            if ($attrs.rowclickdetails != undefined)
	                ctrl.rowClickDetails = true;

	            var loadMoreDataFunction;
	            if ($attrs.loadmoredata != undefined)
	                loadMoreDataFunction = $scope.$parent.$eval($attrs.loadmoredata);

	            var retrieveDataFunction;
	            if ($attrs.dataretrievalfunction != undefined)
	                retrieveDataFunction = $scope.$parent.$eval($attrs.dataretrievalfunction);

	            var pagingType;
	            if ($attrs.pagingtype != undefined)
	                pagingType = $scope.$parent.$eval($attrs.pagingtype);
	            if (ctrl.isMobile)
	                pagingType = "PagingOnScroll";

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
	            ctrl.openGridMenu = function () {
	                if (ctrl.isMobile) {
	                    var modalSettings = {
	                        autoclose: true
	                    };
	                    modalSettings.onScopeReady = function (modalScope) {
	                        modalScope.ctrl = ctrl;
	                    };
	                    VRModalService.showModal("/Client/Javascripts/Directives/DataGrid/GridMenuActionModalPopup.html", null, modalSettings);
	                }
	            };

	            ctrl.openGridSortingMenuOptions = function () {
	                if (ctrl.isMobile) {
	                    var modalSettings = {
	                        autoclose: true
	                    };
	                    modalSettings.onScopeReady = function (modalScope) {
	                        modalScope.ctrl = ctrl;
	                    };
	                    VRModalService.showModal("/Client/Javascripts/Directives/DataGrid/SortByColumnsModalPopup.html", null, modalSettings);
	                }
	            };


	            ctrl.toggelGridMenu = function (e) {
	                var self = angular.element(e.currentTarget);

	                var menu = self.parent().find('.vr-grid-menu')[0];
	                if (ctrl.showgmenu == false) {
	                    setTimeout(function () {
	                        var selfHeight = $(self).height();
	                        var selfOffset = $(self).offset();
	                        var left = selfOffset.left + 14;
	                        if (VRLocalizationService.isLocalizationRTL())
	                            left -= 174;
	                        $(menu).css({
	                            display: 'block'
	                        });
	                        $(menu).addClass("open-grid-menu");
	                        $(menu).css({
	                            position: 'fixed', top: selfOffset.top - $(window).scrollTop() + 5, left: left
	                        });
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
	                menu.css({
	                    display: 'none'
	                });
	                $('out-div').removeClass("open-grid-menu");
	                if (ctrl.showgmenu == true) {
	                    ctrl.showgmenu = false;
	                    $scope.$root.$digest();
	                }

	            };
	            ctrl.hidePagingInfo = ($attrs.hidepaginginfo != undefined);
	            ctrl.rotateHeader = true;
	            ctrl.cellLayoutStyle = $attrs.normalcell != undefined ? {
	                'white-space': 'normal'
	            } : { 'white-space': 'pre' };

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

	    var cellTemplateNormalContent = '<span ng-style="::$parent.ctrl.cellLayoutStyle" ng-class="#CELLCLASS#">{{#CELLVALUE# #CELLFILTER#}}#PERCENTAGE#</span>';
	    var cellTemplateClickableContent = '<a ng-class="#CELLCLASS#" ng-click="$parent.ctrl.onColumnClicked(colDef, dataItem , $event)" style="cursor:pointer;"> {{#CELLVALUE# #CELLFILTER#}}#PERCENTAGE#</a>';
	    var cellTemplateExpendableContent = '<a ng-class="#CELLCLASS#" ng-click="$parent.ctrl.onDescriptionClicked(colDef, dataItem)" style="cursor:pointer;"> {{#CELLVALUE# #CELLFILTER#}}#PERCENTAGE#</a>';

	    var cellTemplate = '<div style="text-align: #TEXTALIGN#;width: 100%;overflow: hidden;text-overflow: ellipsis;white-space: nowrap;" title="{{#CELLTOOLTIP# #TOOLTIPFILTER#}}" >'
			+ '#CELLCONTENT#'
			+ '</div>';

	    var summaryCellTemplate = '<div style="text-align: #TEXTALIGN#;width: 100%;overflow: hidden;text-overflow: ellipsis;white-space: nowrap;" title="{{#COLUMNVALUES#.dataValue #SUMMARYTOOLTIPFILTER#}}" >'
			+ ''
			+ '<span class="span-summary" style="font-size:13px" ng-style="::$parent.ctrl.cellLayoutStyle"> {{#COLUMNVALUES#.dataValue #CELLFILTER#}}</span>'
			+ ''
			+ '</div>';

	    var onSortClick = isMobileView ? "" : "ng-click=\"colDef.onSort()\"";

	    var headerTemplate = '<div ' + onSortClick + ' class="vr-datagrid-header-cell" >'
			+ ' <div col-index="renderIndex" style="width:100%">'
			+ '   <div class="vr-datagrid-celltext" style="overflow: hidden;"  ng-class="::colDef.textAlignmentClass" title="{{colDef.description}}" >'
			+ '    <span ng-show="colDef.sortDirection==\'ASC\'">&uarr;</span>'
			+ '    <span ng-show="colDef.sortDirection==\'DESC\'">&darr;</span>'
			+ '<span ng-if="!colDef.rotateHeader" style="white-space: pre;"> {{colDef.name}}</span>'
			+ '<p ng-if="colDef.rotateHeader" class="vr-rotate-header" style="white-space: pre;" >{{colDef.name}}</p>'
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
	                cssClass: col.cssclass,
	                sysName: col.sysName,
	                listViewWidth: col.listViewWidth,
	                disableSorting: col.disableSorting
	            };

	            var colSys = getColumnBySysName(colDef.sysName);
	            if (colSys != null)
	                ctrl.setColumnVisibilitiesArray(colSys, colDef);



	            lastAddedColumnId++;
	            colDef.columnId = lastAddedColumnId;
	            colDef.filter = getColumnFilter(colDef);
	            colDef.textAlignmentClass = colDef.type == "Number" ? 'vr-grid-cell-number' : '';
	            if (col.onSortChanged == undefined) {
	                col.onSortChanged = function (colDef_internal, sortDirection_internal) {
	                    sortColumn = colDef_internal;
	                    sortDirection = sortDirection_internal;
	                    return retrieveData(false, false, true, DataGridRetrieveDataEventType.Sorting);
	                };
	            }

	            colDef.onSort = function (direction) {
	                if (col.onSortChanged != undefined && col.disableSorting == false) {
	                    var sortDirection = "";
	                    if (direction != undefined)
	                        sortDirection = direction;
	                    else
	                        sortDirection = colDef.sortDirection != "ASC" ? "ASC" : "DESC";
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

	        function getColumnBySysName(sysNameValue) {
	            var colSys = UtilsService.getItemByVal(baseColumnVisibilities, sysNameValue, 'SysName');
	            return colSys;
	        }

	        function getCellTemplateWithFilter(template, colDef) {

	            if (colDef.type == "Progress" || colDef.type == "MultiProgress") {
	                template = template.replace("#TEXTALIGN#", "center;position:absolute;color:#666;");
	                template = UtilsService.replaceAll(template, "#CELLFILTER#", "| vrtextOrNumber:1");
	                template = UtilsService.replaceAll(template, "#PERCENTAGE#", "%");
	            }
	            else {
	                if (colDef.type == "Number") {
	                    template = template.replace("#TEXTALIGN#", numbertextDirection + ";" + paddingDirection + ":2px");
	                    template = UtilsService.replaceAll(template, "#PERCENTAGE#", "");
	                }
	                template = UtilsService.replaceAll(template, "#PERCENTAGE#", "");
	                template = template.replace("#TEXTALIGN#", normaltextDirection);
	                template = UtilsService.replaceAll(template, "#CELLFILTER#", colDef.filter);
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
	            var visibleColCount = 0;
	            angular.forEach(ctrl.columnDefs, function (col) {
	                if (col.isHidden != true) {
	                    if (col.fixedWidth != undefined) {
	                        totalfixedWidth += parseInt(col.fixedWidth);
	                    }
	                    else {
	                        totalWidthFactors += parseInt(col.widthFactor);
	                        normalColCount++;
	                    }
	                    visibleColCount++;
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
	                    var valueWidth = 0;
	                    if (col.fixedWidth != undefined) {
	                        col.width = col.fixedWidth + 'px';
	                        valueWidth = col.fixedWidth;
	                    }
	                    else {
	                        col.width = "calc(" + (totalWidth * col.widthFactor / totalWidthFactors) + "% - " + (totalfixedWidth * (totalWidth * col.widthFactor / totalWidthFactors) / 100) + "px)";

	                        var colFixWidth = (1366 - totalfixedWidth) * ((totalWidth * col.widthFactor / totalWidthFactors) / 100);
	                        var maxColFixWidth = (1920 - totalfixedWidth) * ((totalWidth * col.widthFactor / totalWidthFactors) / 100);
	                        if (colFixWidth > innerWidth)
	                            colFixWidth = (innerWidth - totalfixedWidth) * ((totalWidth * col.widthFactor / totalWidthFactors) / 100);
	                        if (maxColFixWidth > innerWidth)
	                            colFixWidth = innerWidth * (totalWidth * col.widthFactor / totalWidthFactors) / 100;

	                        if (col.type == "Number") {
	                            var numberPrecision = UISettingsService.getNormalPrecision() || 2;
	                            if (col.numberPrecision == "NoDecimal")
	                            { colFixWidth = 65; }
	                            else if (col.numberPrecision == "LongPrecision")
	                            { colFixWidth = 65; }
	                            else {
	                                colFixWidth = 65;
	                            }
	                        }
	                        else {
	                            if (col.type == "LongDatetime")
	                            { colFixWidth = 120; }
	                            else if (col.type == "Datetime")
	                            { colFixWidth = 100; }
	                            else if (col.type == "Date") {
	                                colFixWidth = 80;
	                            }
	                            else if (col.type == "Yearmonth") {
	                                colFixWidth = 50;
	                            }
	                        }
	                        valueWidth = colFixWidth;
	                    }

	                    if (col.listViewWidth != undefined) {
	                        valueWidth = col.listViewWidth;
	                    }
	                    //var labelwidth = UtilsService.getHtmlStringWidth(col.name, '12px Open Sans');
	                    //var totalwidth = valueWidth + parseInt(labelwidth);
	                    //var unitFactor = innerWidth;
	                    //var unitCeildWidth = (Math.ceil(parseInt(valueWidth) / unitFactor));
	                    //var parsedWidth = unitFactor * UtilsService.getUnitCeildWidthNextStepValue(valueWidth);
	                    col.mobileWidth = valueWidth + "px";
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
	            ctrl.gridStyle = {
	            };
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

	            ctrl.onRowClicked = function (evnt, dataItem) {
	                if (dataItem.fullScreenMode == true && ctrl.expandAsfullScreen == true) {
	                    scope.showMenu = false;
	                    return;
	                }
	                $('.vr-datagrid-row').removeClass('vr-datagrid-datacells-click');
	                if (ctrl.norowhighlightonclick == undefined || !ctrl.norowhighlightonclick) {
	                    if (lastSelectedRow != undefined)
	                        lastSelectedRow.removeClass('vr-datagrid-datacells-click');
	                    $(evnt.target).closest('.vr-datagrid-row').addClass('vr-datagrid-datacells-click');
	                }
	                var self = angular.element(evnt.currentTarget);
	                var menu = self.find('.gid-cell-menu')[0];
	                var menuTop = evnt.clientY;
	                var menuLeft = evnt.clientX;
	                $(menu).hide();
	                setTimeout(function () {
	                    var self = angular.element(evnt.currentTarget);
	                    ctrl.toLeftDirection = false;
	                    var menuHeigth = $(menu).height();
	                    if (innerHeight - menuTop - 30 < menuHeigth) {
	                        menuTop -= menuHeigth;
	                        ctrl.toTopDirection = true;
	                    }
	                    if (innerWidth - menuLeft < 160) {
	                        ctrl.toLeftDirection = true;
	                    }
	                    if (VRLocalizationService.isLocalizationRTL()) {
	                        ctrl.toLeftDirection = true;
	                        if (menuLeft < 160)
	                            ctrl.toLeftDirection = false;
	                    }
	                    ctrl.menuLeft = (VRLocalizationService.isLocalizationRTL()) ? evnt.clientX - 90 : evnt.clientX;// evnt.offsetX == undefined ? evnt.originalEvent.layerX : evnt.offsetX;
	                    ctrl.menuTop = menuTop;
	                    scope.$apply();
	                    $(menu).show();
	                }, 1);
	            };

	            ctrl.setRowActionMenuDirection = function () {
	                if (ctrl.isRtl == true)
	                    ctrl.toLeftDirection = false;
	                else
	                    ctrl.toLeftDirection = true;
	            };

	            ctrl.onGroupMenuActionHover = function (evnt, action) {
	                ctrl.toTopDirection = false;
	                ctrl.visibleActionCount;
	                var menuTop = evnt.clientY;
	                ctrl.visibleActionCount = getVisibleMenuItemActionCount(action.childsactions);
	                if (innerHeight - menuTop - 30 < (ctrl.visibleActionCount * 17)) {
	                    ctrl.toTopDirection = true;
	                }
	                action.showChilsMenu = true;
	            };


	            function getVisibleMenuItemActionCount(childsactions) {
	                var visibleaction = childsactions.filter(function (action) {
	                    return !action.disable;
	                });
	                return visibleaction.length;
	            }

	            ctrl.getGroupMenuActionItem = function (dataItem, action) {
	                var actionsItemAttribute = action.childsactions;
	                return actionsItemAttribute;
	            };

	            ctrl.hasVisibleMenuAction = function (actions) {
	                var haseEnabledActions = actions.some(function (action) {
	                    return !action.disable;
	                });
	                return haseEnabledActions;
	            };
	            ctrl.openRowContextMenuPopup = function (evnt, dataItem) {
	                if (ctrl.isMobile) {
	                    var modalSettings = {
	                        autoclose: true
	                    };
	                    modalSettings.onScopeReady = function (modalScope) {
	                        modalScope.ctrl = ctrl;
	                        modalScope.dataItem = dataItem;
	                    };
	                    VRModalService.showModal("/Client/Javascripts/Directives/DataGrid/RowActionModalPopup.html", null, modalSettings);
	                }
	            };

	            var updateRowIndex = function (origin, destination) {
	                var temp = ctrl.datasource[destination];
	                ctrl.datasource[destination] = ctrl.datasource[origin];
	                ctrl.datasource[origin] = temp;
	            };

	            ctrl.moveRowUp = function (index) {
	                updateRowIndex(index, index - 1);
	            };

	            ctrl.moveRowDown = function (index) {
	                updateRowIndex(index, index + 1);
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
	                updateColumnVisibilitiesData(colDef, !colDef.isHidden);
	                calculateColumnsWidth();
	            };
	            function updateColumnVisibilitiesData(colDef, isVisible) {
	                var index = UtilsService.getItemIndexByVal(columnVisibilities, colDef.sysName, 'SysName');
	                if (index >= 0)
	                    columnVisibilities.splice(index, 1);
	                else
	                    columnVisibilities.push({
	                        SysName: colDef.sysName,
	                        IsVisible: isVisible
	                    });
	            }

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
	                var cellValue = eval('dataItem.' + colDef.field);
	                if (cellValue != undefined && cellValue != null) return cellValue;
	                return ' ';
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

	            ctrl.setColumnVisibilitiesArray = function (colItem, colDef) {
	                if (colItem.IsVisible == true) {
	                    ctrl.showColumn(colDef);
	                }
	                else
	                    ctrl.hideColumn(colDef);
	                columnVisibilities.push(colItem);
	            };
	            var odd = false;
	            ctrl.getCellContainerClass = function (dataItem, colDef) {
	                var object = ctrl.getRowCSSClass(dataItem);
	                if (object != null)
	                    return;

	                return colDef && colDef.cssClass;
	            };

	            function fixHeaderLayout() {
	                var div = $(elem).find("#gridBodyContainer")[0];// need real DOM Node, not jQuery wrapper				   
	                var mh = $(div).css('max-height');
	                var dataSourceLength = ctrl.isMainItemsShown ? ctrl.datasource.length : ctrl.updateItems.length;
	                mh = mh && parseInt(mh.substring(0, mh.length - 1)) || 0;
	                if (dataSourceLength * 25 < mh || dataSourceLength == 0 || mh == 0) {
	                    $(div).css({
	                        "overflow-y": 'auto', "overflow-x": 'hidden'
	                    });

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
	                    $(div).css({
	                        "overflow-y": 'auto', "overflow-x": 'hidden'
	                    });
	                    if (VRLocalizationService.isLocalizationRTL())
	                        ctrl.headerStyle = {
	                            "padding-left": getScrollbarWidth() + "px"
	                        };
	                    else
	                        ctrl.headerStyle = {
	                            "padding-right": getScrollbarWidth() + "px"
	                        };
	                }
	                // handle normal cell grid case 
	                setTimeout(function () {
	                    var divBody = $(elem).find("#gridBody")[0];
	                    var divbodyHeight = $(divBody).height();
	                    if (divbodyHeight > mh && mh > 0) {
	                        $(div).css({
	                            "overflow-y": 'auto', "overflow-x": 'hidden'
	                        });
	                        if (VRLocalizationService.isLocalizationRTL())
	                            ctrl.headerStyle = {
	                                "padding-left": getScrollbarWidth() + "px"
	                            };
	                        else
	                            ctrl.headerStyle = {
	                                "padding-right": getScrollbarWidth() + "px"
	                            };
	                    }
	                }, 1);
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
	                    dataItem.rowIndex = i;
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
	                var tooltipFilter = "";
	                var currentColumn = ctrl.columnDefs[i];
	                var currentColumnHtml = '$parent.ctrl.columnDefs[' + i + ']';
	                var dataItemColumnPropertyPath = "dataItem.columnsValues." + getDataItemColumnProperty(currentColumn);
	                var cellWidth = ctrl.isMobile ? 'mobileWidth' : 'width';
	                ctrl.rowHtml += '<div ng-if="!' + currentColumnHtml + '.isHidden" ng-style="{ \'width\': ' + currentColumnHtml + '.' + cellWidth + ', \'display\':\'inline-flex\',\'vertical-align\':\'top\'' + (i != 0 ? (',\'border-left\': \'' + currentColumn.borderRight) + '\'' : '') + '}"" ng-class="ctrl.getCellContainerClass(dataItem, ' + currentColumnHtml + ')" class="vr-datagrid-cell-container {{::ctrl.mobileGridView ?\'grid-view-cell\':\'list-view-cell\'}}">';
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
	                    tooltipFilter = currentColumn.filter;
	                    var mobileStyle = "{width:" + currentColumnHtml + ".mobileWidth}";

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
	                        cellTemplate = UtilsService.replaceAll(cellTemplate, "#CELLVALUE#", "::(" + dataItemColumnPropertyPath + ".dataValue!=null && " + dataItemColumnPropertyPath + ".dataValue!= undefined ) ? " + dataItemColumnPropertyPath + ".dataValue : ' '");
	                        cellTemplate = UtilsService.replaceAll(cellTemplate, "#CELLTOOLTIP#", "::" + dataItemColumnPropertyPath + ".tooltip");
	                        cellTemplate = UtilsService.replaceAll(cellTemplate, "#CELLCLASS#", "::" + dataItemColumnPropertyPath + ".cellClass");
	                        cellTemplate = UtilsService.replaceAll(cellTemplate, "#ISCLICKABLE#", dataItemColumnPropertyPath + ".isClickable");
	                    }
	                    cellTemplate = UtilsService.replaceAll(cellTemplate, "#TOOLTIPFILTER#", tooltipFilter);

	                    cellTemplate = UtilsService.replaceAll(cellTemplate, "colDef", currentColumnHtml);
	                    cellTemplate = getCellTemplateWithFilter(cellTemplate, currentColumn);
	                    var tooltipSection = ctrl.mobileGridView ? 'bs-tooltip data-title="' + currentColumn.name + ' : {{dataItem.' + currentColumn.field + ' ' + tooltipFilter + '}}" ' : '';
	                    ctrl.rowHtml += ctrl.isMobile ? '<div class="mobile-label-container" ng-hide="ctrl.mobileGridView"><label class="mobile-label" >' + currentColumn.name + ': </label></div><div ' + tooltipSection + '  class="mobile-value-container" ng-style=' + mobileStyle + '>'
                            + '<div class="vr-datagrid-celltext ">'
                            + cellTemplate
                            + '</div>'
                            + '</div>'
                            + '</div>' :
                            '<div class="vr-datagrid-cell">'
                            + '<div class="vr-datagrid-celltext ">' + cellTemplate
                            + '</div>'
                            + '</div>'
                            + '</div>';
	                }
	            }
	            if (ctrl.isMobile && ctrl.rowClickDetails == false) {
	                ctrl.rowHtml += '<div ng-if="!ctrl.isMainItemsShown" style="width:60px;display:inline-flex">'
                        + '<span ng-style="::$parent.ctrl.cellLayoutStyle" class="span-summary"> {{ dataItem.actionType}}</span>'
                        + '</div>'
                        + '<div class="mobile-label-container action-main-container" style="align-self: flex-end;margin-left: auto;font-size:22px;margin-top:-5px">'
                        + '<div class="vr-datagrid-celltext">'
                        + '<i ng-if="ctrl.showDeleteRowSection &&  !ctrl.readOnly" class="glyphicon glyphicon-remove hand-cursor" ng-click="ctrl.deleteRowClicked(dataItem)" title="{{ctrl.getRowDeleteIconTitle()}}"></i>'
                        + '<span ng-if="ctrl.showDraggableRowSection && !ctrl.readOnly">'
                        + '<i class="glyphicon glyphicon-circle-arrow-up hand-cursor" ng-show="!$first" ng-click="ctrl.moveRowUp($index)" ></i>'
                        + '<i class="glyphicon glyphicon-circle-arrow-down hand-cursor" ng-show="!$last" ng-click="ctrl.moveRowDown($index)"></i>'
                        + '</span>'
                        + '<i class="glyphicon glyphicon-circle-arrow-right hand-cursor" ng-click="$parent.ctrl.expandRow(dataItem,$event)" style="padding: 2px;top: 5px;" ng-if="$parent.ctrl.expandableColumnWidth != undefined || $parent.ctrl.isActionMenuVisible(dataItem)"></i>'
                        + '</div>'
                        + '</div>';
	            }
	            buildSummaryRowHtml();
	        }



	        function buildSummaryRowHtml() {
	            ctrl.summaryRowHtml = undefined;
	            if (ctrl.summaryDataItem != undefined) {
	                ctrl.summaryRowHtml = '';
	                for (var i = 0; i < ctrl.columnDefs.length; i++) {
	                    var currentColumn = ctrl.columnDefs[i];
	                    var currentColumnHtml = '$parent.ctrl.columnDefs[' + i + ']';
	                    var dataItemColumnPropertyPath = "ctrl.summaryDataItem.columnsValues." + getDataItemColumnProperty(currentColumn);
	                    var cellWidth = ctrl.isMobile ? 'mobileWidth' : 'width';
	                    var mobileStyle = "{width:" + currentColumnHtml + ".mobileWidth}";

	                    var cellClass = ctrl.isMobile ? 'vr-datagrid-cell-container {{ctrl.mobileGridView ?\'grid-view-cell\':\'list-view-cell\'}}"' : '';

	                    var tooltipFilter = currentColumn.filter;
	                    var mobilelableTemplate = '<div class="mobile-label-container" ng-hide="ctrl.mobileGridView" ><label class="mobile-label" >' + currentColumn.name + ': </label></div>';
	                    var tooltipSection = ctrl.mobileGridView ? 'bs-tooltip data-title="' + currentColumn.name + ' : {{' + dataItemColumnPropertyPath + '.dataValue' + tooltipFilter + '}}" ' : '';
	                    var innerSummaryRow = ctrl.isMobile ? mobilelableTemplate
                            + '<div ng-style=' + mobileStyle + ' class="mobile-value-container" ' + tooltipSection + ' >'
                            + '<div class="vr-datagrid-cell">'
                            + ' <div class="vr-datagrid-celltext">'
                            + UtilsService.replaceAll(currentColumn.summaryCellTemplate, "#COLUMNVALUES#", dataItemColumnPropertyPath)
                            + '</div>'
                            + '</div>'
                            + '</div>' : '<div class="vr-datagrid-cell">'
                            + ' <div class="vr-datagrid-celltext">'
                            + UtilsService.replaceAll(currentColumn.summaryCellTemplate, "#COLUMNVALUES#", dataItemColumnPropertyPath)
                            + '</div>'
                            + '</div>';
	                    innerSummaryRow = UtilsService.replaceAll(innerSummaryRow, "#SUMMARYTOOLTIPFILTER#", tooltipFilter);

	                    ctrl.summaryRowHtml +=
                            '<div ng-if="!' + currentColumnHtml + '.isHidden" class="' + cellClass + '" ng-style="{ \'width\': ' + currentColumnHtml + '.' + cellWidth + ', \'display\':\'inline-flex\'' + (i != 0 ? (',\'border-left\': \'' + currentColumn.borderRight) + '\'' : '') + '}">'
                            + innerSummaryRow +
                            '</div>';
	                }
	            }
	        }

	        function getColumnFilter(colDef) {
	            var tooltipFilter = "";
	            if (colDef.type == "Number") {
	                var numberPrecision = UISettingsService.getNormalPrecision() || 2;
	                if (colDef.numberPrecision == "NoDecimal") {
	                    numberPrecision = 0;
	                }
	                else if (colDef.numberPrecision == "LongPrecision") {
	                    numberPrecision = UISettingsService.getUIParameterValue('LongPrecision') || 4;
	                }
	                tooltipFilter = "vrtextOrNumber:" + numberPrecision;
	            }
	            else {
	                if (colDef.type == "LongDatetime") {
	                    tooltipFilter = "date:'yyyy-MM-dd HH:mm:ss'";
	                }
	                else if (colDef.type == "Datetime") {
	                    tooltipFilter = "date:'yyyy-MM-dd HH:mm'";
	                }
	                else if (colDef.type == "Date") {
	                    tooltipFilter = "date:'yyyy-MM-dd'";
	                }
	                else if (colDef.type == "Yearmonth") {
	                    tooltipFilter = "date:'yyyy-MM'";
	                }
	            }
	            if (tooltipFilter != "")
	                tooltipFilter = " | " + tooltipFilter;
	            return tooltipFilter;
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
	                if (ctrl.showexpand != undefined && typeof (ctrl.showexpand) == 'function') {
	                    if (ctrl.showexpand(item)) {
	                        if (ctrl.expandAsfullScreen) {
	                            ctrl.switchFullScreenModeOn(item);
	                        }
	                        else
	                            ctrl.expandRow(item);
	                    }
	                }
	            };

	            gridApi.itemUpdated = function (item) {
	                item.isUpdated = true;
	                itemChanged(item, "Updated");
	                if (ctrl.hasOpenedDataItem == true) {
	                    item.fullScreenMode = true;
	                    ctrl.switchFullScreenModeOn(item);
	                }
	                else
	                    if (ctrl.isMobile)
	                        ctrl.expandRow(item);
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
	                if (ctrl.hasOpenedDataItem) {
	                    $("#mainNgView").addClass("vr-invisible");
	                    $(elem).find(".vr-datagrid-body.row-full-screen").addClass("vr-visible");
	                    ctrl.isLoadingRowFullScreen = true;
	                    ctrl.isLoadingMoreData = true;
	                }
	                else
	                    ctrl.isLoadingMoreData = true;
	            };

	            gridApi.hideLoader = function () {
	                if (ctrl.hasOpenedDataItem) {
	                    $("#mainNgView").removeClass("vr-invisible");
	                    $(elem).find(".vr-datagrid-body.row-full-screen").removeClass("vr-visible");
	                    ctrl.isLoadingRowFullScreen = false;
	                    ctrl.isLoadingMoreData = false;
	                }
	                else
	                    ctrl.isLoadingMoreData = false;
	            };

	            gridApi.refreshGrid = function () {
	                setMainItemsViewVisible();
	                retrieveDataResultKey = null;
	                return retrieveData(false, false, false, DataGridRetrieveDataEventType.ExternalTrigger);
	            };

	            ctrl.redrawGridlayout = function (isGridView) {
	                ctrl.mobileSwitchBtn = false;
	                gridApi.showLoader();
	                setTimeout(function () {
	                    ctrl.mobileGridView = isGridView;
	                    setMainItemsViewVisible();
	                    retrieveDataResultKey = null;
	                    return retrieveData(true, false, false, DataGridRetrieveDataEventType.ExternalTrigger).then(function () {
	                        ctrl.mobileSwitchBtn = true;
	                    });
	                }, 1);
	            };

	            gridApi.refreshMenuActions = function (dataItem) {
	                dataItem.menuActionObj = undefined;
	                ctrl.getMenuActions(dataItem);
	            };

	            gridApi.refreshGridAttributes = function () {
	                var deleteRowFunction = attrs.ondeleterow != undefined ? scope.$parent.$eval(attrs.ondeleterow) : undefined;
	                defineDeleteRowAction(deleteRowFunction);
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
	            gridApi.exportData = function (query) {
	                //retrieveDataInput should be of type Vanrise.Entities.RetrieveDataInput<T>
	                //setMainItemsViewVisible();
	                retrieveDataInput = {
	                    Query: query
	                };
	                return retrieveData(true, true, false, DataGridRetrieveDataEventType.ExternalTrigger);
	            };
	            setTimeout(function () {

	                isGridReady = true;
	                if (ctrl.onReady != null) {
	                    if (ctrl.pagerReadyPromiseDefferred != undefined) {
	                        ctrl.pagerReadyPromiseDefferred.promise.then(function () {
	                            ctrl.onReady(gridApi);
	                        });
	                    }
	                    else {
	                        ctrl.onReady(gridApi);
	                    }
	                }
	            }, 100);

	            gridApi.expandRow = function (dataItem) {
	                if (ctrl.expandAsfullScreen == true) {
	                    ctrl.switchFullScreenModeOn(dataItem);
	                }
	                else
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

	            function getColDefBySyName(sysNameValue) {
	                var colDef = UtilsService.getItemByVal(ctrl.columnDefs, sysNameValue, 'sysName');
	                return colDef;
	            }

	            gridApi.setPersonalizationItem = function (settings) {
	                if (settings != undefined) {
	                    var payloadColumnVisibilities = settings.ColumnVisibilities;
	                    if (payloadColumnVisibilities == undefined || payloadColumnVisibilities == null)
	                        columnVisibilities.length = 0;
	                    else {
	                        for (var i = 0; i < payloadColumnVisibilities.length; i++) {
	                            baseColumnVisibilities = payloadColumnVisibilities;
	                            var colItem = payloadColumnVisibilities[i];
	                            var colDef = getColDefBySyName(colItem.SysName);
	                            if (colDef == null)
	                                continue;
	                            ctrl.setColumnVisibilitiesArray(colItem, colDef);
	                        }
	                    }
	                }
	            };

	            gridApi.getPersonalizationItem = function () {
	                var setting = null;
	                if (columnVisibilities.length > 0) {
	                    setting = {
	                        "$type": "Vanrise.Common.Business.GridPersonalizationExtendedSetting, Vanrise.Common.Business",
	                        ColumnVisibilities: columnVisibilities
	                    };
	                }
	                return setting;
	            };
	        }

	        function filldataItemColumnValues(dataItem, colDef) {
	            try {

	                var colValuesObj = {
	                };
	                if (colDef.field != undefined)
	                    colValuesObj.dataValue = ctrl.getCellValue(dataItem, colDef);
	                colValuesObj.tooltip = ctrl.getCellTooltip(dataItem, colDef);

	                colValuesObj.cellClass = ctrl.getCellClass(dataItem, colDef);

	                colValuesObj.isClickable = ctrl.isColumnClickable(dataItem, colDef);

	                colValuesObj.expendableColumn = ctrl.hasExpendableColumn(colDef);

	                if (dataItem.columnsValues == undefined)
	                    dataItem.columnsValues = {
	                    };

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
	                    ctrl.summaryDataItem.columnsValues = {
	                    };
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
	                else if (ctrl.isMobile) {
	                    setMaxHeight(400);
	                }
	                else {
	                    var sh = innerHeight;
	                    var h = 28;
	                    var bcHeight = UISettingsService.getMasterLayoutBreadcrumbVisibility() ? 20 : 0;
	                    if (isInModal() == true)
	                        h += sh * 0.5;
	                    else
	                        h += sh - (376 + bcHeight);

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
	            if (ctrl.isMobile) return 20;
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
	            ctrl.expandRow = function (dataItem, evnt) {
	                if (expandableRowTemplate != undefined || (ctrl.isActionMenuVisible(dataItem) && ctrl.isMobile)) {
	                    dataItem.showRow = false;
	                    if (evnt) {
	                        $(".tooltip ").remove();
	                        var self = angular.element(evnt.currentTarget);

	                        self.parents('.expandable-row-content').addClass('full-mobile-body');

	                        if (self.parents('.mobile-grid-row').length > 0 && !$('body').hasClass('full-mobile-body'))
	                            $('body').addClass('full-mobile-body');

	                        if (self.parents('.modal-dialog').length > 0) {
	                            self.parents('.modal-dialog').removeClass('centred-model');
	                        }
	                    }
	                    dataItem.expandableRowTemplate = expandableRowTemplate;
	                    dataItem.showRow = true;
	                    dataItem.isRowExpanded = true;
	                    var data;
	                    if (ctrl.objectType)
	                        data = VRDataGridService.getDataItemByRegisterdObjectTypeId(ctrl.objectType.objectTypeId);
	                    dataItem.viewReadyPromiseDeffered && dataItem.viewReadyPromiseDeffered.promise.then(function () {
	                        dataItem.viewDirectiveApi.load({
	                            dataItem: data,
	                            directiveSettings: ctrl.directiveSettings
	                        }).then(function () {
	                            dataItem.isLoadingRowView = false;
	                        });
	                    });

	                }
	            };

	            ctrl.collapseRow = function (dataItem, evnt) {
	                if (ctrl.isMobile) {
	                    $(".tooltip ").remove();
	                    var self = angular.element(evnt.currentTarget);
	                    var expandparent = self.parents('.expandable-row-content').eq(1);
	                    $(expandparent).removeClass('full-mobile-body');
	                    if (self.parents('.expandable-row-content').length == 1) {
	                        $('body').removeClass('full-mobile-body');
	                    }
	                    if (self.parents('.modal-dialog').length > 0) {
	                        self.parents('.modal-dialog').addClass('centred-model');
	                    }
	                }
	                dataItem.isRowExpanded = false;
	            };
	            ctrl.hasOpenedDataItem = false;
	            ctrl.switchFullScreenModeOn = function (dataItem, evnt) {
	                if (!dataItem.fullScreenMode) {
	                    dataItem.fullScreenMode = true;
	                    ctrl.hasOpenedDataItem = true;
	                    ctrl.objectType.objectTypeId = UtilsService.replaceAll(UtilsService.guid(), '-', '');
	                    ctrl.objectType.dataItem = UtilsService.cloneObject(dataItem);
	                    VRDataGridService.addObjectType(ctrl.objectType);
	                    ctrl.parentNames = VRDataGridService.getRegisterdObjectTypes();
	                    if (ctrl.viewDirective) {
	                        dataItem.viewReadyPromiseDeffered = UtilsService.createPromiseDeferred();
	                        dataItem.isLoadingRowView = true;
	                        dataItem.onDirectiveReady = function (api) {
	                            dataItem.viewDirectiveApi = api;
	                            dataItem.viewReadyPromiseDeffered.resolve();
	                        };
	                    }
	                }
	                ctrl.expandRow(dataItem, evnt);
	            };

	            ctrl.switchFullScreenModeOff = function (dataItem, evnt) {
	                dataItem.fullScreenMode = false;
	                ctrl.hasOpenedDataItem = false;
	                VRDataGridService.removeObjectType(ctrl.objectType);
	                ctrl.parentNames = VRDataGridService.getRegisterdObjectTypes();
	                ctrl.collapseRow(dataItem, evnt);
	            };
	        }

	        function defineMenuColumn(hasActionMenu, actionsAttribute) {
	            actionMenuWidth = 0;//hasActionMenu ? 3 : 0;
	            ctrl.actionsColumnWidth = actionMenuWidth + 'px';

	            ctrl.isActionMenuVisible = function (dataItem) {
	                var actions = ctrl.getActionMenuArray(dataItem);
	                if (!actions || actions.length == 0)
	                    return false;
	                var haseEnabledActions = actions.some(function (action) {
	                    if (action.childsactions != undefined) {
	                        if (action.childsactions.length == 0) return false;
	                        return action.childsactions.some(function (childAction) {
	                            return !childAction.disable;
	                        });
	                    }
	                    return !action.disable;
	                });
	                return (haseEnabledActions);
	            };

	            ctrl.hasDefinedMenuAction = function (dataItem) {
	                return ctrl.getActionMenuArray(dataItem) != undefined && ctrl.getActionMenuArray(dataItem).length > 0;
	            };

	            ctrl.getActionMenuArray = function (dataItem) {
	                if (!hasActionMenu || dataItem.isDeleted)
	                    return [];
	                var actions = ctrl.getMenuActions(dataItem);
	                if (actions == undefined || actions == null)
	                    return [];
	                if (actions.length > 0)
	                    return actions;
	            };

	            ctrl.adjustPosition = function (e) {
	                var self = angular.element(e.currentTarget);
	                var selfHeight = $(this).parent().height() + 12;
	                var selfWidth = $(this).parent().width();
	                var selfOffset = $(self).offset();
	                var w = $(window);
	                var selfOffsetRigth = $(document).width() - selfOffset.left - selfWidth;
	                var dropDown = self.parent().find('ul');
	                $(dropDown).css({
	                    position: 'fixed', top: (selfOffset.top - w.scrollTop()) + selfHeight, left: 'auto'
	                });
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
	                            if (menuAction.childsactions != undefined && menuAction.childsactions.length > 0) {
	                                for (var j = 0; j < menuAction.childsactions.length; j++) {
	                                    invokeHasPermission(menuAction.childsactions[j], dataItem);
	                                }
	                            }
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
	                var promise = action.clicked(dataItem, gridApi);
	                action.isSubmitting = true;
	                if (promise != undefined && promise != null) {
	                    promise.finally(function () {
	                        action.isSubmitting = false;
	                    });
	                }
	                else {
	                    var dummypromise = UtilsService.createPromiseDeferred();
	                    setTimeout(function () {
	                        dummypromise.resolve();
	                    }, 10);
	                    dummypromise.promise.finally(function () {
	                        action.isSubmitting = false;
	                    });
	                }
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
	                    var onBeforeDeleteRowClicked = scope.$parent.$eval(attrs.onbeforedeleterowclicked);
	                    if (onBeforeDeleteRowClicked != undefined && typeof (onBeforeDeleteRowClicked) == 'function') {
	                        var onBeforeDeleteRowClickedPromise = onBeforeDeleteRowClicked(dataItem);
	                        if (onBeforeDeleteRowClickedPromise != undefined && onBeforeDeleteRowClickedPromise.then != undefined) {
	                            onBeforeDeleteRowClickedPromise.then(function (response) {
	                                if (response)
	                                    deleteRowFunction(dataItem);
	                                else
	                                    return;
	                            }).catch(function () {
	                                return;
	                            });
	                        }
	                        else {
	                            deleteRowFunction(dataItem);
	                        }
	                    }
	                    else
	                        deleteRowFunction(dataItem);



	                };

	                ctrl.getRowDeleteIconTitle = function () {
	                    return attrs.rowdeleteicontitle != undefined ? attrs.rowdeleteicontitle : "";
	                };
	            }
	            else {
	                deleteRowColumnWidth = 0;
	                ctrl.deleteRowColumnWidth = deleteRowColumnWidth + "px";
	                ctrl.showDeleteRowSection = false;
	            }
	            calculateDataColumnsSectionWidth();
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
	                    ctrl.pagerReadyPromiseDefferred = UtilsService.createPromiseDeferred();
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
	                if (ctrl.showPager && ctrl.pagerSettings != undefined) {
	                    //console.log(new Date() + "pagerSettings ")
	                    pageInfo = ctrl.pagerSettings.getPageInfo();
	                }

	                else if (pagingOnScrollEnabled)
	                    pageInfo = getPageInfo(clearBeforeRetrieve || isSorting);

	                if (pageInfo != undefined) {
	                    retrieveDataInput.FromRow = pageInfo.fromRow;
	                    retrieveDataInput.ToRow = pageInfo.toRow;
	                }
	            }



	            var onResponseReady = function (response) {
	                if (isExport) {
	                    if (response) {
	                        UtilsService.downloadFile(response.data, response.headers);
	                    }
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

	            var retrieveDataContext = {
	                eventType: eventType, isDataSorted: sortColumn != undefined
	            };
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

(function (app) {
    'use stict';
    VRDataGridService.$inject = ['UtilsService'];
    function VRDataGridService(UtilsService) {
        var objectTypes = [];

        function addObjectType(objectType) {
            objectTypes.push(objectType);
        }
        function removeObjectType(objectType) {
            var index = UtilsService.getItemIndexByVal(objectTypes, objectType.objectTypeId, "objectTypeId");
            if (index >= 0)
                objectTypes.splice(index, 1);
        }
        function getRegisterdObjectTypes() {
            return objectTypes;
        }

        function getDataItemByRegisterdObjectTypeId(objectTypeId) {
            var objectType = UtilsService.getItemByVal(objectTypes, objectTypeId, "objectTypeId");
            return objectType && objectType.dataItem || null;
        }

        function clearobjectTypes() {
            objectTypes.length = 0;
        }
        return {
            getRegisterdObjectTypes: getRegisterdObjectTypes,
            addObjectType: addObjectType,
            removeObjectType: removeObjectType,
            clearobjectTypes: clearobjectTypes,
            getDataItemByRegisterdObjectTypeId: getDataItemByRegisterdObjectTypeId
        };
    }
    app.service('VRDataGridService', VRDataGridService);

})(app);