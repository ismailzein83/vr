(function (appControllers) {

    "use strict";

    SummaryTransformationDefinitionEditorController.$inject = ['$scope', 'VR_GenericData_SummaryTransformationDefinitionAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'VR_GenericData_DataRecordTypeAPIService', 'VR_GenericData_KeyFieldMappingService'];

    function SummaryTransformationDefinitionEditorController($scope, VR_GenericData_SummaryTransformationDefinitionAPIService, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, VR_GenericData_DataRecordTypeAPIService, VR_GenericData_KeyFieldMappingService) {

        var isEditMode;
        var summaryTransformationDefinitionEntity;
        var summaryTransformationDefinitionId;
        var dataRawRecordTypeSelectorAPI;
        var dataRawRecordTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var dataRecordStorageSelectorAPI;
        var dataRecordStorageSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var dataRawRecordTypeFieldsSelectorAPI;
        var dataRawRecordTypeFieldsSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var dataSummaryRecordTypeSelectorAPI;
        var dataSummaryRecordTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var dataSummaryRecordTypeFieldsIDSelectorAPI;
        var dataSummaryRecordTypeFieldsIDSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var dataSummaryRecordTypeFieldsBatchSelectorAPI;
        var dataSummaryRecordTypeFieldsBatchSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var isRawRecordTypeSelectedProgramatically = false;
        var isSummaryRecordTypeSelectedProgramatically = false;

        loadParameters();
        defineScope();
        defineMenuActionsColumnsGrouping();
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

            $scope.scopeModal.datasourceColumnGrouping = [];

            $scope.scopeModal.isValidColumnGrouping = function () {
                if ($scope.scopeModal.datasourceColumnGrouping != undefined && $scope.scopeModal.datasourceColumnGrouping.length > 0)
                    return null;
                return "You Should Select at least one column grouping ";
            }

            $scope.scopeModal.addColumnGrouping = function () {
                var onDataItemAdded = function (itemAdded) {
                    var alreadyExists = false;
                    angular.forEach($scope.scopeModal.datasourceColumnGrouping, function (item) {
                        if (item.RawFieldName == itemAdded.RawFieldName && item.SummaryFieldName == itemAdded.SummaryFieldName)
                            alreadyExists = true;
                    });
                    if (!alreadyExists)
                        $scope.scopeModal.datasourceColumnGrouping.push(itemAdded);
                }

                VR_GenericData_KeyFieldMappingService.addItem(dataRawRecordTypeSelectorAPI.getSelectedIds(), dataSummaryRecordTypeSelectorAPI.getSelectedIds(), onDataItemAdded, $scope.scopeModal.datasourceColumnGrouping);
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
                if (!isRawRecordTypeSelectedProgramatically) {
                    var selectedRawDataRecordTypeId = dataRawRecordTypeSelectorAPI.getSelectedIds();
                    if (selectedRawDataRecordTypeId != undefined) {
                        enableColumnGrouping();
                        $scope.scopeModal.isLoadingisRawRecordTypeFields = true;
                        var payload = {
                            dataRecordTypeId: selectedRawDataRecordTypeId
                        };

                        dataRawRecordTypeFieldsSelectorAPI.load(payload).catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        }).finally(function () {
                            $scope.scopeModal.isLoadingisRawRecordTypeFields = false;
                        });
                    }
                }
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

                if (!isSummaryRecordTypeSelectedProgramatically) {
                    var selectedSummaryDataRecordTypeId = dataSummaryRecordTypeSelectorAPI.getSelectedIds();
                    if (selectedSummaryDataRecordTypeId != undefined) {
                        enableColumnGrouping();
                        $scope.scopeModal.isLoadingSummaryRecordTypeFieldsID = true;
                        $scope.scopeModal.isLoadingSummaryRecordTypeFieldsBatch = true;
                        $scope.scopeModal.isLoadingRecordStorage = true;

                        var payloadforRecordFieldsID = {
                            dataRecordTypeId: selectedSummaryDataRecordTypeId
                        };

                        dataSummaryRecordTypeFieldsIDSelectorAPI.load(payloadforRecordFieldsID).catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        }).finally(function () {
                            $scope.scopeModal.isLoadingSummaryRecordTypeFieldsID = false;
                        });

                        var payloadforRecordFieldsBatch = {
                            dataRecordTypeId: selectedSummaryDataRecordTypeId
                        };

                        dataSummaryRecordTypeFieldsBatchSelectorAPI.load(payloadforRecordFieldsBatch).catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        }).finally(function () {
                            $scope.scopeModal.isLoadingSummaryRecordTypeFieldsBatch = false;
                        });


                        var payloadforRecordStorage = {
                            DataRecordTypeId: selectedSummaryDataRecordTypeId
                        };

                        dataRecordStorageSelectorAPI.load(payloadforRecordStorage).catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        }).finally(function () {
                            $scope.scopeModal.isLoadingRecordStorage = false;
                        });


                    }
                }
            }

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
            }
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

        function defineMenuActionsColumnsGrouping() {
            var defaultMenuActions = [
            {
                name: "Edit",
                clicked: editColumnGrouping,
            },
            {
                name: "Delete",
                clicked: deleteColumnGrouping,
            }];

            $scope.gridMenuActionsColumnGrouping = function (dataItem) {
                return defaultMenuActions;
            }
        }

        function editColumnGrouping(dataItem) {
            var onDataItemUpdated = function (updatedDataItem) {
                var alreadyExists = false;
                angular.forEach($scope.scopeModal.datasourceColumnGrouping, function (item) {
                    if (item.RawFieldName == updatedDataItem.RawFieldName && item.SummaryFieldName == updatedDataItem.SummaryFieldName)
                        alreadyExists = true;
                });
                if (!alreadyExists) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModal.datasourceColumnGrouping, dataItem.RawFieldName, 'RawFieldName');
                    $scope.scopeModal.datasourceColumnGrouping[index] = updatedDataItem;
                }
            }
            VR_GenericData_KeyFieldMappingService.editItem(dataRawRecordTypeSelectorAPI.getSelectedIds(), dataSummaryRecordTypeSelectorAPI.getSelectedIds(), dataItem, onDataItemUpdated, $scope.scopeModal.datasourceColumnGrouping);
        }

        function deleteColumnGrouping(dataItem) {
            var onDataItemDeleted = function (deletedDataItem) {
                var index = UtilsService.getItemIndexByVal($scope.scopeModal.datasourceColumnGrouping, dataItem.RawFieldName, 'RawFieldName');
                $scope.scopeModal.datasourceColumnGrouping.splice(index, 1);
            };

            VR_GenericData_KeyFieldMappingService.deleteItem($scope, dataItem, onDataItemDeleted);
        }

        function load() {
            $scope.scopeModal.isLoading = true;

            if (isEditMode) {
                getSummaryTransformationDefinition().then(function () {
                    loadAllControls()
                        .finally(function () {
                            enableColumnGrouping();
                            summaryTransformationDefinitionEntity = undefined;
                        });
                }).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModal.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }


        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadFilterBySection, setTitle, loadDataRecordTypeandRelated, loadColumnsGroup])
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
               .finally(function () {
                   $scope.scopeModal.isLoading = false;
                   isRawRecordTypeSelectedProgramatically = false;
                   isSummaryRecordTypeSelectedProgramatically = false;
               });
        }

        function loadDataRecordTypeandRelated() {
            var promises = [];



            // Load Raw Data Record Type
            var dataRawRecordTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            promises.push(dataRawRecordTypeSelectorLoadDeferred.promise);

            dataRawRecordTypeSelectorReadyDeferred.promise.then(function () {
                var payload;
                if (summaryTransformationDefinitionEntity != undefined && summaryTransformationDefinitionEntity.RawItemRecordTypeId != undefined) {
                    isRawRecordTypeSelectedProgramatically = true;
                    payload = {
                        selectedIds: summaryTransformationDefinitionEntity.RawItemRecordTypeId
                    };
                }

                VRUIUtilsService.callDirectiveLoad(dataRawRecordTypeSelectorAPI, payload, dataRawRecordTypeSelectorLoadDeferred);
            });

            if (summaryTransformationDefinitionEntity != undefined && summaryTransformationDefinitionEntity.RawItemRecordTypeId != undefined) {

                var dataRawRecordTypeFieldsSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                promises.push(dataRawRecordTypeFieldsSelectorLoadDeferred.promise);
                dataRawRecordTypeSelectorLoadDeferred.promise.then(function () {

                    dataRawRecordTypeFieldsSelectorReadyDeferred.promise.then(function () {
                        var payloadRawDataRecordTypeFields;
                        if (summaryTransformationDefinitionEntity != undefined) {
                            payloadRawDataRecordTypeFields = {
                                dataRecordTypeId: summaryTransformationDefinitionEntity.RawItemRecordTypeId,
                                selectedIds: summaryTransformationDefinitionEntity.RawTimeFieldName
                            };
                        }
                        VRUIUtilsService.callDirectiveLoad(dataRawRecordTypeFieldsSelectorAPI, payloadRawDataRecordTypeFields, dataRawRecordTypeFieldsSelectorLoadDeferred);
                    });
                });
            }
            // End Load Raw Data Record Type



            // Load Summary Data Record Type
            var dataSummaryRecordTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            promises.push(dataSummaryRecordTypeSelectorLoadDeferred.promise);

            dataSummaryRecordTypeSelectorReadyDeferred.promise.then(function () {
                var payload;
                if (summaryTransformationDefinitionEntity != undefined && summaryTransformationDefinitionEntity.SummaryItemRecordTypeId != undefined) {
                    isSummaryRecordTypeSelectedProgramatically = true;
                    payload = {
                        selectedIds: summaryTransformationDefinitionEntity.SummaryItemRecordTypeId
                    };
                }

                VRUIUtilsService.callDirectiveLoad(dataSummaryRecordTypeSelectorAPI, payload, dataSummaryRecordTypeSelectorLoadDeferred);
            });


            if (summaryTransformationDefinitionEntity != undefined && summaryTransformationDefinitionEntity.SummaryItemRecordTypeId != undefined) {

                var dataSummaryRecordTypeFieldsIDSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                promises.push(dataSummaryRecordTypeFieldsIDSelectorLoadDeferred.promise);
                dataSummaryRecordTypeSelectorLoadDeferred.promise.then(function () {

                    dataSummaryRecordTypeFieldsIDSelectorReadyDeferred.promise.then(function () {
                        var payloadSummaryDataRecordTypeFieldsID;
                        if (summaryTransformationDefinitionEntity != undefined) {
                            payloadSummaryDataRecordTypeFieldsID = {
                                dataRecordTypeId: summaryTransformationDefinitionEntity.SummaryItemRecordTypeId,
                                selectedIds: summaryTransformationDefinitionEntity.SummaryIdFieldName
                            };
                        }
                        VRUIUtilsService.callDirectiveLoad(dataSummaryRecordTypeFieldsIDSelectorAPI, payloadSummaryDataRecordTypeFieldsID, dataSummaryRecordTypeFieldsIDSelectorLoadDeferred);
                    });
                });

                var dataSummaryRecordTypeFieldsBatchSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                promises.push(dataSummaryRecordTypeFieldsBatchSelectorLoadDeferred.promise);
                dataSummaryRecordTypeSelectorLoadDeferred.promise.then(function () {

                    dataSummaryRecordTypeFieldsBatchSelectorReadyDeferred.promise.then(function () {
                        var payloadSummaryDataRecordTypeFieldsBatch;
                        if (summaryTransformationDefinitionEntity != undefined) {
                            payloadSummaryDataRecordTypeFieldsBatch = {
                                dataRecordTypeId: summaryTransformationDefinitionEntity.SummaryItemRecordTypeId,
                                selectedIds: summaryTransformationDefinitionEntity.SummaryBatchStartFieldName
                            };
                        }
                        VRUIUtilsService.callDirectiveLoad(dataSummaryRecordTypeFieldsBatchSelectorAPI, payloadSummaryDataRecordTypeFieldsBatch, dataSummaryRecordTypeFieldsBatchSelectorLoadDeferred);
                    });
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

        function loadColumnsGroup() {

            if (summaryTransformationDefinitionEntity != undefined) {
                console.log(summaryTransformationDefinitionEntity.KeyFieldMappings)
                for (var i = 0; i < summaryTransformationDefinitionEntity.KeyFieldMappings.length; i++) {
                    var dataItem = summaryTransformationDefinitionEntity.KeyFieldMappings[i];
                    $scope.scopeModal.datasourceColumnGrouping.push(
                        {
                            RawFieldName: dataItem.RawFieldName,
                            SummaryFieldName: dataItem.SummaryFieldName
                        }
                );
                }

                console.log($scope.scopeModal.datasourceColumnGrouping)
            }
        }

        function enableColumnGrouping() {
            $scope.scopeModal.datasourceColumnGrouping.length = 0;
            if (dataRawRecordTypeSelectorAPI.getSelectedIds() == undefined || dataSummaryRecordTypeSelectorAPI.getSelectedIds() == undefined)
                $scope.scopeModal.enableColumnGrouping = false;
            else
                $scope.scopeModal.enableColumnGrouping = true;
        }

        function getSummaryTransformationDefinition() {
            return VR_GenericData_SummaryTransformationDefinitionAPIService.GetSummaryTransformationDefinition(summaryTransformationDefinitionId).then(function (summaryTransformationDefinition) {
                summaryTransformationDefinitionEntity = summaryTransformationDefinition;
            });
        }

        function buildSummaryTransformationDefinitionObjFromScope() {

            var keyFieldMappings = [];

            for (var i = 0; i < $scope.scopeModal.datasourceColumnGrouping.length; i++) {
                var dataItem = $scope.scopeModal.datasourceColumnGrouping[i];

                keyFieldMappings.push(
                    {
                        RawFieldName: dataItem.RawFieldName,
                        SummaryFieldName: dataItem.SummaryFieldName
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
                BatchRangeRetrieval: {},
                SummaryFromSettings: [],
                UpdateExistingSummaryFromNewSettings: {},
                DataRecordStorageId: dataRecordStorageSelectorAPI.getSelectedIds()
            }

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
                if (VRNotificationService.notifyOnItemAdded("Summary Transformation Definition", response)) {
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
                if (VRNotificationService.notifyOnItemUpdated("Summary Transformation Definition", response)) {
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
