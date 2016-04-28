(function (appControllers) {

    'use strict';

    DataRecordStorageLogController.$inject = ['$scope', 'VRValidationService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'VRModalService', 'VR_GenericData_DataRecordTypeService', 'VR_Sec_ViewAPIService', 'VRNavigationService', 'VR_GenericData_OrderDirectionEnum'];

    function DataRecordStorageLogController($scope, VRValidationService, UtilsService, VRUIUtilsService, VRNotificationService, VRModalService, VR_GenericData_DataRecordTypeService, VR_Sec_ViewAPIService, VRNavigationService, VR_GenericData_OrderDirectionEnum) {

        var dataRecordStorageSelectorAPI;
        var dataRecordStorageSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var gridAPI;
        var gridQuery = {};
        var filterObj;
        $scope.expression;
        $scope.selectedDataRecordStorage = undefined;
        $scope.scopeModel = {};
        var viewId;
        var viewEntity;

        defineScope();
        loadParameters();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null)
                viewId = parameters.viewId;
        }

        function defineScope() {
            $scope.onDataRecordStorageSelectorReady = function (api) {

                dataRecordStorageSelectorAPI = api;
                dataRecordStorageSelectorReadyDeferred.resolve();
            };

            $scope.onGridReady = function (api) {
                gridAPI = api;
            };

            $scope.search = function () {
                setGridQuery();
                return gridAPI.loadGrid(gridQuery);
            };

            $scope.addFilter = function () {

                var onDataRecordFieldTypeFilterAdded = function (filter, expression) {
                    filterObj = filter;
                    $scope.expression = expression;
                }
                VR_GenericData_DataRecordTypeService.addDataRecordTypeFieldFilter($scope.selectedDRSearchPageStorageSource.DataRecordTypeId, filterObj, onDataRecordFieldTypeFilterAdded);

            };

            $scope.onDRSearchPageStorageSourceChanged = function () {
                $scope.expression = undefined;
                filterObj = null;
            }

            $scope.resetFilter = function () {
                $scope.expression = undefined;
                filterObj = null;
            }

            $scope.validateTimeRange = function () {
                return VRValidationService.validateTimeRange($scope.fromDate, $scope.toDate);
            }
        }

        function load() {
            $scope.scopeModel.isLoading = true;

            $scope.orderDirectionList = UtilsService.getArrayEnum(VR_GenericData_OrderDirectionEnum);
            $scope.limit = 10000;
            getView().then(function () {
                loadAllControls();
            }).catch(function () {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.scopeModel.isLoading = false;
            });
        }

        function getView() {
            return VR_Sec_ViewAPIService.GetView(viewId).then(function (viewEntityObj) {
                viewEntity = viewEntityObj;
                $scope.drSearchPageStorageSources = viewEntity.Settings.Sources;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadStaticData]).then(function () {

            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
        }

        function loadStaticData() {

        }

        function setGridQuery() {
            gridQuery = {
                DataRecordStorageIds: $scope.selectedDRSearchPageStorageSource.RecordStorageIds,
                FromTime: $scope.fromDate,
                ToTime: $scope.toDate,
                Columns: getColumns(),
                FilterGroup: filterObj,
                LimitResult: $scope.limit,
                Direction: $scope.selectedOrderDirection.value,
                sortDirection: $scope.selectedOrderDirection.sortDirection
            };
        }
        function getColumns() {
            var columns = [];
            for (var x = 0; x < $scope.selectedDRSearchPageStorageSource.GridColumns.length; x++) {
                var currentColumn = $scope.selectedDRSearchPageStorageSource.GridColumns[x];
                columns.push(currentColumn.FieldName);
            }
            return columns;
        }
    }

    appControllers.controller('VR_GenericData_DataRecordStorageLogController', DataRecordStorageLogController);

})(appControllers);