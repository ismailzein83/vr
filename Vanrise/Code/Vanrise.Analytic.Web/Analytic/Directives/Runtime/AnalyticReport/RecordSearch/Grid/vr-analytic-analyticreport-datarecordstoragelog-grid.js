(function (app) {

    'use strict';

    DataRecordStorageLogGridDirective.$inject = ['VR_GenericData_DataRecordStorageLogAPIService', 'VRNotificationService', 'VR_GenericData_DataRecordFieldAPIService','UtilsService'];

    function DataRecordStorageLogGridDirective(VR_GenericData_DataRecordStorageLogAPIService, VRNotificationService, VR_GenericData_DataRecordFieldAPIService, UtilsService) {
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

            var gridAPI;

            function initializeController() {
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
                        onResponseReady(response);
                    }).catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
                };
            }

            function getDirectiveAPI() {
                var api = {};

                api.loadGrid = function (query) {
                    var promiseDeffer = UtilsService.createPromiseDeferred();
                    getDataRecordAttributes(query).then(function () {
                        query.Columns = getColumnsName(query.GridColumns);
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
                        ctrl.columns.push(column);
                    });

                    ctrl.showGrid = true;

                });
            }

            function getColumnsName(gridColumns) {
                var columns = [];
                for (var x = 0; x < gridColumns.length; x++) {
                    var currentColumn = gridColumns[x];
                    columns.push(currentColumn.FieldName);
                }
                return columns;
            }
        }
    } 

    app.directive('vrAnalyticAnalyticreportDatarecordstoragelogGrid', DataRecordStorageLogGridDirective);

})(app);