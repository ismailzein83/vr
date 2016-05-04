(function (app) {

    'use strict';

    DataRecordStorageLogGridDirective.$inject = ['VR_GenericData_DataRecordStorageLogAPIService', 'VRNotificationService'];

    function DataRecordStorageLogGridDirective(VR_GenericData_DataRecordStorageLogAPIService, VRNotificationService) {
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
            templateUrl: '/Client/Modules/VR_GenericData/Directives/DataRecordStorage/Templates/DataRecordStorageLogGridTemplate.html'
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
                    ctrl.columns.length = 0;
                    ctrl.sortDirection = query.sortDirection;

                    angular.forEach(query.GridColumns, function (column) {
                        ctrl.columns.push(column);
                    });

                    query.Columns = getColumnsName(query.GridColumns);
                    ctrl.showGrid = true;
                    return gridAPI.retrieveData(query);
                };

                return api;
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

    app.directive('vrGenericdataDatarecordstorageLogGrid', DataRecordStorageLogGridDirective);

})(app);