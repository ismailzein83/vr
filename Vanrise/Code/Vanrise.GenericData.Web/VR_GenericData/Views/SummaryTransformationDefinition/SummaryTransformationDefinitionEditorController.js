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
                    loadRawDataRecordTypeFieldsSelector(selectedRawDataRecordTypeId);
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
                var selectedSummaryDataRecordTypeId = dataSummaryRecordTypeSelectorAPI.getSelectedIds();
                if (selectedSummaryDataRecordTypeId != undefined) {
                    loadSummaryDataRecordTypeFieldsIDSelector(selectedSummaryDataRecordTypeId);
                    loadSummaryDataRecordTypeFieldsBatchSelector(selectedSummaryDataRecordTypeId);
                    loadDataRecordStorageSelector(selectedSummaryDataRecordTypeId);
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
            return UtilsService.waitMultipleAsyncOperations([loadFilterBySection, setTitle, loadRawDataRecordTypeSelector, loadRawDataRecordTypeFieldsSelector, loadSummaryDataRecordTypeSelector, loadSummaryDataRecordTypeFieldsIDSelector, loadSummaryDataRecordTypeFieldsBatchSelector, loadDataRecordStorageSelector])
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
               .finally(function () {
                   $scope.scopeModal.isLoading = false;
               });
        }

        function loadRawDataRecordTypeSelector() {
            var dataRawRecordTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            dataRawRecordTypeSelectorReadyDeferred.promise.then(function () {
                var payload;

                if (summaryTransformationDefinitionEntity != undefined) {
                    payload = {
                        selectedIds: summaryTransformationDefinitionEntity.RawItemRecordTypeId
                    };
                }

                VRUIUtilsService.callDirectiveLoad(dataRawRecordTypeSelectorAPI, payload, dataRawRecordTypeSelectorLoadDeferred);
            });

            return dataRawRecordTypeSelectorLoadDeferred.promise;
        }


        function loadRawDataRecordTypeFieldsSelector(selectedRawDataRecordTypeId) {
            var payload;
            var dataRawRecordTypeFieldsSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            dataRawRecordTypeFieldsSelectorReadyDeferred.promise.then(function () {
                if (summaryTransformationDefinitionEntity != undefined && summaryTransformationDefinitionEntity.RawItemRecordTypeId != undefined) {
                    payload = {
                        dataRecordTypeId: summaryTransformationDefinitionEntity.RawItemRecordTypeId,
                        selectedIds: summaryTransformationDefinitionEntity.RawTimeFieldName
                    };
                }
                else if (selectedRawDataRecordTypeId != undefined) {
                    payload = {
                        dataRecordTypeId: selectedRawDataRecordTypeId
                    };
                }

                if (payload != undefined)
                    VRUIUtilsService.callDirectiveLoad(dataRawRecordTypeFieldsSelectorAPI, payload, dataRawRecordTypeFieldsSelectorLoadDeferred);
                else
                    dataRawRecordTypeFieldsSelectorLoadDeferred.resolve();
            });
            return dataRawRecordTypeFieldsSelectorLoadDeferred.promise;
        }

        function loadSummaryDataRecordTypeSelector() {
            var dataSummaryRecordTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            dataSummaryRecordTypeSelectorReadyDeferred.promise.then(function () {
                var payload;

                if (summaryTransformationDefinitionEntity != undefined) {
                    payload = {
                        selectedIds: summaryTransformationDefinitionEntity.SummaryItemRecordTypeId
                    };
                }

                VRUIUtilsService.callDirectiveLoad(dataSummaryRecordTypeSelectorAPI, payload, dataSummaryRecordTypeSelectorLoadDeferred);
            });

            return dataSummaryRecordTypeSelectorLoadDeferred.promise;
        }


        function loadSummaryDataRecordTypeFieldsIDSelector(selectedSummaryDataRecordTypeId) {
            var payload;
            var dataSummaryRecordTypeFieldsIDSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            dataSummaryRecordTypeFieldsIDSelectorReadyDeferred.promise.then(function () {
                if (summaryTransformationDefinitionEntity != undefined && summaryTransformationDefinitionEntity.SummaryItemRecordTypeId != undefined) {
                    payload = {
                        dataRecordTypeId: summaryTransformationDefinitionEntity.SummaryItemRecordTypeId,
                        selectedIds: summaryTransformationDefinitionEntity.SummaryIdFieldName
                    };
                }
                else if (selectedSummaryDataRecordTypeId != undefined) {
                    payload = {
                        dataRecordTypeId: selectedSummaryDataRecordTypeId,
                    };
                }

                if (payload != undefined)
                    VRUIUtilsService.callDirectiveLoad(dataSummaryRecordTypeFieldsIDSelectorAPI, payload, dataSummaryRecordTypeFieldsIDSelectorLoadDeferred);
                else
                    dataSummaryRecordTypeFieldsIDSelectorLoadDeferred.resolve();
            });
            return dataSummaryRecordTypeFieldsIDSelectorLoadDeferred.promise;
        }

        function loadSummaryDataRecordTypeFieldsBatchSelector(selectedSummaryDataRecordTypeId) {
            var payload;
            var dataSummaryRecordTypeFieldsBatchSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            dataSummaryRecordTypeFieldsBatchSelectorReadyDeferred.promise.then(function () {
                if (summaryTransformationDefinitionEntity != undefined && summaryTransformationDefinitionEntity.SummaryItemRecordTypeId != undefined) {
                    payload = {
                        dataRecordTypeId: summaryTransformationDefinitionEntity.SummaryItemRecordTypeId,
                        selectedIds: summaryTransformationDefinitionEntity.SummaryBatchStartFieldName
                    };
                }
                else if (selectedSummaryDataRecordTypeId != undefined) {
                    payload = {
                        dataRecordTypeId: selectedSummaryDataRecordTypeId
                    };
                }

                if (payload != undefined)
                    VRUIUtilsService.callDirectiveLoad(dataSummaryRecordTypeFieldsBatchSelectorAPI, payload, dataSummaryRecordTypeFieldsBatchSelectorLoadDeferred);
                else
                    dataSummaryRecordTypeFieldsBatchSelectorLoadDeferred.resolve();
            });
            return dataSummaryRecordTypeFieldsBatchSelectorLoadDeferred.promise;
        }


        function loadDataRecordStorageSelector(selectedDataRecordTypeId) {
            var payload;
            var dataRecordStorageSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            dataRecordStorageSelectorReadyDeferred.promise.then(function () {
                if (summaryTransformationDefinitionEntity != undefined && summaryTransformationDefinitionEntity.SummaryItemRecordTypeId != undefined) {
                    payload = {
                        DataRecordTypeId: summaryTransformationDefinitionEntity.SummaryItemRecordTypeId,
                        selectedIds: summaryTransformationDefinitionEntity.DataRecordStorageId
                    };
                }
                else if (selectedDataRecordTypeId != undefined) {
                    payload = {
                        DataRecordTypeId: selectedDataRecordTypeId
                    };
                }

                if (payload != undefined)
                    VRUIUtilsService.callDirectiveLoad(dataRecordStorageSelectorAPI, payload, dataRecordStorageSelectorLoadDeferred);
                else
                    dataRecordStorageSelectorLoadDeferred.resolve();
            });
            return dataRecordStorageSelectorLoadDeferred.promise;
        }

        function getSummaryTransformationDefinition() {
            return VR_GenericData_SummaryTransformationDefinitionAPIService.GetSummaryTransformationDefinition(summaryTransformationDefinitionId).then(function (summaryTransformationDefinition) {
                summaryTransformationDefinitionEntity = summaryTransformationDefinition;
            });
        }

        function buildSummaryTransformationDefinitionObjFromScope() {
            var summaryTransformationDefinition = {
                Name: $scope.scopeModal.name,
                SummaryTransformationDefinitionId: summaryTransformationDefinitionId,
                RawItemRecordTypeId: dataRawRecordTypeSelectorAPI.getSelectedIds(),
                SummaryItemRecordTypeId: dataSummaryRecordTypeSelectorAPI.getSelectedIds(),
                RawTimeFieldName: dataRawRecordTypeFieldsSelectorAPI.getSelectedIds(),
                SummaryIdFieldName: dataSummaryRecordTypeFieldsIDSelectorAPI.getSelectedIds(),
                SummaryBatchStartFieldName: dataSummaryRecordTypeFieldsBatchSelectorAPI.getSelectedIds(),
                BatchRangeRetrieval: {},
                SummaryFromSettings: [],
                UpdateExistingSummaryFromNewSettings: {},
                DataRecordStorageId: dataRecordStorageSelectorAPI.getSelectedIds()
            }
            return summaryTransformationDefinition;
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
