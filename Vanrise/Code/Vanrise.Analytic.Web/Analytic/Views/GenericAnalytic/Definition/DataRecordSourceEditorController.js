(function (appControllers) {

    'use strict';

    DataRecordSourceEditorController.$inject = ['$scope', 'VR_GenericData_DataRecordStorageAPIService', 'VR_GenericData_DataStoreConfigAPIService', 'VR_GenericData_DataStoreAPIService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'VR_GenericData_DataRecordFieldAPIService',  'ColumnWidthEnum', 'VR_Analytic_OrderDirectionEnum','VRCommon_GridWidthFactorEnum'];

    function DataRecordSourceEditorController($scope, VR_GenericData_DataRecordStorageAPIService, VR_GenericData_DataStoreConfigAPIService, VR_GenericData_DataStoreAPIService, VRNavigationService, UtilsService, VRUIUtilsService, VRNotificationService, VR_GenericData_DataRecordFieldAPIService, ColumnWidthEnum, VR_Analytic_OrderDirectionEnum, VRCommon_GridWidthFactorEnum) {

        var isEditMode;
        var dataRecordSource;

        var dataRecordTypeSelectorAPI;
        var dataRecordTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var dataRecordStorageSelectorAPI;
        var dataRecordStorageSelectorReadyDeferred;

        $scope.selectedFields = [];
        $scope.selectedDetails = [];
        $scope.selectedSortColumns = [];

        $scope.selectedFieldsGrid = [];
        $scope.selectedDetailsGrid = [];
        $scope.selectedSortColumnsGrid = [];

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
            $scope.scopeModel.detailWidths = UtilsService.getArrayEnum(ColumnWidthEnum);
            $scope.scopeModel.orderDirectionList = UtilsService.getArrayEnum(VR_Analytic_OrderDirectionEnum);

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

            $scope.onSelectedField = function(selectedField)
            {
                var dataItem = {
                    FieldName: selectedField.Name,
                    FieldTitle: selectedField.Title,
                };
                dataItem.onGridWidthFactorSelectorReady = function (api) {
                    dataItem.gridWidthFactorAPI = api;
                    var dataItemPayload = { selectedIds: VRCommon_GridWidthFactorEnum.Normal.value };
                    var setLoader = function (value) { $scope.isLoadingDirective = value };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.gridWidthFactorAPI, dataItemPayload, setLoader);
                };
                $scope.selectedFieldsGrid.push(dataItem);
            }
            $scope.onSelectedDetailItem = function (detailItem) {
                var dataItem = {
                    FieldName: detailItem.Name,
                    FieldTitle: detailItem.Title,
                    SelectedDetailWidth: ColumnWidthEnum.QuarterRow
                };
                $scope.selectedDetailsGrid.push(dataItem);
            }
            $scope.onSelectedSortColumn = function (sortColumn) {
                var dataItem = {
                    FieldName: sortColumn.Name,
                    FieldTitle: sortColumn.Title,
                    SelectedOrderDirection: VR_Analytic_OrderDirectionEnum.Ascending
                };
                $scope.selectedSortColumnsGrid.push(dataItem);
            }

            $scope.onDeSelectedField = function (selectedField) {
                var index = UtilsService.getItemIndexByVal($scope.selectedFieldsGrid, selectedField.Name, "FieldName")
                $scope.selectedFieldsGrid.splice(index, 1);
            }
            $scope.onDeSelectedDetailItem = function (detailItem) {
                $scope.selectedDetailsGrid.splice(UtilsService.getItemIndexByVal($scope.selectedDetailsGrid, detailItem.Name, "FieldName"), 1);
            }
            $scope.onDeSelectedSortColumn = function (sortColumn) {
                $scope.selectedSortColumnsGrid.splice(UtilsService.getItemIndexByVal($scope.selectedSortColumnsGrid, sortColumn.Name, "FieldName"), 1);
            }

            $scope.removeField = function (field) {
                $scope.selectedFieldsGrid.splice($scope.selectedFieldsGrid.indexOf(field), 1);
                $scope.selectedFields.splice(UtilsService.getItemIndexByVal($scope.selectedFields, field.FieldName, "Name"), 1);

            }

            $scope.removeDetail = function (detail) {
                $scope.selectedDetailsGrid.splice($scope.selectedDetailsGrid.indexOf(detail), 1);
                $scope.selectedDetails.splice(UtilsService.getItemIndexByVal($scope.selectedDetails, detail.FieldName, "Name"), 1);
            }

            $scope.removeSortColumn = function (sortColumn) {
                $scope.selectedSortColumnsGrid.splice($scope.selectedSortColumnsGrid.indexOf(sortColumn), 1);
                $scope.selectedSortColumns.splice(UtilsService.getItemIndexByVal($scope.selectedSortColumns,sortColumn.FieldName,"Name"), 1);
            }
            
            $scope.isFieldGridValid = function () {
                if ($scope.selectedFieldsGrid.length == 0) {
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
                $scope.scopeModel.sourceName = dataRecordSource.Name;
            }
        }

        function loadDataRecordTypeSelector() {
            var dataRecordTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();
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
            for (var x = 0; x < $scope.selectedFieldsGrid.length; x++) {
                var currentItem = $scope.selectedFieldsGrid[x];
                columns.push({ FieldName: currentItem.FieldName, FieldTitle: currentItem.FieldTitle, Width: currentItem.gridWidthFactorAPI.getSelectedIds() });
            }

            var details = [];
            for (var y = 0; y < $scope.selectedDetailsGrid.length; y++) {
                var currentDetail = $scope.selectedDetailsGrid[y];
                details.push({ FieldName: currentDetail.FieldName, FieldTitle: currentDetail.FieldTitle, ColumnWidth: currentDetail.SelectedDetailWidth.value });
            }

            var sortColumns = [];
            for (var z = 0; z < $scope.selectedSortColumnsGrid.length; z++) {
                var currentSortColumn = $scope.selectedSortColumnsGrid[z];
                sortColumns.push({ FieldName: currentSortColumn.FieldName, IsDescending: currentSortColumn.SelectedOrderDirection.value });
            }

            var obj = {
                Title: $scope.scopeModel.title,
                Name: $scope.scopeModel.sourceName,
                DataRecordTypeId: $scope.selectedDataRecordType.DataRecordTypeId,
                RecordStorageIds: dataRecordStorageSelectorAPI.getSelectedIds(),
                GridColumns: columns,
                ItemDetails: details,
                SortColumns: sortColumns
            };
            return obj;
        }

        function loadDataRecord(dataRecordTypeId) {
           
            var obj = { DataRecordTypeId: dataRecordTypeId };
            var serializedFilter = UtilsService.serializetoJson(obj);
            return VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldsInfo(serializedFilter).then(function (response) {
                $scope.dataRecordTypeFields.length = 0;
                if (response != undefined) {
                    angular.forEach(response, function (item) {
                        $scope.dataRecordTypeFields.push(item.Entity);
                    });
                }
            });
        }

        function addGridColumnAPI(gridField, payload)
        {
            var dataItemPayload = { selectedIds: VRCommon_GridWidthFactorEnum.Normal.value };
            var dataItem = {
                FieldName: gridField.payload.Name,
                FieldTitle: gridField.payload.Title,
            };
            if (payload)
            {
                dataItem.FieldTitle = payload.FieldTitle;
                dataItemPayload.selectedIds = payload.Width;
            }
            dataItem.onGridWidthFactorSelectorReady = function (api) {
                dataItem.gridWidthFactorAPI = api;
                gridField.readyPromiseDeferred.resolve();
            };

            gridField.readyPromiseDeferred.promise
                .then(function () {
                    VRUIUtilsService.callDirectiveLoad(dataItem.gridWidthFactorAPI, dataItemPayload, gridField.loadPromiseDeferred);
                });
            $scope.selectedFieldsGrid.push(dataItem);
        }

        function addGridItemDetailAPI(gridField, payload) {
            var dataItem = {
                FieldName: gridField.payload.Name,
                FieldTitle: gridField.payload.Title,
                SelectedDetailWidth: payload != undefined ? UtilsService.getItemByVal($scope.scopeModel.detailWidths, payload.ColumnWidth, 'value') : ColumnWidthEnum.QuarterRow
            };
            if (payload) {
                dataItem.FieldTitle = payload.FieldTitle;
            }
            $scope.selectedDetailsGrid.push(dataItem);
        }

        function addSortColumnAPI(gridField, payload) {
            var dataItem = {
                FieldName: gridField.payload.Name,
                FieldTitle: gridField.payload.Title,
                SelectedOrderDirection: payload != undefined ? UtilsService.getItemByVal($scope.scopeModel.orderDirectionList, payload.IsDescending, 'value') : undefined
            };
            if (payload) {
                dataItem.FieldTitle = payload.FieldTitle;
            }
            $scope.selectedSortColumnsGrid.push(dataItem);
        }

        function loadDataRecordFields() {
            if (isEditMode)
            {
               var promises = [];
               loadDataRecord(dataRecordSource.DataRecordTypeId).then(function () {
                   $scope.selectedFields.length = 0;
                   $scope.selectedDetails.length = 0;
                   if (dataRecordSource != undefined && dataRecordSource.GridColumns) {
                       for (var x = 0; x < dataRecordSource.GridColumns.length; x++) {
                           var currentColumn = dataRecordSource.GridColumns[x];
                           var selectedField = UtilsService.getItemByVal($scope.dataRecordTypeFields, currentColumn.FieldName, "Name");
                           if (selectedField != undefined) {

                               $scope.selectedFields.push(selectedField);
                               
                               var gridColumnItem = {
                                   payload: selectedField,
                                   readyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                   loadPromiseDeferred: UtilsService.createPromiseDeferred()
                               };
                               promises.push(gridColumnItem.loadPromiseDeferred.promise);
                               addGridColumnAPI(gridColumnItem, currentColumn);
                           }
                       }
                   }
                   if (dataRecordSource != undefined && dataRecordSource.ItemDetails) {
                       for (var y = 0; y < dataRecordSource.ItemDetails.length; y++) {
                           var currentDetail = dataRecordSource.ItemDetails[y];
                           var selectedDetail = UtilsService.getItemByVal($scope.dataRecordTypeFields, currentDetail.FieldName, "Name");
                           if (selectedDetail != undefined) {
                               $scope.selectedDetails.push(selectedDetail);
                               var itemDetail = {
                                   payload: selectedDetail,
                               };
                               addGridItemDetailAPI(itemDetail, currentDetail);
                           }
                       }
                   }
                   if (dataRecordSource != undefined && dataRecordSource.SortColumns) {
                       for (var z = 0; z < dataRecordSource.SortColumns.length; z++) {
                           var currentSortColumn = dataRecordSource.SortColumns[z];
                           var selectedSortColumn = UtilsService.getItemByVal($scope.dataRecordTypeFields, currentSortColumn.FieldName, "Name");
                           if (selectedSortColumn != undefined) {
                               $scope.selectedSortColumns.push(selectedSortColumn);

                               var sortColumn = {
                                   payload: selectedSortColumn,
                               };
                               addSortColumnAPI(sortColumn, currentSortColumn);
                           }
                       }
                   }
               });
               return UtilsService.waitMultiplePromises(promises);
            }
        }
    }

    appControllers.controller('Analytic_DataRecordSourceEditorController', DataRecordSourceEditorController);

})(appControllers);