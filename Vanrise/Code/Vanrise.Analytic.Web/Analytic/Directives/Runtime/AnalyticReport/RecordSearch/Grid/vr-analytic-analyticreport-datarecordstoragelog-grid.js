(function (app) {

    'use strict';

    DataRecordStorageLogGridDirective.$inject = ['VR_GenericData_DataRecordStorageLogAPIService', 'VRNotificationService', 'VR_GenericData_DataRecordFieldAPIService', 'UtilsService', 'VR_Analytic_GridWidthEnum', 'ColumnWidthEnum', 'VRUIUtilsService', 'VRCommon_StyleDefinitionAPIService'];

    function DataRecordStorageLogGridDirective(VR_GenericData_DataRecordStorageLogAPIService, VRNotificationService, VR_GenericData_DataRecordFieldAPIService, UtilsService, VR_Analytic_GridWidthEnum, ColumnWidthEnum, VRUIUtilsService, VRCommon_StyleDefinitionAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var dataRecordStorageLogGrid = new DataRecordStorageLogGrid($scope, ctrl, $attrs);
                dataRecordStorageLogGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Analytic/Directives/Runtime/AnalyticReport/RecordSearch/Grid/Templates/DataRecordStorageLogGridTemplate.html"
        };

        function DataRecordStorageLogGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridWidths;
            var detailWidths;
            var itemDetails;
            var subviewDefinitions;
            var sortColumns;
            var searchQuery;
            var dataRecordTypeId;
            var dataRecordTypeAttributes;

            var gridAPI;

            function initializeController() {
                ctrl.sortField = 'DateTimeField';
                ctrl.sortDirection = undefined;
                ctrl.dataRecordStorageLogs = [];
                ctrl.classStyleDefinitions = [];
                ctrl.columns = [];

                gridWidths = UtilsService.getArrayEnum(VR_Analytic_GridWidthEnum);
                detailWidths = UtilsService.getArrayEnum(ColumnWidthEnum);

                ctrl.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                ctrl.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady, retrieveDataContext) {
                    if (!retrieveDataContext.isDataSorted) {
                        dataRetrievalInput.SortByColumnName = null;
                    }

                    return VR_GenericData_DataRecordStorageLogAPIService.GetFilteredDataRecordStorageLogs(dataRetrievalInput).then(function (response) {
                        if (response && response.Data) {
                            for (var z = 0; z < response.Data.length; z++) {
                                var dataRecordStorageLog = response.Data[z];
                                if (itemDetails != undefined) {
                                    dataRecordStorageLog.details = UtilsService.cloneObject(itemDetails, false);

                                    for (var x = 0; x < dataRecordStorageLog.details.length; x++) {
                                        var currentDetail = dataRecordStorageLog.details[x];

                                        for (var y = 0; y <= dataRecordTypeAttributes.length; y++) {
                                            var currentAttribute = dataRecordTypeAttributes[y];
                                            if (currentDetail.FieldName == currentAttribute.Name) {
                                                currentDetail.editor = currentAttribute.DetailViewerEditor;
                                                break;
                                            }
                                        }
                                    }
                                }

                                defineDataRecordStorageLogTabs(gridAPI, dataRecordStorageLog, subviewDefinitions, dataRecordTypeId, searchQuery.LimitResult);
                            }
                        }

                        onResponseReady(response);
                    }).catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
                };

                ctrl.isExpandable = function (dataItem) {
                    return ctrl.showDetails;
                };
            }

            function defineAPI() {
                var api = {};

                api.loadGrid = function (query) {
                    ctrl.dataRecordStorageLogs.length = 0;

                    dataRecordTypeId = query.DataRecordTypeId;
                    sortColumns = query.SortColumns;
                    itemDetails = buildItemDetails(query.ItemDetails);

                    ctrl.showDetails = false;
                    if (query.ItemDetails && query.ItemDetails.length > 0)
                        ctrl.showDetails = true;

                    if (query.SubviewDefinitions && query.SubviewDefinitions.length > 0) {
                        subviewDefinitions = query.SubviewDefinitions;
                        ctrl.showDetails = true;
                    } else {
                        subviewDefinitions = undefined;
                    }

                    var promiseDeffer = UtilsService.createPromiseDeferred();
                    loadStyleDefinitions().then(function () {
                        getDataRecordAttributes(query).then(function () {
                            searchQuery = {
                                DataRecordStorageIds: query.DataRecordStorageIds,
                                FromTime: query.FromTime,
                                ToTime: query.ToTime,
                                FilterGroup: query.FilterGroup,
                                LimitResult: query.LimitResult,
                                Direction: query.Direction,
                                Filters: query.Filters
                            };

                            fillQueryColumns(searchQuery, query.GridColumns, query.ItemDetails);

                            if (query.SortColumns && query.SortColumns.length > 0) {
                                searchQuery.SortColumns = [];
                                for (var t = 0; t < query.SortColumns.length; t++) {
                                    var currentSortColumn = query.SortColumns[t];
                                    var sortColumnItem = {
                                        FieldName: currentSortColumn.FieldName,
                                        IsDescending: currentSortColumn.IsDescending
                                    };
                                    searchQuery.SortColumns.push(sortColumnItem);
                                }
                            }

                            gridAPI.retrieveData(searchQuery).finally(function () {
                                promiseDeffer.resolve();
                            }).catch(function (error) {
                                promiseDeffer.reject(error);
                            });
                        });
                    });



                    return promiseDeffer.promise;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
                    ctrl.onReady(api);
            }

            function getDataRecordAttributes(query) {
                return VR_GenericData_DataRecordFieldAPIService.GetDataRecordAttributes(query.DataRecordTypeId).then(function (attributes) {
                    dataRecordTypeAttributes = attributes;
                    ctrl.columns.length = 0;
                    //ctrl.sortDirection = query.sortDirection;

                    angular.forEach(query.GridColumns, function (column) {
                        for (var x = 0; x < attributes.length; x++) {
                            var attribute = attributes[x];
                            if (column.FieldName == attribute.Name) {
                                column.type = attribute.Attribute.Type;
                                column.numberprecision = attribute.Attribute.NumberPrecision;
                                column.Field = getFieldValue(column.type, column.FieldName);
                                break;
                            }
                        }

                        var gridWidth;
                        if (column.ColumnSettings != null)
                            gridWidth = UtilsService.getItemByVal(gridWidths, column.ColumnSettings.Width, "value");
                        if (gridWidth != undefined)
                            column.Widthfactor = gridWidth.widthFactor;

                        var classStyleItem;
                        if (column.ColumnStyleId != null)
                            classStyleItem = UtilsService.getItemByVal(ctrl.classStyleDefinitions, column.ColumnStyleId, "StyleDefinitionId");
                        if (classStyleItem != undefined) {
                            if (classStyleItem.StyleDefinitionSettings != undefined && classStyleItem.StyleDefinitionSettings.StyleFormatingSettings != undefined && classStyleItem.StyleDefinitionSettings.StyleFormatingSettings.ClassName != undefined) {
                                column.cssClass = classStyleItem.StyleDefinitionSettings.StyleFormatingSettings.ClassName;
                            }
                        }


                        ctrl.columns.push(column);
                    });

                    if (sortColumns && sortColumns.length > 0) {
                        var firstSortColumn = sortColumns[0];
                        var matchingAttribute = UtilsService.getItemByVal(dataRecordTypeAttributes, firstSortColumn.FieldName, "Name");
                        ctrl.defaultSortByFieldName = getFieldValue(matchingAttribute.Attribute.Type, matchingAttribute.Name);
                        ctrl.sortDirection = firstSortColumn.IsDescending ? 'DESC' : 'ASC';
                    }
                    else {
                        ctrl.defaultSortByFieldName = 'RecordTime';
                        ctrl.sortDirection = query.sortDirection;
                    }
                });
            }

            function buildItemDetails(itemDetails) {
                if (itemDetails) {
                    for (var t = 0; t < itemDetails.length; t++) {
                        var currentDetailItem = itemDetails[t];
                        var columnWidth = UtilsService.getItemByVal(detailWidths, currentDetailItem.ColumnWidth, "value");
                        if (columnWidth != undefined)
                            currentDetailItem.colnum = columnWidth.numberOfColumns;
                        else
                            currentDetailItem.colnum = 2;
                    }
                }
                return itemDetails;
            }

            function getFieldValue(type, fieldName) {
                return type.indexOf('Date') > -1 || type.indexOf('Number') > -1 ? 'FieldValues.' + fieldName + '.Value' : 'FieldValues.' + fieldName + '.Description';
            }

            function fillQueryColumns(query, gridColumns, itemDetails) {
                query.Columns = [];
                query.ColumnTitles = [];

                for (var x = 0; x < gridColumns.length; x++) {
                    var currentColumn = gridColumns[x];
                    if (query.Columns.indexOf(currentColumn.FieldName) < 0) {
                        query.Columns.push(currentColumn.FieldName);
                        query.ColumnTitles.push(currentColumn.FieldTitle);
                    }
                }

                if (itemDetails != undefined && itemDetails != null && itemDetails.length > 0) {
                    for (var y = 0; y < itemDetails.length; y++) {
                        var currentDetail = itemDetails[y];
                        if (query.Columns.indexOf(currentDetail.FieldName) < 0) {
                            query.Columns.push(currentDetail.FieldName);
                            query.ColumnTitles.push(currentDetail.FieldTitle);
                        }
                    }
                }
            }

            function loadStyleDefinitions() {
                return VRCommon_StyleDefinitionAPIService.GetAllStyleDefinitions().then(function (response) {
                    if (response) {
                        for (var i = 0; i < response.length; i++) {
                            ctrl.classStyleDefinitions.push(response[i]);
                        }
                    }
                });
            }

            function defineDataRecordStorageLogTabs(gridAPI, dataRecordStorageLog, subviewDefinitions, parentDataRecordTypeId, limitResult) {

                var drillDownTabs = [];

                if (dataRecordStorageLog.details != undefined && dataRecordStorageLog.details.length > 0) {
                    buildDetailsDrillDownTab();
                }

                if (subviewDefinitions != undefined) {
                    for (var i = 0; i < subviewDefinitions.length; i++) {
                        var subviewDefinition = subviewDefinitions[i];

                        addDrillDownTab(dataRecordStorageLog, subviewDefinition);
                    }
                }

                setDrillDownTabs();


                function buildDetailsDrillDownTab() {
                    var detailsTab = {};
                    detailsTab.title = 'Details';
                    detailsTab.directive = 'vr-analytic-datarecordsearchpage-itemdetails';

                    detailsTab.loadDirective = function (directiveAPI, dataRecordStorageLog) {
                        dataRecordStorageLog.directiveAPI = directiveAPI;
                        var payload = { dataRecordStorageLog: dataRecordStorageLog };
                        return dataRecordStorageLog.directiveAPI.load(payload);
                    };

                    drillDownTabs.push(detailsTab);
                }

                function addDrillDownTab(dataRecordStorageLog, subviewDefinition) {
                    var drillDownTab = {};
                    drillDownTab.title = subviewDefinition.Name;
                    drillDownTab.directive = subviewDefinition.Settings.RuntimeEditor;

                    drillDownTab.loadDirective = function (directiveAPI, dataRecordStorageLog) {
                        dataRecordStorageLog.dataRecordStorageLogSubviewGridAPI = directiveAPI;

                        var payload = {
                            subviewDefinition: subviewDefinition,
                            dataRecordStorageLog: dataRecordStorageLog,
                            parentDataRecordTypeId: parentDataRecordTypeId,
                            limitResult: limitResult
                        };

                        return dataRecordStorageLog.dataRecordStorageLogSubviewGridAPI.load(payload);
                    };

                    drillDownTabs.push(drillDownTab);
                }

                function setDrillDownTabs() {
                    var drillDownManager = VRUIUtilsService.defineGridDrillDownTabs(drillDownTabs, gridAPI);
                    drillDownManager.setDrillDownExtensionObject(dataRecordStorageLog);
                }
            }
        }

    }

    app.directive('vrAnalyticAnalyticreportDatarecordstoragelogGrid', DataRecordStorageLogGridDirective);

})(app);