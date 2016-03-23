(function (appControllers) {

    "use strict";

    SummaryTransformationDefinitionEditorController.$inject = ['$scope', 'VR_GenericData_SummaryTransformationDefinitionAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'VR_GenericData_DataRecordTypeAPIService'];

    function SummaryTransformationDefinitionEditorController($scope, VR_GenericData_SummaryTransformationDefinitionAPIService, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, VR_GenericData_DataRecordTypeAPIService) {

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

        var summaryGroupingColumnsAPI;
        var summaryGroupingColumnsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var isRawRecordTypeSelectedProgramatically = false;
        var isSummaryRecordTypeSelectedProgramatically = false;

        loadParameters();
        defineScope();

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
            $scope.scopeModal.onSummaryGroupingColumnsDirectiveReady = function (api) {
                summaryGroupingColumnsAPI = api;
                summaryGroupingColumnsReadyPromiseDeferred.resolve();
            }
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
                        setRecordTypeIdsforSummaryColumnsGrouping();
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
                        setRecordTypeIdsforSummaryColumnsGrouping();
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

        function load() {
            $scope.scopeModal.isLoading = true;

            if (isEditMode) {
                getSummaryTransformationDefinition().then(function () {
                    loadAllControls()
                        .finally(function () {
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
            return UtilsService.waitMultipleAsyncOperations([loadFilterBySection, setTitle, loadDataRecordTypeandRelated])
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
            var promisesRawandSummaryRecordType = [];



            // Load Raw Data Record Type
            var dataRawRecordTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            promises.push(dataRawRecordTypeSelectorLoadDeferred.promise);
            promisesRawandSummaryRecordType.push(dataRawRecordTypeSelectorLoadDeferred.promise)

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
            promisesRawandSummaryRecordType.push(dataSummaryRecordTypeSelectorLoadDeferred.promise)

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



            // Load Raw & Summary Data Record Type

            if (summaryTransformationDefinitionEntity != undefined && summaryTransformationDefinitionEntity.SummaryItemRecordTypeId != undefined && summaryTransformationDefinitionEntity.RawItemRecordTypeId != undefined) {

                var loadSummaryGroupingColumnsPromiseDeferred = UtilsService.createPromiseDeferred();
                promises.push(loadSummaryGroupingColumnsPromiseDeferred.promise);

                UtilsService.waitMultiplePromises(promisesRawandSummaryRecordType).then(function () {
                    summaryGroupingColumnsReadyPromiseDeferred.promise
                        .then(function () {
                            var directivePayload;
                            if (summaryTransformationDefinitionEntity != undefined)
                                directivePayload = {
                                    rawDataRecordTypeId: summaryTransformationDefinitionEntity.RawItemRecordTypeId,
                                    summaryDataRecordTypeId: summaryTransformationDefinitionEntity.SummaryItemRecordTypeId,
                                    KeyFieldMappings: summaryTransformationDefinitionEntity.KeyFieldMappings
                                }
                            VRUIUtilsService.callDirectiveLoad(summaryGroupingColumnsAPI, directivePayload, loadSummaryGroupingColumnsPromiseDeferred);
                        });

                    return loadSummaryGroupingColumnsPromiseDeferred.promise;
                })
            }
            // End Load Raw & Summary Data Record Type



            return UtilsService.waitMultiplePromises(promises);
        }

        function setRecordTypeIdsforSummaryColumnsGrouping() {
            summaryGroupingColumnsAPI.setRecordTypeIds(dataRawRecordTypeSelectorAPI.getSelectedIds(), dataSummaryRecordTypeSelectorAPI.getSelectedIds())
        }


        function getSummaryTransformationDefinition() {
            return VR_GenericData_SummaryTransformationDefinitionAPIService.GetSummaryTransformationDefinition(summaryTransformationDefinitionId).then(function (summaryTransformationDefinition) {
                summaryTransformationDefinitionEntity = summaryTransformationDefinition;
            });
        }

        function buildSummaryTransformationDefinitionObjFromScope() {

            var item = {
                Name: $scope.scopeModal.name,
                SummaryTransformationDefinitionId: summaryTransformationDefinitionId,
                RawItemRecordTypeId: dataRawRecordTypeSelectorAPI.getSelectedIds(),
                SummaryItemRecordTypeId: dataSummaryRecordTypeSelectorAPI.getSelectedIds(),
                RawTimeFieldName: dataRawRecordTypeFieldsSelectorAPI.getSelectedIds(),
                SummaryIdFieldName: dataSummaryRecordTypeFieldsIDSelectorAPI.getSelectedIds(),
                SummaryBatchStartFieldName: dataSummaryRecordTypeFieldsBatchSelectorAPI.getSelectedIds(),
                KeyFieldMappings: summaryGroupingColumnsAPI.getData(),
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
