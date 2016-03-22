(function (appControllers) {

    "use strict";

    SummaryTransformationDefinitionEditorController.$inject = ['$scope', 'VR_GenericData_SummaryTransformationDefinitionAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'VR_GenericData_DataRecordTypeAPIService'];

    function SummaryTransformationDefinitionEditorController($scope, VR_GenericData_SummaryTransformationDefinitionAPIService, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, VR_GenericData_DataRecordTypeAPIService) {

        var recordTypeEntity;
        var isEditMode;
        var summaryTransformationDefinitionEntity;
        var summaryTransformationDefinitionId;
        var dataRecordTypeSelectorAPI;
        var dataRecordTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var dataRecordTypeFieldsSelectorAPI;
        var dataRecordTypeFieldsSelectorReadyDeferred = UtilsService.createPromiseDeferred();


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
            $scope.scopeModal.selectedDataRecordType;
            $scope.scopeModal.onDataRecordTypeSelectorReady = function (api) {
                dataRecordTypeSelectorAPI = api;
                dataRecordTypeSelectorReadyDeferred.resolve();
            };

            $scope.scopeModal.selectedDataRecordTypeFields;
            $scope.scopeModal.onDataRecordTypeFieldsSelectorReady = function (api) {
                dataRecordTypeFieldsSelectorAPI = api;
                dataRecordTypeFieldsSelectorReadyDeferred.resolve();
            };


            $scope.scopeModal.onRecordTypeSelectionChanged = function () {
                var selectedDataRecordTypeId = dataRecordTypeSelectorAPI.getSelectedIds();
                if (selectedDataRecordTypeId != undefined) {
                    loadDataRecordTypeFieldsSelector(selectedDataRecordTypeId);
                }
            }

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
            return UtilsService.waitMultipleAsyncOperations([loadFilterBySection, setTitle, loadDataRecordTypeSelector, loadDataRecordTypeFieldsSelector])
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
               .finally(function () {
                   $scope.scopeModal.isLoading = false;
               });
        }

        function loadDataRecordTypeSelector() {
            var dataRecordTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            dataRecordTypeSelectorReadyDeferred.promise.then(function () {
                var payload;

                if (summaryTransformationDefinitionEntity != undefined) {
                    payload = {
                        selectedIds: summaryTransformationDefinitionEntity.DataRecordTypeId
                    };
                }

                VRUIUtilsService.callDirectiveLoad(dataRecordTypeSelectorAPI, payload, dataRecordTypeSelectorLoadDeferred);
            });

            return dataRecordTypeSelectorLoadDeferred.promise;
        }


        function loadDataRecordTypeFieldsSelector(selectedDataRecordTypeId) {
            var payload;
            var dataRecordTypeFieldsSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            dataRecordTypeFieldsSelectorReadyDeferred.promise.then(function () {
                if (summaryTransformationDefinitionEntity != undefined && summaryTransformationDefinitionEntity.DataRecordTypeId != undefined) {
                    payload = {
                        dataRecordTypeId: summaryTransformationDefinitionEntity.DataRecordTypeId
                    };
                }
                else if (selectedDataRecordTypeId != undefined) {
                    payload = {
                        dataRecordTypeId: selectedDataRecordTypeId
                    };
                }

                if (payload != undefined)
                    VRUIUtilsService.callDirectiveLoad(dataRecordTypeFieldsSelectorAPI, payload, dataRecordTypeFieldsSelectorLoadDeferred);
                else
                    dataRecordTypeFieldsSelectorLoadDeferred.resolve();
            });
            return dataRecordTypeFieldsSelectorLoadDeferred.promise;
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
                ItemRecordTypeId: dataRecordTypeSelectorAPI.getSelectedIds(),
                SummaryItemRecordTypeId: 1,
                TimeFieldName: '',
                SummaryIdFieldName: '',
                SummaryBatchStartFieldName: '',
                BatchRangeRetrieval: {},
                SummaryFromSettings: [],
                UpdateExistingSummaryFromNewSettings: {},
                DataRecordStorageId: 0,
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
                console.log(response)
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
