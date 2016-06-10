(function (app) {

    'use strict';

    DataRecordStorageLogGridDirective.$inject = ['VR_GenericData_DataRecordStorageLogAPIService', 'VRNotificationService', 'VR_GenericData_DataRecordFieldAPIService', 'UtilsService', 'VR_Analytic_GridWidthEnum', 'ColumnWidthEnum'];

    function DataRecordStorageLogGridDirective(VR_GenericData_DataRecordStorageLogAPIService, VRNotificationService, VR_GenericData_DataRecordFieldAPIService, UtilsService, VR_Analytic_GridWidthEnum, ColumnWidthEnum) {
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
            ctrl.showGrid = false;
            this.initializeController = initializeController;
            var gridWidths;
            var detailWidths;
            var gridAPI;
            var itemDetails;

            function initializeController() {
                gridWidths = UtilsService.getArrayEnum(VR_Analytic_GridWidthEnum);
                detailWidths = UtilsService.getArrayEnum(ColumnWidthEnum);

                ctrl.sortField = 'DateTimeField';
                ctrl.sortDirection = undefined;
                ctrl.dataRecordStorageLogs = [];
                ctrl.columns = [];

                ctrl.onGridReady = function (api) {
                    gridAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };

                ctrl.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VR_GenericData_DataRecordStorageLogAPIService.GetFilteredDataRecordStorageLogs(dataRetrievalInput).then(function (response) {
                        if (response) {

                            for (var z = 0; z < response.Data.length; z++) {
                                response.Data[z].details = itemDetails;
                            }
                        }
                        onResponseReady(response);
                    }).catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
                };
            }

            function getDirectiveAPI() {
                var api = {};

                api.loadGrid = function (query) {
                   itemDetails = buildItemDetails(query.ItemDetails);

                    var promiseDeffer = UtilsService.createPromiseDeferred();
                    getDataRecordAttributes(query).then(function () {
                        query.Columns = getColumnsName(query.GridColumns, query.ItemDetails);
                        gridAPI.retrieveData(query).finally(function () {
                            promiseDeffer.resolve();
                        }).catch(function (error) {
                            promiseDeffer.reject(error);
                        });
                    });
                    return promiseDeffer.promise;
                };

                return api;
            }

            function getDataRecordAttributes(query) {
                return VR_GenericData_DataRecordFieldAPIService.GetDataRecordAttributes(query.DataRecordTypeId).then(function (attributes) {
                    ctrl.columns.length = 0;
                    ctrl.sortDirection = query.sortDirection;

                    angular.forEach(query.GridColumns, function (column) {
                        for (var x = 0; x < attributes.length; x++) {
                            var attribute = attributes[x];
                            if (column.FieldName == attribute.Name) {
                                column.type = attribute.Attribute.Type;
                                column.numberprecision = attribute.Attribute.NumberPrecision;
                                break;
                            }
                        }
                        var gridWidth = UtilsService.getItemByVal(gridWidths, column.Width, "value");
                        if (gridWidth != undefined)
                            column.Widthfactor = gridWidth.widthFactor;
                        ctrl.columns.push(column);
                    });

                    ctrl.showGrid = true;

                });
            }

            function getColumnsName(gridColumns, itemDetails) {
                var columns = [];
                for (var x = 0; x < gridColumns.length; x++) {
                    var currentColumn = gridColumns[x];
                    columns.push(currentColumn.FieldName);
                }

                if (itemDetails != undefined && itemDetails != null && itemDetails.length > 0) {
                    for (var y = 0; y < itemDetails.length; y++) {
                        var currentDetail = itemDetails[y];
                        columns.push(currentDetail.FieldName);
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
        }
    }

    app.directive('vrAnalyticAnalyticreportDatarecordstoragelogGrid', DataRecordStorageLogGridDirective);

})(app);