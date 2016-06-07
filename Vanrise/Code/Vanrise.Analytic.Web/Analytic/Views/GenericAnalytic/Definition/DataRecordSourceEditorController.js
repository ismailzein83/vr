﻿(function (appControllers) {

    'use strict';

    DataRecordSourceEditorController.$inject = ['$scope', 'VR_GenericData_DataRecordStorageAPIService', 'VR_GenericData_DataStoreConfigAPIService', 'VR_GenericData_DataStoreAPIService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'VR_GenericData_DataRecordFieldAPIService','VR_Analytic_GridWidthEnum'];

    function DataRecordSourceEditorController($scope, VR_GenericData_DataRecordStorageAPIService, VR_GenericData_DataStoreConfigAPIService, VR_GenericData_DataStoreAPIService, VRNavigationService, UtilsService, VRUIUtilsService, VRNotificationService, VR_GenericData_DataRecordFieldAPIService, VR_Analytic_GridWidthEnum) {

        var isEditMode;
        var dataRecordSource;

        var dataRecordTypeSelectorAPI;
        var dataRecordTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var dataRecordStorageSelectorAPI;
        var dataRecordStorageSelectorReadyDeferred;

        $scope.selectedFields = [];
        $scope.dataRecordTypeFields = [];
        $scope.scopeModel = {};
        var existingSources;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                dataRecordSource = parameters.DataRecordSource;
                existingSources = parameters.ExistingSources;
            }

            isEditMode = (dataRecordSource != undefined);
        }
        function defineScope() {
            $scope.scopeModel.gridWidths = UtilsService.getArrayEnum(VR_Analytic_GridWidthEnum);

            $scope.onDataRecordStorageSelectorReady = function (api) {
                dataRecordStorageSelectorAPI = api;
            }

            $scope.onDataRecordTypeSelectorReady = function (api) {
                dataRecordTypeSelectorAPI = api;
                dataRecordTypeSelectorReadyDeferred.resolve();
            };

            $scope.onDataRecordTypeSelectionChanged = function (option) {
                if (option != undefined)
                {
                    if (dataRecordStorageSelectorAPI != undefined) {
                        var setLoader = function (value) { $scope.isRecordStorageLoading = value; };
                        var payload = {
                            DataRecordTypeId: option != undefined ? option.DataRecordTypeId : 0,
                            selectedIds: dataRecordSource != undefined ? dataRecordSource.RecordStorageIds : undefined
                        };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataRecordStorageSelectorAPI, payload, setLoader, dataRecordStorageSelectorReadyDeferred);
                    }
                    $scope.isRecordTypeFieldsLoading = true;
                    loadDataRecord(option.DataRecordTypeId).finally(function (response) {
                        $scope.isRecordTypeFieldsLoading = false;
                    });
                }
            };

            $scope.removeField = function (field) {
                $scope.selectedFields.splice($scope.selectedFields.indexOf(field), 1);
            }

            $scope.isFieldGridValid = function () {
                if ($scope.selectedFields.length == 0) {
                    return 'At least one Field must be added.'
                }
                return null;
            }

            $scope.close = function () {
                $scope.modalContext.closeModal();
            }

            $scope.saveDataRecordSource = function () {
                if (validateData()) {
                    if (isEditMode) {
                        return updateDataRecordSource();
                    }
                    else {
                        return saveDataRecordSource();
                    }
                }
                else {
                    VRNotificationService.showWarning('Same Source Title Exists');
                }
            }
        }
        function validateData() {
            if (!existingSources || existingSources.length == 0) {
                return true;
            }
            if (existingSources.indexOf($scope.scopeModel.title.toLowerCase()) > -1) {
                return false;
            } else {
                return true;
            }
        }

        function load() {
            $scope.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
          
            return UtilsService.waitMultipleAsyncOperations([setTitle, setData, loadDataRecordTypeSelector, loadDataRecordFields]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }

        function setTitle() {
            if (isEditMode && dataRecordSource != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(dataRecordSource.Title, 'Record Source');
            else
                $scope.title = UtilsService.buildTitleForAddEditor('Record Source');
        }

        function setData() {
            if (dataRecordSource != undefined) {
                $scope.scopeModel.title = dataRecordSource.Title;
            }
        }
        function loadDataRecordTypeSelector() {
            var dataRecordTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            //dataRecordTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            dataRecordTypeSelectorReadyDeferred.promise.then(function () {
                var payload;

                if (isEditMode) {
                    payload = {
                        selectedIds: dataRecordSource.DataRecordTypeId
                    };
                }

                VRUIUtilsService.callDirectiveLoad(dataRecordTypeSelectorAPI, payload, dataRecordTypeSelectorLoadDeferred);
            });

            return dataRecordTypeSelectorLoadDeferred.promise;
        }

        function saveDataRecordSource() {
            var sourceObj = buildSourceObj();
            if ($scope.onDataRecordSourceAdded != undefined && typeof ($scope.onDataRecordSourceAdded) == 'function') {
                $scope.onDataRecordSourceAdded(sourceObj);
            }
            $scope.modalContext.closeModal();
        }

        function updateDataRecordSource() {
            var sourceObj = buildSourceObj();
            if ($scope.onDataRecordSourceUpdated != undefined && typeof ($scope.onDataRecordSourceUpdated) == 'function') {
                $scope.onDataRecordSourceUpdated(sourceObj);
            }
            $scope.modalContext.closeModal();
        }

        function buildSourceObj() {
            var columns = [];
            for (var x = 0; x < $scope.selectedFields.length; x++) {
                var currentItem = $scope.selectedFields[x];
                columns.push({ FieldName: currentItem.FieldName, FieldTitle: currentItem.FieldTitle, Width: currentItem.SelectedGridWidth.value });
            }
            var obj = {
                Title: $scope.scopeModel.title,
                DataRecordTypeId: $scope.selectedDataRecordType.DataRecordTypeId,
                RecordStorageIds: dataRecordStorageSelectorAPI.getSelectedIds(),
                GridColumns: columns
            };
            return obj;
        }

        function loadDataRecord(dataRecordTypeId)
        {
          
            $scope.selectedFields.length = 0;
            var obj = { DataRecordTypeId: dataRecordTypeId };
            var serializedFilter = UtilsService.serializetoJson(obj);
            return VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldsInfo(serializedFilter).then(function (response) {
                if (response != undefined) {
                    $scope.dataRecordTypeFields.length = 0;
                    angular.forEach(response, function (item) {
                        var obj = { FieldName: item.Entity.Name, FieldTitle: item.Entity.Title, SelectedGridWidth: VR_Analytic_GridWidthEnum.Normal, };
                        $scope.dataRecordTypeFields.push(obj);
                    });
                }
            });
        }

        function loadDataRecordFields() {
            if (isEditMode)
            {
               return loadDataRecord(dataRecordSource.DataRecordTypeId).then(function()
               {
                   $scope.selectedFields.length = 0;
                    if (dataRecordSource != undefined && dataRecordSource.GridColumns) {
                        for (var x = 0; x < dataRecordSource.GridColumns.length; x++) {
                            var currentColumn = dataRecordSource.GridColumns[x];
                            var selectedField = UtilsService.getItemByVal($scope.dataRecordTypeFields, currentColumn.FieldName, "FieldName");
                            if (selectedField != undefined)
                            {
                                selectedField.SelectedGridWidth = UtilsService.getItemByVal($scope.scopeModel.gridWidths, currentColumn.Width,'value');
                                $scope.selectedFields.push(selectedField);
                            }
                        }
                    }
                })
            }
        }
    }

    appControllers.controller('Analytic_DataRecordSourceEditorController', DataRecordSourceEditorController);

})(appControllers);