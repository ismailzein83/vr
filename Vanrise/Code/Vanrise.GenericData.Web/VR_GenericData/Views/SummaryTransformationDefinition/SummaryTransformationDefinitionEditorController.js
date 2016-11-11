(function (appControllers) {

    "use strict";

    SummaryTransformationDefinitionEditorController.$inject = ['$scope', 'VR_GenericData_SummaryTransformationDefinitionAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'VR_GenericData_DataRecordTypeAPIService', 'VR_GenericData_KeyFieldMappingService'];

    function SummaryTransformationDefinitionEditorController($scope, VR_GenericData_SummaryTransformationDefinitionAPIService, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, VR_GenericData_DataRecordTypeAPIService, VR_GenericData_KeyFieldMappingService) {

        var isEditMode;
        var summaryTransformationDefinitionEntity;
        var summaryTransformationDefinitionId;


        var dataTransformationDefinitionInsertSelectorAPI;
        var dataTransformationDefinitionInsertSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var dataTransformationDefinitionInsertSelectedPromiseDeferred;

        var dataTransformationDefinitionRecordRawInsertSelectorAPI;
        var dataTransformationDefinitionRecordRawInsertSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var dataTransformationDefinitionRecordSummaryInsertSelectorAPI;
        var dataTransformationDefinitionRecordSummaryInsertSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var dataTransformationDefinitionUpdateSelectorAPI;
        var dataTransformationDefinitionUpdateSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var dataTransformationDefinitionUpdateSelectedPromiseDeferred;

        var dataTransformationDefinitionRecordExistingSummaryUpdateSelectorAPI;
        var dataTransformationDefinitionRecordExistingSummaryUpdateSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var dataTransformationDefinitionRecordNewSummaryUpdateSelectorAPI;
        var dataTransformationDefinitionRecordNewSummaryUpdateSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var dataRawRecordTypeSelectorAPI;
        var dataRawRecordTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var dataRawRecordTypeSelectedPromiseDeferred;

        var dataRecordStorageSelectorAPI;
        var dataRecordStorageSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var dataRawRecordTypeFieldsSelectorAPI;
        var dataRawRecordTypeFieldsSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var dataSummaryRecordTypeSelectorAPI;
        var dataSummaryRecordTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var dataSummaryRecordTypeSelectedPromiseDeferred;

        var dataSummaryRecordTypeFieldsIDSelectorAPI;
        var dataSummaryRecordTypeFieldsIDSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var dataSummaryRecordTypeFieldsBatchSelectorAPI;
        var dataSummaryRecordTypeFieldsBatchSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var batchTimeIntervalSelectorAPI;
        var batchTimeIntervalSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var directiveSummaryReadyAPI;
        var directiveSummaryReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var directiveSummarySelectedPromiseDeferred;

        var selectedFieldNames = [];

        loadParameters();
        defineScope();
        //defineMenuActionsColumnsGrouping();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                summaryTransformationDefinitionId = parameters.SummaryTransformationDefinitionId;
            }
            isEditMode = (summaryTransformationDefinitionId != undefined);
        }

        function defineScope() {
            $scope.scopeModal = {}
            $scope.scopeModal.enableColumnGrouping = true;
            $scope.scopeModal.enableTransformationforInsert = true;
            $scope.scopeModal.enableTransformationforUpdate = true;

            $scope.scopeModal.summaryFields = [];
            //$scope.scopeModal.datasourceColumnGrouping = [];

            $scope.scopeModal.onSummaryDataRecordTypeFieldsSelectorReady = function (api) {
                directiveSummaryReadyAPI = api;
                directiveSummaryReadyPromiseDeferred.resolve();
            };
            //$scope.scopeModal.onSelectSummaryFieldItem = function (summaryItem) {
            //    var dataItem = {
            //        SummaryFieldName: summaryItem.Name,
            //    };
            //    $scope.scopeModal.datasourceColumnGrouping.push(dataItem);
            //}

            //$scope.scopeModal.onDeselectSummaryFieldItem = function (summaryItem) {
            //    var itemIndex = UtilsService.getItemIndexByVal($scope.scopeModal.datasourceColumnGrouping, summaryItem.Name, 'SummaryFieldName');
            //    $scope.scopeModal.datasourceColumnGrouping.splice(itemIndex, 1);
            //}

            //$scope.scopeModal.isValidColumnGrouping = function () {
            //    if ($scope.scopeModal.datasourceColumnGrouping != undefined && $scope.scopeModal.datasourceColumnGrouping.length > 0)
            //        return null;
            //    return "You Should Select at least one column grouping ";
            //}

            //$scope.scopeModal.addColumnGrouping = function () {
            //    var onDataItemAdded = function (itemAdded) {
            //        var alreadyExists = false;
            //        angular.forEach($scope.scopeModal.datasourceColumnGrouping, function (item) {
            //            if (item.SummaryFieldName == itemAdded.SummaryFieldName)
            //                alreadyExists = true;
            //        });
            //        if (!alreadyExists)
            //            $scope.scopeModal.datasourceColumnGrouping.push(itemAdded);
            //    }

            //    VR_GenericData_KeyFieldMappingService.addItem(dataRawRecordTypeSelectorAPI.getSelectedIds(), dataSummaryRecordTypeSelectorAPI.getSelectedIds(), onDataItemAdded, $scope.scopeModal.datasourceColumnGrouping);
            //};

            $scope.scopeModal.onBatchTimeIntervalSelectorReady = function (api) {
                batchTimeIntervalSelectorAPI = api;
                batchTimeIntervalSelectorReadyDeferred.resolve();
            };

            $scope.scopeModal.selectedDataTransformationDefinitionInsert;
            $scope.scopeModal.onDataTransformationDefinitionInsertReady = function (api) {
                dataTransformationDefinitionInsertSelectorAPI = api;
                dataTransformationDefinitionInsertSelectorReadyDeferred.resolve();
            };

            $scope.scopeModal.selectedDataTransformationDefinitionRecordRawInsert;
            $scope.scopeModal.onDataTransformationDefinitionRecordRawInsertReady = function (api) {
                dataTransformationDefinitionRecordRawInsertSelectorAPI = api;
                dataTransformationDefinitionRecordRawInsertSelectorReadyDeferred.resolve();
            };
            $scope.scopeModal.onDataTransformationDefinitionInsertSelectionChanged = function () {
                var selectedRawDataRecordTypeId = dataRawRecordTypeSelectorAPI.getSelectedIds();
                var selectedSummaryDataRecordTypeId = dataSummaryRecordTypeSelectorAPI.getSelectedIds();

                var selectedDataTransformationDefinitionInsertId = dataTransformationDefinitionInsertSelectorAPI.getSelectedIds();
                if (selectedDataTransformationDefinitionInsertId != undefined) {
                    var setLoaderDataTransformationDefinitionInsertRaw = function (value) { $scope.scopeModal.isLoadingDataTransformationDefinitionInsertRaw = value };
                    var payloadfornDataTransformationDefinitionInsertRaw = {
                        dataTransformationDefinitionId: selectedDataTransformationDefinitionInsertId,
                        filter: { DataRecordTypeIds: [selectedRawDataRecordTypeId], IsArray: false }
                    };

                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataTransformationDefinitionRecordRawInsertSelectorAPI, payloadfornDataTransformationDefinitionInsertRaw, setLoaderDataTransformationDefinitionInsertRaw, dataTransformationDefinitionInsertSelectedPromiseDeferred);

                    var setLoaderDataTransformationDefinitionInsertSummary = function (value) { $scope.scopeModal.isLoadingDataTransformationDefinitionInsertSummary = value };
                    var payloadfornDataTransformationDefinitionInsertSummary = {
                        dataTransformationDefinitionId: selectedDataTransformationDefinitionInsertId,
                        filter: { DataRecordTypeIds: [selectedSummaryDataRecordTypeId], IsArray: false }
                    };

                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataTransformationDefinitionRecordSummaryInsertSelectorAPI, payloadfornDataTransformationDefinitionInsertSummary, setLoaderDataTransformationDefinitionInsertSummary, dataTransformationDefinitionInsertSelectedPromiseDeferred);

                }
                else {
                    if (dataTransformationDefinitionRecordRawInsertSelectorAPI != undefined)
                        dataTransformationDefinitionRecordRawInsertSelectorAPI.clearDataSource();

                    if (dataTransformationDefinitionRecordSummaryInsertSelectorAPI != undefined)
                        dataTransformationDefinitionRecordSummaryInsertSelectorAPI.clearDataSource();
                }
            };

            $scope.scopeModal.selectedDataTransformationDefinitionRecordSummaryInsert;
            $scope.scopeModal.onDataTransformationDefinitionRecordSummaryInsertReady = function (api) {
                dataTransformationDefinitionRecordSummaryInsertSelectorAPI = api;
                dataTransformationDefinitionRecordSummaryInsertSelectorReadyDeferred.resolve();
            };

            $scope.scopeModal.selectedDataTransformationDefinitionUpdate;
            $scope.scopeModal.onDataTransformationDefinitionUpdateReady = function (api) {
                dataTransformationDefinitionUpdateSelectorAPI = api;
                dataTransformationDefinitionUpdateSelectorReadyDeferred.resolve();
            };
            $scope.scopeModal.onDataTransformationDefinitionUpdateSelectionChanged = function () {
                var selectedSummaryDataRecordTypeId = dataSummaryRecordTypeSelectorAPI.getSelectedIds();

                var selectedDataTransformationDefinitionUpdateId = dataTransformationDefinitionUpdateSelectorAPI.getSelectedIds();
                if (selectedDataTransformationDefinitionUpdateId != undefined) {
                    var setLoaderDataTransformationDefinitionUpdateExisting = function (value) { $scope.scopeModal.isLoadingDataTransformationDefinitionUpdateExisting = value };
                    var payloadforDataTransformationDefinitionUpdateExisting = {
                        dataTransformationDefinitionId: selectedDataTransformationDefinitionUpdateId,
                        filter: { DataRecordTypeIds: [selectedSummaryDataRecordTypeId], IsArray: false }
                    };

                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataTransformationDefinitionRecordExistingSummaryUpdateSelectorAPI, payloadforDataTransformationDefinitionUpdateExisting, setLoaderDataTransformationDefinitionUpdateExisting, dataTransformationDefinitionUpdateSelectedPromiseDeferred);

                    var setLoaderDataTransformationDefinitionUpdateNew = function (value) { $scope.scopeModal.isLoadingDataTransformationDefinitionUpdateNew = value };
                    var payloadfornDataTransformationDefinitionUpdateNew = {
                        dataTransformationDefinitionId: selectedDataTransformationDefinitionUpdateId,
                        filter: { DataRecordTypeIds: [selectedSummaryDataRecordTypeId], IsArray: false }
                    };

                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataTransformationDefinitionRecordNewSummaryUpdateSelectorAPI, payloadfornDataTransformationDefinitionUpdateNew, setLoaderDataTransformationDefinitionUpdateNew, dataTransformationDefinitionUpdateSelectedPromiseDeferred);

                }
                else {
                    if (dataTransformationDefinitionRecordExistingSummaryUpdateSelectorAPI != undefined)
                        dataTransformationDefinitionRecordExistingSummaryUpdateSelectorAPI.clearDataSource();

                    if (dataTransformationDefinitionRecordNewSummaryUpdateSelectorAPI != undefined)
                        dataTransformationDefinitionRecordNewSummaryUpdateSelectorAPI.clearDataSource();
                }
            }

            $scope.scopeModal.selectedDataTransformationDefinitionRecordExistingSummaryUpdate;
            $scope.scopeModal.onDataTransformationDefinitionRecordRawUpdateReady = function (api) {
                dataTransformationDefinitionRecordExistingSummaryUpdateSelectorAPI = api;
                dataTransformationDefinitionRecordExistingSummaryUpdateSelectorReadyDeferred.resolve();
            };

            $scope.scopeModal.selectedDataTransformationDefinitionRecordNewSummaryUpdate;
            $scope.scopeModal.onDataTransformationDefinitionRecordSummaryUpdateReady = function (api) {
                dataTransformationDefinitionRecordNewSummaryUpdateSelectorAPI = api;
                dataTransformationDefinitionRecordNewSummaryUpdateSelectorReadyDeferred.resolve();
            };

            $scope.scopeModal.selectedRawDataRecordType;
            $scope.scopeModal.onRawDataRecordTypeSelectorReady = function (api) {
                dataRawRecordTypeSelectorAPI = api;
                dataRawRecordTypeSelectorReadyDeferred.resolve();
            };

            $scope.scopeModal.selectedRawDataRecordTypeFields;
            $scope.scopeModal.onRawDataRecordTypeFieldsSelectorReady = function (api) {
                dataRawRecordTypeFieldsSelectorAPI = api;
                dataRawRecordTypeFieldsSelectorReadyDeferred.resolve();
            };
            $scope.scopeModal.onRawRecordTypeSelectionChanged = function () {
                var selectedRawDataRecordTypeId = dataRawRecordTypeSelectorAPI.getSelectedIds();
                if (selectedRawDataRecordTypeId != undefined) {
                    var setLoader = function (value) { $scope.scopeModal.isLoadingisRawRecordTypeFields = value };
                    var payload = {
                        dataRecordTypeId: selectedRawDataRecordTypeId
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataRawRecordTypeFieldsSelectorAPI, payload, setLoader, dataRawRecordTypeSelectedPromiseDeferred);
                }
                else {
                    if (dataRawRecordTypeFieldsSelectorAPI != undefined)
                        dataRawRecordTypeFieldsSelectorAPI.clearDataSource();
                }

                $scope.scopeModal.selectedDataTransformationDefinitionInsert = undefined;
                if (dataRawRecordTypeSelectorAPI.getSelectedIds() == undefined || dataSummaryRecordTypeSelectorAPI.getSelectedIds() == undefined)
                    $scope.scopeModal.enableTransformationforInsert = false;
                else $scope.scopeModal.enableTransformationforInsert = true;

                enableColumnGrouping();
            }

            $scope.scopeModal.selectedDataRecordStorage;
            $scope.scopeModal.onDataRecordStorageSelectorReady = function (api) {
                dataRecordStorageSelectorAPI = api;
                dataRecordStorageSelectorReadyDeferred.resolve();
            };

            $scope.scopeModal.selectedSummaryDataRecordType;
            $scope.scopeModal.onSummaryDataRecordTypeSelectorReady = function (api) {
                dataSummaryRecordTypeSelectorAPI = api;
                dataSummaryRecordTypeSelectorReadyDeferred.resolve();
            };

            $scope.scopeModal.selectedSummaryDataRecordTypeFieldsID;
            $scope.scopeModal.onSummaryDataRecordTypeFieldsIDSelectorReady = function (api) {
                dataSummaryRecordTypeFieldsIDSelectorAPI = api;
                dataSummaryRecordTypeFieldsIDSelectorReadyDeferred.resolve();
            };

            $scope.scopeModal.onSummaryRecordTypeSelectionChanged = function () {

                var selectedSummaryDataRecordTypeId = dataSummaryRecordTypeSelectorAPI.getSelectedIds();
                if (selectedSummaryDataRecordTypeId != undefined) {
                    var setLoaderFieldsID = function (value) { $scope.scopeModal.isLoadingSummaryRecordTypeFieldsID = value };
                    var payloadforRecordFieldsID = {
                        dataRecordTypeId: selectedSummaryDataRecordTypeId
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataSummaryRecordTypeFieldsIDSelectorAPI, payloadforRecordFieldsID, setLoaderFieldsID, dataSummaryRecordTypeSelectedPromiseDeferred);

                    var setLoaderFieldsBatch = function (value) { $scope.scopeModal.isLoadingSummaryRecordTypeFieldsBatch = value };
                    var payloadforRecordFieldsBatch = {
                        dataRecordTypeId: selectedSummaryDataRecordTypeId
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataSummaryRecordTypeFieldsBatchSelectorAPI, payloadforRecordFieldsBatch, setLoaderFieldsBatch, dataSummaryRecordTypeSelectedPromiseDeferred);


                    var setLoaderRecordStorage = function (value) { $scope.scopeModal.isLoadingRecordStorage = value };
                    var payloadforRecordStorage = {
                        DataRecordTypeId: selectedSummaryDataRecordTypeId
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataRecordStorageSelectorAPI, payloadforRecordStorage, setLoaderRecordStorage, dataSummaryRecordTypeSelectedPromiseDeferred);


                    var setLoaderSummary = function (value) { $scope.scopeModal.isLoadingSummaryFields = value };
                    var payloadforSummaryField = {
                        dataRecordTypeId: selectedSummaryDataRecordTypeId
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveSummaryReadyAPI, payloadforSummaryField, setLoaderSummary, directiveSummarySelectedPromiseDeferred);
                }
                else {

                    if (dataSummaryRecordTypeFieldsIDSelectorAPI != undefined)
                        dataSummaryRecordTypeFieldsIDSelectorAPI.clearDataSource();

                    if (dataSummaryRecordTypeFieldsBatchSelectorAPI != undefined)
                        dataSummaryRecordTypeFieldsBatchSelectorAPI.clearDataSource();

                    if (dataRecordStorageSelectorAPI != undefined)
                        dataRecordStorageSelectorAPI.clearDataSource();

                }



                $scope.scopeModal.selectedDataTransformationDefinitionUpdate = undefined;
                $scope.scopeModal.selectedDataTransformationDefinitionInsert = undefined;
                if (dataRawRecordTypeSelectorAPI.getSelectedIds() == undefined || dataSummaryRecordTypeSelectorAPI.getSelectedIds() == undefined)
                    $scope.scopeModal.enableTransformationforInsert = false;
                else
                    $scope.scopeModal.enableTransformationforInsert = true;


                if (dataSummaryRecordTypeSelectorAPI.getSelectedIds() == undefined)
                    $scope.scopeModal.enableTransformationforUpdate = false;
                else
                    $scope.scopeModal.enableTransformationforUpdate = true;


                enableColumnGrouping();
            };

            $scope.scopeModal.selectedSummaryDataRecordTypeFieldsBatch;
            $scope.scopeModal.onSummaryDataRecordTypeFieldsBatchSelectorReady = function (api) {
                dataSummaryRecordTypeFieldsBatchSelectorAPI = api;
                dataSummaryRecordTypeFieldsBatchSelectorReadyDeferred.resolve();
            };

            $scope.hasSaveSummaryTransformationDefinition = function () {
                if (isEditMode) {
                    return VR_GenericData_SummaryTransformationDefinitionAPIService.HasUpdateSummaryTransformationDefinition();
                }
                else {
                    return VR_GenericData_SummaryTransformationDefinitionAPIService.HasAddSummaryTransformationDefinition();
                }
            };
            $scope.scopeModal.SaveSummaryTransformationDefinition = function () {
                $scope.scopeModal.isLoading = true;
                if (isEditMode) {
                    return updateSummaryTransformationDefinition();
                }
                else {
                    return insertSummaryTransformationDefinition();
                }
            };

            $scope.scopeModal.close = function () {
                $scope.modalContext.closeModal()
            };
        }

        //function defineMenuActionsColumnsGrouping() {
        //    var defaultMenuActions = [
        //    {
        //        name: "Edit",
        //        clicked: editColumnGrouping,
        //    },
        //    {
        //        name: "Delete",
        //        clicked: deleteColumnGrouping,
        //    }];

        //    $scope.gridMenuActionsColumnGrouping = function (dataItem) {
        //        return defaultMenuActions;
        //    }
        //}

        //function editColumnGrouping(dataItem) {
        //    var onDataItemUpdated = function (updatedDataItem) {
        //        var alreadyExists = false;
        //        angular.forEach($scope.scopeModal.datasourceColumnGrouping, function (item) {
        //            if (item.SummaryFieldName == updatedDataItem.SummaryFieldName)
        //                alreadyExists = true;
        //        });
        //        if (alreadyExists) {
        //            var index = UtilsService.getItemIndexByVal($scope.scopeModal.datasourceColumnGrouping, dataItem.RawFieldName, 'RawFieldName');
        //            $scope.scopeModal.datasourceColumnGrouping[index] = updatedDataItem;
        //        }
        //    }
        //    VR_GenericData_KeyFieldMappingService.editItem(dataRawRecordTypeSelectorAPI.getSelectedIds(), dataSummaryRecordTypeSelectorAPI.getSelectedIds(), dataItem, onDataItemUpdated, $scope.scopeModal.datasourceColumnGrouping);
        //}

        //function deleteColumnGrouping(dataItem) {
        //    var onDataItemDeleted = function (deletedDataItem) {
        //        var index = UtilsService.getItemIndexByVal($scope.scopeModal.datasourceColumnGrouping, dataItem.RawFieldName, 'RawFieldName');
        //        $scope.scopeModal.datasourceColumnGrouping.splice(index, 1);
        //    };

        //    VR_GenericData_KeyFieldMappingService.deleteItem($scope, dataItem, onDataItemDeleted);
        //}

        function load() {
            $scope.scopeModal.isLoading = true;

            if (isEditMode) {
                getSummaryTransformationDefinition().then(function () {
                    loadDataRecordTypeandRelated().then(function () {
                        loadAllControls().finally(function () {
                            summaryTransformationDefinitionEntity = undefined;
                        });
                    });

                }).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModal.isLoading = false;
                });
            }
            else {
                loadDataRecordTypeandRelated().then(function () {
                    loadAllControls();
                });
            }
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadFilterBySection, setTitle, loadSummaryInsertSection, loadSummaryUpdateSection, loadBatchStartIdentificationSelector])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.scopeModal.isLoading = false;
              });
        }

        function loadDataRecordTypeandRelated() {
            var promises = [];

            // Load Raw Data Record Type

            var dataRawRecordTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            promises.push(dataRawRecordTypeSelectorLoadDeferred.promise);

            var payloadRawRecordTypeSelector;
            if (summaryTransformationDefinitionEntity != undefined && summaryTransformationDefinitionEntity.RawItemRecordTypeId != undefined) {
                dataRawRecordTypeSelectedPromiseDeferred = UtilsService.createPromiseDeferred();

                payloadRawRecordTypeSelector = {
                    selectedIds: summaryTransformationDefinitionEntity.RawItemRecordTypeId
                };
            }

            dataRawRecordTypeSelectorReadyDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(dataRawRecordTypeSelectorAPI, payloadRawRecordTypeSelector, dataRawRecordTypeSelectorLoadDeferred);
            });


            if (summaryTransformationDefinitionEntity != undefined && summaryTransformationDefinitionEntity.RawItemRecordTypeId != undefined) {

                var dataRawRecordTypeFieldsSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                promises.push(dataRawRecordTypeFieldsSelectorLoadDeferred.promise);
                UtilsService.waitMultiplePromises([dataRawRecordTypeSelectedPromiseDeferred.promise, dataRawRecordTypeFieldsSelectorReadyDeferred.promise]).then(function () {
                    var payloadRawDataRecordTypeFields;
                    payloadRawDataRecordTypeFields = {
                        dataRecordTypeId: summaryTransformationDefinitionEntity.RawItemRecordTypeId,
                        selectedIds: summaryTransformationDefinitionEntity.RawTimeFieldName
                    };
                    VRUIUtilsService.callDirectiveLoad(dataRawRecordTypeFieldsSelectorAPI, payloadRawDataRecordTypeFields, dataRawRecordTypeFieldsSelectorLoadDeferred);
                    dataRawRecordTypeSelectedPromiseDeferred = undefined;
                });
            }
            // End Load Raw Data Record Type


            // Load Summary Data Record Type
            var dataSummaryRecordTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            promises.push(dataSummaryRecordTypeSelectorLoadDeferred.promise);

            var payloadSummaryRecordTypeSelector;
            if (summaryTransformationDefinitionEntity != undefined && summaryTransformationDefinitionEntity.SummaryItemRecordTypeId != undefined) {
                dataSummaryRecordTypeSelectedPromiseDeferred = UtilsService.createPromiseDeferred();

                payloadSummaryRecordTypeSelector = {
                    selectedIds: summaryTransformationDefinitionEntity.SummaryItemRecordTypeId
                };
            }

            dataSummaryRecordTypeSelectorReadyDeferred.promise.then(function () {

                VRUIUtilsService.callDirectiveLoad(dataSummaryRecordTypeSelectorAPI, payloadSummaryRecordTypeSelector, dataSummaryRecordTypeSelectorLoadDeferred);
            });

            if (summaryTransformationDefinitionEntity != undefined) {
                directiveSummarySelectedPromiseDeferred = UtilsService.createPromiseDeferred();

                var directiveSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                directiveSummarySelectedPromiseDeferred.promise.then(function () {
                    for (var i = 0; i < summaryTransformationDefinitionEntity.KeyFieldMappings.length; i++) {
                        var dataItem = summaryTransformationDefinitionEntity.KeyFieldMappings[i];

                        selectedFieldNames.push(dataItem.SummaryFieldName);
                    }




                    var payloadDirective = { dataRecordTypeId: summaryTransformationDefinitionEntity.SummaryItemRecordTypeId, selectedIds: selectedFieldNames };

                    directiveSummaryReadyPromiseDeferred.promise.then(function () {
                        directiveSummarySelectedPromiseDeferred = undefined;
                        VRUIUtilsService.callDirectiveLoad(directiveSummaryReadyAPI, payloadDirective, directiveSelectorLoadDeferred);
                    });
                });
                promises.push(directiveSelectorLoadDeferred.promise);

            }


            if (summaryTransformationDefinitionEntity != undefined && summaryTransformationDefinitionEntity.SummaryItemRecordTypeId != undefined) {

                var dataSummaryRecordTypeFieldsIDSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                promises.push(dataSummaryRecordTypeFieldsIDSelectorLoadDeferred.promise);
                UtilsService.waitMultiplePromises([dataSummaryRecordTypeSelectedPromiseDeferred.promise, dataSummaryRecordTypeFieldsIDSelectorReadyDeferred.promise]).then(function () {
                    var payloadSummaryDataRecordTypeFieldsID;
                    payloadSummaryDataRecordTypeFieldsID = {
                        dataRecordTypeId: summaryTransformationDefinitionEntity.SummaryItemRecordTypeId,
                        selectedIds: summaryTransformationDefinitionEntity.SummaryIdFieldName
                    };
                    VRUIUtilsService.callDirectiveLoad(dataSummaryRecordTypeFieldsIDSelectorAPI, payloadSummaryDataRecordTypeFieldsID, dataSummaryRecordTypeFieldsIDSelectorLoadDeferred);
                    dataSummaryRecordTypeSelectedPromiseDeferred = undefined;
                });


                var dataSummaryRecordTypeFieldsBatchSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                promises.push(dataSummaryRecordTypeFieldsBatchSelectorLoadDeferred.promise);
                UtilsService.waitMultiplePromises([dataSummaryRecordTypeSelectedPromiseDeferred.promise, dataSummaryRecordTypeFieldsBatchSelectorReadyDeferred.promise]).then(function () {
                    var payloadSummaryDataRecordTypeFieldsBatch;
                    payloadSummaryDataRecordTypeFieldsBatch = {
                        dataRecordTypeId: summaryTransformationDefinitionEntity.SummaryItemRecordTypeId,
                        selectedIds: summaryTransformationDefinitionEntity.SummaryBatchStartFieldName
                    };
                    VRUIUtilsService.callDirectiveLoad(dataSummaryRecordTypeFieldsBatchSelectorAPI, payloadSummaryDataRecordTypeFieldsBatch, dataSummaryRecordTypeFieldsBatchSelectorLoadDeferred);
                    dataSummaryRecordTypeSelectedPromiseDeferred = undefined;
                });


                var dataRecordStorageSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                promises.push(dataRecordStorageSelectorLoadDeferred.promise);
                dataSummaryRecordTypeSelectorLoadDeferred.promise.then(function () {

                    dataRecordStorageSelectorReadyDeferred.promise.then(function () {
                        var payloadStorage;
                        if (summaryTransformationDefinitionEntity != undefined) {
                            payloadStorage = {
                                DataRecordTypeId: summaryTransformationDefinitionEntity.SummaryItemRecordTypeId,
                                selectedIds: summaryTransformationDefinitionEntity.DataRecordStorageId
                            };
                        }
                        VRUIUtilsService.callDirectiveLoad(dataRecordStorageSelectorAPI, payloadStorage, dataRecordStorageSelectorLoadDeferred);
                    });
                });


            }
            // End Load Summary Data Record Type




            return UtilsService.waitMultiplePromises(promises);
        }

        function loadBatchStartIdentificationSelector() {
            var batchStartIdentificationSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            var payload;
            if (summaryTransformationDefinitionEntity != undefined && summaryTransformationDefinitionEntity.BatchRangeRetrieval != undefined) {
                payload = {
                    Settings: summaryTransformationDefinitionEntity.BatchRangeRetrieval,
                };
            }
            dataTransformationDefinitionInsertSelectorReadyDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(batchTimeIntervalSelectorAPI, payload, batchStartIdentificationSelectorLoadDeferred);
            });

            return batchStartIdentificationSelectorLoadDeferred.promise;
        }

        //function loadColumnGroup() {
        //    if (summaryTransformationDefinitionEntity != undefined) {
        //        for (var i = 0; i < summaryTransformationDefinitionEntity.KeyFieldMappings.length; i++) {
        //            var dataItem = summaryTransformationDefinitionEntity.KeyFieldMappings[i];
        //            $scope.scopeModal.datasourceColumnGrouping.push(
        //                {
        //                    //RawFieldName: dataItem.RawFieldName,
        //                    SummaryFieldName: dataItem.SummaryFieldName,
        //                    //GetRawFieldExpression: dataItem.GetRawFieldExpression
        //                }
        //            );
        //        }

        //        $scope.scopeModal.enableColumnGrouping = true;
        //    }
        //}

        function loadSummaryInsertSection() {
            // load Data Transformation Definition Insert Selector

            var promises = [];

            var dataTransformationDefinitionInsertSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            promises.push(dataTransformationDefinitionInsertSelectorLoadDeferred.promise);

            var payload;
            if (summaryTransformationDefinitionEntity != undefined && summaryTransformationDefinitionEntity.SummaryFromRawSettings != undefined) {
                dataTransformationDefinitionInsertSelectedPromiseDeferred = UtilsService.createPromiseDeferred();

                payload = {
                    selectedIds: summaryTransformationDefinitionEntity.SummaryFromRawSettings.TransformationDefinitionId
                };
            }

            dataTransformationDefinitionInsertSelectorReadyDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(dataTransformationDefinitionInsertSelectorAPI, payload, dataTransformationDefinitionInsertSelectorLoadDeferred);
            });

            if (summaryTransformationDefinitionEntity != undefined && summaryTransformationDefinitionEntity.SummaryFromRawSettings != undefined && summaryTransformationDefinitionEntity.SummaryFromRawSettings.TransformationDefinitionId != undefined) {

                var dataTransformationDefinitionRecordRawInsertSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                promises.push(dataTransformationDefinitionRecordRawInsertSelectorLoadDeferred.promise);
                UtilsService.waitMultiplePromises([dataTransformationDefinitionInsertSelectedPromiseDeferred.promise, dataTransformationDefinitionRecordRawInsertSelectorReadyDeferred.promise]).then(function () {
                    var payload;
                    payload = {
                        dataTransformationDefinitionId: summaryTransformationDefinitionEntity.SummaryFromRawSettings.TransformationDefinitionId,
                        filter: { DataRecordTypeIds: [summaryTransformationDefinitionEntity.RawItemRecordTypeId], IsArray: false },
                        selectedIds: summaryTransformationDefinitionEntity.SummaryFromRawSettings.RawRecordName
                    };
                    VRUIUtilsService.callDirectiveLoad(dataTransformationDefinitionRecordRawInsertSelectorAPI, payload, dataTransformationDefinitionRecordRawInsertSelectorLoadDeferred);
                    dataTransformationDefinitionInsertSelectedPromiseDeferred = undefined;
                });


                var dataTransformationDefinitionRecordSummaryInsertSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                promises.push(dataTransformationDefinitionRecordSummaryInsertSelectorLoadDeferred.promise);
                UtilsService.waitMultiplePromises([dataTransformationDefinitionInsertSelectedPromiseDeferred.promise, dataTransformationDefinitionRecordSummaryInsertSelectorReadyDeferred.promise]).then(function () {
                    var payload;
                    payload = {
                        dataTransformationDefinitionId: summaryTransformationDefinitionEntity.SummaryFromRawSettings.TransformationDefinitionId,
                        filter: { DataRecordTypeIds: [summaryTransformationDefinitionEntity.SummaryItemRecordTypeId], IsArray: false },
                        selectedIds: summaryTransformationDefinitionEntity.SummaryFromRawSettings.SymmaryRecordName
                    };
                    VRUIUtilsService.callDirectiveLoad(dataTransformationDefinitionRecordSummaryInsertSelectorAPI, payload, dataTransformationDefinitionRecordSummaryInsertSelectorLoadDeferred);
                    dataTransformationDefinitionInsertSelectedPromiseDeferred = undefined;
                });

            }

            // End load Data Transformation Definition Insert Selector

            return UtilsService.waitMultiplePromises(promises);
        }

        function loadSummaryUpdateSection() {
            // load Data Transformation Definition Update Selector

            var promises = [];

            var dataTransformationDefinitionUpdateSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            promises.push(dataTransformationDefinitionUpdateSelectorLoadDeferred.promise);

            var payload;
            if (summaryTransformationDefinitionEntity != undefined && summaryTransformationDefinitionEntity.UpdateExistingSummaryFromNewSettings != undefined) {
                dataTransformationDefinitionUpdateSelectedPromiseDeferred = UtilsService.createPromiseDeferred();

                payload = {
                    selectedIds: summaryTransformationDefinitionEntity.UpdateExistingSummaryFromNewSettings.TransformationDefinitionId
                };
            }

            dataTransformationDefinitionUpdateSelectorReadyDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(dataTransformationDefinitionUpdateSelectorAPI, payload, dataTransformationDefinitionUpdateSelectorLoadDeferred);
            });

            if (summaryTransformationDefinitionEntity != undefined && summaryTransformationDefinitionEntity.UpdateExistingSummaryFromNewSettings != undefined && summaryTransformationDefinitionEntity.UpdateExistingSummaryFromNewSettings.TransformationDefinitionId != undefined) {

                var dataTransformationDefinitionRecordExistingUpdateSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                promises.push(dataTransformationDefinitionRecordExistingUpdateSelectorLoadDeferred.promise);
                UtilsService.waitMultiplePromises([dataTransformationDefinitionUpdateSelectedPromiseDeferred.promise, dataTransformationDefinitionRecordExistingSummaryUpdateSelectorReadyDeferred.promise]).then(function () {
                    var payload;
                    payload = {
                        dataTransformationDefinitionId: summaryTransformationDefinitionEntity.UpdateExistingSummaryFromNewSettings.TransformationDefinitionId,
                        filter: { DataRecordTypeIds: [summaryTransformationDefinitionEntity.SummaryItemRecordTypeId], IsArray: false },
                        selectedIds: summaryTransformationDefinitionEntity.UpdateExistingSummaryFromNewSettings.ExistingRecordName
                    };
                    VRUIUtilsService.callDirectiveLoad(dataTransformationDefinitionRecordExistingSummaryUpdateSelectorAPI, payload, dataTransformationDefinitionRecordExistingUpdateSelectorLoadDeferred);
                    dataTransformationDefinitionUpdateSelectedPromiseDeferred = undefined;
                });


                var dataTransformationDefinitionRecordNewUpdateSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                promises.push(dataTransformationDefinitionRecordNewUpdateSelectorLoadDeferred.promise);
                UtilsService.waitMultiplePromises([dataTransformationDefinitionUpdateSelectedPromiseDeferred.promise, dataTransformationDefinitionRecordNewSummaryUpdateSelectorReadyDeferred.promise]).then(function () {
                    var payload;
                    payload = {
                        dataTransformationDefinitionId: summaryTransformationDefinitionEntity.UpdateExistingSummaryFromNewSettings.TransformationDefinitionId,
                        filter: { DataRecordTypeIds: [summaryTransformationDefinitionEntity.SummaryItemRecordTypeId], IsArray: false },
                        selectedIds: summaryTransformationDefinitionEntity.UpdateExistingSummaryFromNewSettings.NewRecordName
                    };
                    VRUIUtilsService.callDirectiveLoad(dataTransformationDefinitionRecordNewSummaryUpdateSelectorAPI, payload, dataTransformationDefinitionRecordNewUpdateSelectorLoadDeferred);
                    dataTransformationDefinitionUpdateSelectedPromiseDeferred = undefined;
                });

            }

            // End load Data Transformation Definition Insert Selector

            return UtilsService.waitMultiplePromises(promises);
        }

        function enableColumnGrouping() {
            //$scope.scopeModal.datasourceColumnGrouping.length = 0;
            if (dataSummaryRecordTypeSelectorAPI.getSelectedIds() == undefined)
                $scope.scopeModal.enableColumnGrouping = false;
            else
                $scope.scopeModal.enableColumnGrouping = true;
        }

        function getSummaryTransformationDefinition() {
            return VR_GenericData_SummaryTransformationDefinitionAPIService.GetSummaryTransformationDefinition(summaryTransformationDefinitionId)
                .then(function (summaryTransformationDefinition) {
                    summaryTransformationDefinitionEntity = summaryTransformationDefinition;
                });
        }

        function buildSummaryTransformationDefinitionObjFromScope() {

            var keyFieldMappings = [];

            for (var i = 0; i < $scope.scopeModal.summaryFields.length; i++) {
                var dataItem = $scope.scopeModal.summaryFields[i];

                keyFieldMappings.push(
                    {
                        SummaryFieldName: dataItem.Name,
                        //RawFieldName: dataItem.RawFieldName,
                        //GetRawFieldExpression: dataItem.GetRawFieldExpression
                    }
                );
            }


            var item = {
                Name: $scope.scopeModal.name,
                SummaryTransformationDefinitionId: summaryTransformationDefinitionId,
                RawItemRecordTypeId: dataRawRecordTypeSelectorAPI.getSelectedIds(),
                SummaryItemRecordTypeId: dataSummaryRecordTypeSelectorAPI.getSelectedIds(),
                RawTimeFieldName: dataRawRecordTypeFieldsSelectorAPI.getSelectedIds(),
                SummaryIdFieldName: dataSummaryRecordTypeFieldsIDSelectorAPI.getSelectedIds(),
                SummaryBatchStartFieldName: dataSummaryRecordTypeFieldsBatchSelectorAPI.getSelectedIds(),
                KeyFieldMappings: keyFieldMappings,
                SummaryFromRawSettings: {
                    TransformationDefinitionId: dataTransformationDefinitionInsertSelectorAPI.getSelectedIds(),
                    RawRecordName: dataTransformationDefinitionRecordRawInsertSelectorAPI.getSelectedIds(),
                    SymmaryRecordName: dataTransformationDefinitionRecordSummaryInsertSelectorAPI.getSelectedIds()
                },
                UpdateExistingSummaryFromNewSettings: {
                    TransformationDefinitionId: dataTransformationDefinitionUpdateSelectorAPI.getSelectedIds(),
                    NewRecordName: dataTransformationDefinitionRecordNewSummaryUpdateSelectorAPI.getSelectedIds(),
                    ExistingRecordName: dataTransformationDefinitionRecordExistingSummaryUpdateSelectorAPI.getSelectedIds()
                },
                DataRecordStorageId: dataRecordStorageSelectorAPI.getSelectedIds(),
                BatchRangeRetrieval: batchTimeIntervalSelectorAPI.getData()
            };
            return item;
        }

        function loadFilterBySection() {
            if (summaryTransformationDefinitionEntity != undefined) {
                $scope.scopeModal.name = summaryTransformationDefinitionEntity.Name;
            }
        }

        function setTitle() {
            if (isEditMode && summaryTransformationDefinitionEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(summaryTransformationDefinitionEntity.Name, 'Summary Transformation Definition');
            else
                $scope.title = UtilsService.buildTitleForAddEditor('Summary Transformation Definition');
        }

        function insertSummaryTransformationDefinition() {
            var summaryTransformationDefinitionObject = buildSummaryTransformationDefinitionObjFromScope();
            return VR_GenericData_SummaryTransformationDefinitionAPIService.AddSummaryTransformationDefinition(summaryTransformationDefinitionObject)
            .then(function (response) {
                $scope.scopeModal.isLoading = false;
                if (VRNotificationService.notifyOnItemAdded("Summary Transformation Definition", response, "Name")) {
                    if ($scope.onSummaryTransformationDefinitionAdded != undefined)
                        $scope.onSummaryTransformationDefinitionAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });

        }

        function updateSummaryTransformationDefinition() {
            var summaryTransformationDefinitionObject = buildSummaryTransformationDefinitionObjFromScope();
            VR_GenericData_SummaryTransformationDefinitionAPIService.UpdateSummaryTransformationDefinition(summaryTransformationDefinitionObject)
            .then(function (response) {
                $scope.scopeModal.isLoading = false;
                if (VRNotificationService.notifyOnItemUpdated("Summary Transformation Definition", response, "Name")) {
                    if ($scope.onSummaryTransformationDefinitionUpdated != undefined)
                        $scope.onSummaryTransformationDefinitionUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }

    }

    appControllers.controller('VR_GenericData_SummaryTransformationDefinitionEditorController', SummaryTransformationDefinitionEditorController);
})(appControllers);
