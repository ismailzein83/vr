(function (app) {

    'use strict';

    DataRecordStorageLogGridDirective.$inject = ['VR_GenericData_DataRecordStorageLogAPIService', 'VRNotificationService', 'VR_GenericData_DataRecordFieldAPIService', 'UtilsService', 'VR_Analytic_GridWidthEnum', 'ColumnWidthEnum', 'VRUIUtilsService'];

    function DataRecordStorageLogGridDirective(VR_GenericData_DataRecordStorageLogAPIService, VRNotificationService, VR_GenericData_DataRecordFieldAPIService, UtilsService, VR_Analytic_GridWidthEnum, ColumnWidthEnum, VRUIUtilsService) {
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
            var gridAPI;
            var itemDetails;

            var sortColumns;
            var dataRecordTypeAttributes;

            function initializeController() {
                ctrl.showGrid = false;
                ctrl.sortField = 'DateTimeField';
                ctrl.sortDirection = undefined;
                ctrl.dataRecordStorageLogs = [];
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
                                var currentData = response.Data[z];
                                if (itemDetails != undefined) {
                                    currentData.details = UtilsService.cloneObject(itemDetails, false);

                                    for (var x = 0; x < currentData.details.length; x++) {
                                        var currentDetail = currentData.details[x];

                                        for (var y = 0; y <= dataRecordTypeAttributes.length; y++) {
                                            var currentAttribute = dataRecordTypeAttributes[y];
                                            if (currentDetail.FieldName == currentAttribute.Name) {
                                                currentDetail.editor = currentAttribute.DetailViewerEditor;
                                                break;
                                            }
                                        }
                                        extendDetailItemObject(currentDetail, currentData);
                                    }
                                }
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

                    ctrl.showDetails = false;

                    if (query.ItemDetails && query.ItemDetails.length > 0)
                        ctrl.showDetails = true;

                    itemDetails = buildItemDetails(query.ItemDetails);
                    sortColumns = query.SortColumns;

                    var promiseDeffer = UtilsService.createPromiseDeferred();
                    getDataRecordAttributes(query).then(function () {

                        var searchQuery = {
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

                    ctrl.showGrid = true;
                });
            }

            function getFieldValue(type, fieldName) {
                return type.indexOf('Date') > -1 || type.indexOf('Number') > -1 ? 'FieldValues.' + fieldName + '.Value' : 'FieldValues.' + fieldName + '.Description';
            }

            function fillQueryColumns(query, gridColumns, itemDetails) {
                query.Columns = [];
                query.ColumnTitles = [];

                var columns = [];
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
                return columns;
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

            function extendDetailItemObject(detailItem, data) {
                detailItem.detailViewerLoadDeferred = UtilsService.createPromiseDeferred();
                detailItem.onDetailViewerReady = function (api) {
                    detailItem.detailViewerAPI = api;
                    var payload = { detailItem: detailItem, fieldValue: data.FieldValues[detailItem.FieldName] };
                    VRUIUtilsService.callDirectiveLoad(detailItem.detailViewerAPI, payload, detailItem.detailViewerLoadDeferred);
                };
                return detailItem.detailViewerLoadDeferred.promise;
            };
        }
    }

    app.directive('vrAnalyticAnalyticreportDatarecordstoragelogGrid', DataRecordStorageLogGridDirective);

})(app);