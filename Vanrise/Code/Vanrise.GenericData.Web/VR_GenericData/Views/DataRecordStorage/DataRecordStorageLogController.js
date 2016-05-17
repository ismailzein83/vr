(function (appControllers) {

    'use strict';

    DataRecordStorageLogController.$inject = ['$scope', 'VRValidationService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'VRModalService', 'VR_GenericData_DataRecordTypeService', 'VR_Sec_ViewAPIService', 'VRNavigationService', 'VR_GenericData_OrderDirectionEnum','VR_GenericData_DataRecordFieldAPIService'];

    function DataRecordStorageLogController($scope, VRValidationService, UtilsService, VRUIUtilsService, VRNotificationService, VRModalService, VR_GenericData_DataRecordTypeService, VR_Sec_ViewAPIService, VRNavigationService, VR_GenericData_OrderDirectionEnum, VR_GenericData_DataRecordFieldAPIService) {

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
                if ($scope.selectedDRSearchPageStorageSource != undefined) {
                    $scope.scopeModel.isLoading = true;
                    loadFields().then(function (response) {
                        if (response)
                        {
                            var fields = [];
                            for (var i = 0; i < response.length; i++) {
                                var dataRecordField = response[i];
                                fields.push({
                                    FieldName: dataRecordField.Entity.Name,
                                    FieldTitle: dataRecordField.Entity.Title,
                                    Type: dataRecordField.Entity.Type,
                                });
                            }
                            $scope.scopeModel.isLoading = false;
                            var onDataRecordFieldTypeFilterAdded = function (filter, expression) {
                                filterObj = filter;
                                $scope.expression = expression;
                            }
                            VR_GenericData_DataRecordTypeService.addDataRecordTypeFieldFilter(fields, filterObj, onDataRecordFieldTypeFilterAdded);
                        }

                    });
                }

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

        function loadFields()
        {
            var obj = { DataRecordTypeId: $scope.selectedDRSearchPageStorageSource.DataRecordTypeId };
            var serializedFilter = UtilsService.serializetoJson(obj);
            return VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldsInfo(serializedFilter);
        }

        function load() {
            $scope.scopeModel.isLoading = true;

            $scope.orderDirectionList = UtilsService.getArrayEnum(VR_GenericData_OrderDirectionEnum);
            $scope.selectedOrderDirection = $scope.orderDirectionList[0];
            $scope.fromDate = new Date();
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
                GridColumns: $scope.selectedDRSearchPageStorageSource.GridColumns,
                FilterGroup: filterObj,
                LimitResult: $scope.limit,
                Direction: $scope.selectedOrderDirection.value,
                sortDirection: $scope.selectedOrderDirection.sortDirection,
                DataRecordTypeId: $scope.selectedDRSearchPageStorageSource.DataRecordTypeId
            };
        }
    }

    appControllers.controller('VR_GenericData_DataRecordStorageLogController', DataRecordStorageLogController);

})(appControllers);