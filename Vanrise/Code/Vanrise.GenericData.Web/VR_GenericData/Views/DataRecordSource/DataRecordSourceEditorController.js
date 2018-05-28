(function (appControllers) {

    'use strict';

    DataRecordSourceEditorController.$inject = ['$scope', 'VR_GenericData_DataRecordStorageAPIService', 'VR_GenericData_DataStoreAPIService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'VR_GenericData_DataRecordFieldAPIService'];

    function DataRecordSourceEditorController($scope, VR_GenericData_DataRecordStorageAPIService, VR_GenericData_DataStoreAPIService, VRNavigationService, UtilsService, VRUIUtilsService, VRNotificationService, VR_GenericData_DataRecordFieldAPIService) {

        var isEditMode;
        var dataRecordSource;

        var dataRecordTypeSelectorAPI;
        var dataRecordTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var dataRecordStorageSelectorAPI;

        $scope.selectedFields = [];
        $scope.dataRecordTypeFields = [];
        $scope.scopeModal = {};
        var existingSources;

        loadParameters();
        defineScope();
        loadAllControls();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                dataRecordSource = parameters.DataRecordSource;
                existingSources = parameters.ExistingSources;
            }

            isEditMode = (dataRecordSource != undefined);
        }
        function defineScope() {
            $scope.onDataRecordStorageSelectorReady = function (api) {
                dataRecordStorageSelectorAPI = api;
            };
            $scope.onDataRecordTypeSelectorReady = function (api) {
                dataRecordTypeSelectorAPI = api;
                dataRecordTypeSelectorReadyDeferred.resolve();
            };

            $scope.onDataRecordTypeSelectionChanged = function (option) {
                var payload = { DataRecordTypeId: option != undefined ? option.DataRecordTypeId : 0, selectedIds: dataRecordSource != undefined ? dataRecordSource.RecordStorageIds : undefined };
                dataRecordStorageSelectorAPI.load(payload);
                loadDataRecordFields(option);
            };

            //$scope.addField = function () {
            //    $scope.selectedFields.push($scope.selectedDataRecordTypeField);
            //    $scope.dataRecordTypeFields.splice($scope.dataRecordTypeFields.indexOf($scope.selectedDataRecordTypeField), 1);
            //    $scope.selectedDataRecordTypeField = undefined;
            //}

            $scope.removeField = function (field) {
                //$scope.dataRecordTypeFields.push(field);
                $scope.selectedFields.splice($scope.selectedFields.indexOf(field), 1);
            };

            $scope.isFieldGridValid = function () {
                if ($scope.selectedFields.length == 0) {
                    return 'At least one Field must be added.';
                }
                return null;
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };

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
            };
        }
        function validateData() {
            if (!existingSources || existingSources.length == 0) {
                return true;
            }
            if (existingSources.indexOf($scope.scopeModal.title.toLowerCase()) > -1) {
                return false;
            } else {
                return true;
            }
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, setData, loadDataRecordTypeSelector]).catch(function (error) {
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
                $scope.scopeModal.title = dataRecordSource.Title;
            }
        }
        function loadDataRecordTypeSelector() {
            var dataRecordTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            dataRecordTypeSelectorReadyDeferred.promise.then(function () {
                var payload;

                if (dataRecordSource != undefined) {
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
                columns.push({ FieldName: currentItem.FieldName, FieldTitle: currentItem.FieldTitle });
            }
            var obj = {
                Title: $scope.scopeModal.title,
                DataRecordTypeId: $scope.selectedDataRecordType.DataRecordTypeId,
                RecordStorageIds: dataRecordStorageSelectorAPI.getSelectedIds(),
                GridColumns: columns
            };
            return obj;
        }

        function load() {

        }

        function loadDataRecordFields(option) {
            //$scope.selectedDataRecordTypeField = undefined;
            $scope.dataRecordTypeFields.length = 0;
            $scope.selectedFields.length = 0;
            if (option != undefined) {
                
                return VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldsInfo(option.DataRecordTypeId).then(function (response) {
                    if (response != undefined) {
                        angular.forEach(response, function (item) {
                            //var found = false;
                            if (dataRecordSource != undefined && dataRecordSource.GridColumns) {
                                for (var x = 0; x < dataRecordSource.GridColumns.length; x++) {
                                    var currentColumn = dataRecordSource.GridColumns[x];
                                    if (currentColumn.FieldName == item.Entity.Name) {
                                        var obj = { FieldName: currentColumn.FieldName, FieldTitle: currentColumn.FieldTitle };
                                        $scope.selectedFields.push(obj);
                                        //found = true;
                                        break;
                                    }
                                }
                            }
                            //if (!found) {
                            var obj = { FieldName: item.Entity.Name, FieldTitle: item.Entity.Title };
                            $scope.dataRecordTypeFields.push(obj);
                            //}
                        });
                    }
                    dataRecordSource = undefined;
                });
            }
        }
    }

    appControllers.controller('VR_GenericData_DataRecordSourceEditorController', DataRecordSourceEditorController);

})(appControllers);