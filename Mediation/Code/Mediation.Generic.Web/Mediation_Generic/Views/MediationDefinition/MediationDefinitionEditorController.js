(function (appControllers) {

    "use strict";

    MediationDefinitionEditorController.$inject = ['$scope', 'Mediation_Generic_MediationDefinitionAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'VR_GenericData_DataRecordFieldAPIService', 'Mediation_Generic_StorageStagingStatusEnum', 'VR_GenericData_DataRecordTypeService'];

    function MediationDefinitionEditorController($scope, Mediation_Generic_MediationDefinitionAPIService, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, VR_GenericData_DataRecordFieldAPIService, Mediation_Generic_StorageStagingStatusEnum, VR_GenericData_DataRecordTypeService) {

        var isEditMode;
        var mediationDefinitionId;
        var mediationDefinitionEntity;
        var gridAPI;

        //#region Definition Tab

        var dataParsedRecordTypeSelectorAPI;
        var dataParsedRecordTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var dataParsedRecordTypeSelectedPromiseDeferred;

        var dataCookedRecordTypeSelectorAPI;
        var dataCookedRecordTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var dataCookedRecordTypeSelectedPromiseDeferred;

        //#endregion

        //#region Parsed Identification Tab

        var dataParsedRecordTypeFieldsSessionIdSelectorAPI;
        var dataParsedRecordTypeFieldsSessionIdSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var dataParsedRecordTypeFieldsTimeSelectorAPI;
        var dataParsedRecordTypeFieldsTimeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        //#endregion

        //#region ParsedToCooked Tab

        var dataTransformationDefinitionInsertSelectorAPI;
        var dataTransformationDefinitionInsertSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var dataTransformationDefinitionInsertSelectedPromiseDeferred;

        var dataTransformationDefinitionRecordParsedSelectorAPI;
        var dataTransformationDefinitionRecordParsedSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var dataTransformationDefinitionRecordCookedSelectorAPI;
        var dataTransformationDefinitionRecordCookedSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        //#endregion

        //#region Cooked CDR Data Store Tab

        var dataStoreSelectorAPI;
        var dataStoreSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        //#endregion

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                mediationDefinitionId = parameters.MediationDefinitionId;
            }
            isEditMode = (mediationDefinitionId != undefined);
        }

        function defineScope() {

            $scope.scopeModal = {};
            $scope.scopeModal.statusMappings = [];

            //#region Definition Tab

            $scope.scopeModal.selectedParsedDataRecordType;
            $scope.scopeModal.onParsedDataRecordTypeSelectorReady = function (api) {
                dataParsedRecordTypeSelectorAPI = api;
                dataParsedRecordTypeSelectorReadyDeferred.resolve();
            };

            $scope.scopeModal.selectedCookedDataRecordType;
            $scope.scopeModal.onCookedDataRecordTypeSelectorReady = function (api) {
                dataCookedRecordTypeSelectorAPI = api;
                dataCookedRecordTypeSelectorReadyDeferred.resolve();
            };

            $scope.scopeModal.onParsedRecordTypeSelectionChanged = function () {
                var selectedParsedDataRecordTypeId = dataParsedRecordTypeSelectorAPI.getSelectedIds();
                if (selectedParsedDataRecordTypeId != undefined) {

                    var setSessionIdLoader = function (value) { $scope.scopeModal.isLoadingParsedSessionID = value };
                    var sessionIdPayload = {
                        dataRecordTypeId: selectedParsedDataRecordTypeId
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataParsedRecordTypeFieldsSessionIdSelectorAPI, sessionIdPayload, setSessionIdLoader, dataParsedRecordTypeSelectedPromiseDeferred);


                    var setTimeLoader = function (value) { $scope.scopeModal.isLoadingParsedSessionID = value };
                    var timePayload = {
                        dataRecordTypeId: selectedParsedDataRecordTypeId
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataParsedRecordTypeFieldsTimeSelectorAPI, timePayload, setTimeLoader, dataParsedRecordTypeSelectedPromiseDeferred);
                }
            }

            //#endregion

            //#region ParsedToCooked Tab

            $scope.scopeModal.selectedDataTransformationDefinitionInsert;
            $scope.scopeModal.onDataTransformationDefinitionInsertReady = function (api) {
                dataTransformationDefinitionInsertSelectorAPI = api;
                dataTransformationDefinitionInsertSelectorReadyDeferred.resolve();
            };
            $scope.scopeModal.onDataTransformationDefinitionInsertSelectionChanged = function () {
                var selectedParsedDataRecordTypeId = dataParsedRecordTypeSelectorAPI.getSelectedIds();
                var selectedCookedDataRecordTypeId = dataCookedRecordTypeSelectorAPI.getSelectedIds();

                var selectedDataTransformationDefinitionInsertId = dataTransformationDefinitionInsertSelectorAPI.getSelectedIds();
                if (selectedDataTransformationDefinitionInsertId != undefined) {
                    var setLoaderDataTransformationDefinitionInsertParsed = function (value) { $scope.scopeModal.isLoadingDataTransformationDefinitionInsertParsed = value };
                    var payloadfornDataTransformationDefinitionInsertParsed = {
                        dataTransformationDefinitionId: selectedDataTransformationDefinitionInsertId,
                        filter: { DataRecordTypeIds: [selectedParsedDataRecordTypeId], IsArray: false }
                    };

                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataTransformationDefinitionRecordParsedSelectorAPI, payloadfornDataTransformationDefinitionInsertParsed, setLoaderDataTransformationDefinitionInsertParsed, dataTransformationDefinitionInsertSelectedPromiseDeferred);

                    var setLoaderDataTransformationDefinitionInsertCooked = function (value) { $scope.scopeModal.isLoadingDataTransformationDefinitionInsertCooked = value };
                    var payloadfornDataTransformationDefinitionInsertCooked = {
                        dataTransformationDefinitionId: selectedDataTransformationDefinitionInsertId,
                        filter: { DataRecordTypeIds: [selectedCookedDataRecordTypeId], IsArray: false }
                    };

                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataTransformationDefinitionRecordCookedSelectorAPI, payloadfornDataTransformationDefinitionInsertCooked, setLoaderDataTransformationDefinitionInsertCooked, dataTransformationDefinitionInsertSelectedPromiseDeferred);

                }
                else {
                    if (dataTransformationDefinitionRecordParsedSelectorAPI != undefined)
                        dataTransformationDefinitionRecordParsedSelectorAPI.clearDataSource();

                    if (dataTransformationDefinitionRecordCookedSelectorAPI != undefined)
                        dataTransformationDefinitionRecordCookedSelectorAPI.clearDataSource();
                }
            }

            $scope.scopeModal.selectedDataTransformationDefinitionParsedRecord;
            $scope.scopeModal.onDataTransformationDefinitionParsedRecordReady = function (api) {
                dataTransformationDefinitionRecordParsedSelectorAPI = api;
                dataTransformationDefinitionRecordParsedSelectorReadyDeferred.resolve();
            };

            $scope.scopeModal.selectedDataTransformationDefinitionCookedRecord;
            $scope.scopeModal.onDataTransformationDefinitionCookedRecordReady = function (api) {
                dataTransformationDefinitionRecordCookedSelectorAPI = api;
                dataTransformationDefinitionRecordCookedSelectorReadyDeferred.resolve();
            };
            //#endregion

            //#region Parsed Identification Tab

            $scope.scopeModal.selectedParsedDataRecordTypeSessionId;
            $scope.scopeModal.onParsedDataRecordSessionIDFieldSelectorReady = function (api) {
                dataParsedRecordTypeFieldsSessionIdSelectorAPI = api;
                dataParsedRecordTypeFieldsSessionIdSelectorReadyDeferred.resolve();
            };

            $scope.scopeModal.selectedParsedDataRecordTypeTimeField;
            $scope.scopeModal.onParsedDataRecordTimeFieldSelectorReady = function (api) {
                dataParsedRecordTypeFieldsTimeSelectorAPI = api;
                dataParsedRecordTypeFieldsTimeSelectorReadyDeferred.resolve();
            };

            //#endregion

            $scope.hasSaveMediationDefinition = function () {
                if (isEditMode) {
                    return Mediation_Generic_MediationDefinitionAPIService.HasUpdateMediationDefinition();
                }
                else {
                    return Mediation_Generic_MediationDefinitionAPIService.HasAddMediationDefinition();
                }
            }

            $scope.scopeModal.SaveMediationDefinition = function () {
                $scope.scopeModal.isLoading = true;
                if (isEditMode) {
                    return updateMediationDefinition();
                }
                else {
                    return insertMediationDefinition();
                }
            };

            $scope.scopeModal.onDataRecordStorageSelectorReady = function (api) {
                dataStoreSelectorAPI = api;
                dataStoreSelectorReadyDeferred.resolve();
            };

            $scope.scopeModal.onGridReady = function (api) {
                gridAPI = api;
            }

            $scope.scopeModal.close = function () {
                $scope.modalContext.closeModal()
            };

        }

        function load() {

            $scope.scopeModal.isLoading = true;
            defineMenuActions();

            if (isEditMode) {
                getMediationDefinition().then(function () {
                    loadDataRecordTypes().then(function () {
                        loadAllControls().finally(function () {
                            mediationDefinitionEntity = undefined;
                        });
                    });

                }).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModal.isLoading = false;
                });
            }
            else {
                loadDataRecordTypes().then(function () {
                    loadAllControls();
                });
            }
        }

        function getMediationDefinition() {
            return Mediation_Generic_MediationDefinitionAPIService.GetMediationDefinition(mediationDefinitionId)
                .then(function (mediationDefinition) {
                    mediationDefinitionEntity = mediationDefinition;
                });
        }

        function loadDataRecordTypes() {

            var promises = [];

            //#region  Load Parsed Data Record Type

            var dataParsedRecordTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            promises.push(dataParsedRecordTypeSelectorLoadDeferred.promise);

            var payloadParsedRecordTypeSelector;
            if (mediationDefinitionEntity != undefined && mediationDefinitionEntity.ParsedRecordTypeId != undefined) {
                dataParsedRecordTypeSelectedPromiseDeferred = UtilsService.createPromiseDeferred();

                payloadParsedRecordTypeSelector = {
                    selectedIds: mediationDefinitionEntity.ParsedRecordTypeId
                };
            }

            dataParsedRecordTypeSelectorReadyDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(dataParsedRecordTypeSelectorAPI, payloadParsedRecordTypeSelector, dataParsedRecordTypeSelectorLoadDeferred);
            });


            //#endregion

            //#region Load Cooked Data Record Type
            var dataCookedRecordTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            promises.push(dataCookedRecordTypeSelectorLoadDeferred.promise);

            var payloadCookedRecordTypeSelector;
            if (mediationDefinitionEntity != undefined && mediationDefinitionEntity.CookedRecordTypeId != undefined) {
                dataCookedRecordTypeSelectedPromiseDeferred = UtilsService.createPromiseDeferred();

                payloadCookedRecordTypeSelector = {
                    selectedIds: mediationDefinitionEntity.CookedRecordTypeId
                };
            }

            dataCookedRecordTypeSelectorReadyDeferred.promise.then(function () {

                VRUIUtilsService.callDirectiveLoad(dataCookedRecordTypeSelectorAPI, payloadCookedRecordTypeSelector, dataCookedRecordTypeSelectorLoadDeferred);
            });

            //#endregion

            return UtilsService.waitMultiplePromises(promises);
        }

        function loadParsedToCookedSection() {
            var promises = [];

            var dataTransformationDefinitionInsertSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            promises.push(dataTransformationDefinitionInsertSelectorLoadDeferred.promise);

            var payload;
            if (mediationDefinitionEntity != undefined && mediationDefinitionEntity.CookedFromParsedSettings != undefined) {
                dataTransformationDefinitionInsertSelectedPromiseDeferred = UtilsService.createPromiseDeferred();

                payload = {
                    selectedIds: mediationDefinitionEntity.CookedFromParsedSettings.TransformationDefinitionId
                };
            }

            var payloadParsedRecordTypeSelector;

            dataTransformationDefinitionInsertSelectorReadyDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(dataTransformationDefinitionInsertSelectorAPI, payload, dataTransformationDefinitionInsertSelectorLoadDeferred);
            });

            //#region Load Existing Setting

            if (mediationDefinitionEntity != undefined && mediationDefinitionEntity.CookedFromParsedSettings != undefined && mediationDefinitionEntity.CookedFromParsedSettings.TransformationDefinitionId != undefined) {

                var dataTransformationDefinitionRecordParsedInsertSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                promises.push(dataTransformationDefinitionRecordParsedInsertSelectorLoadDeferred.promise);
                UtilsService.waitMultiplePromises([dataTransformationDefinitionInsertSelectedPromiseDeferred.promise, dataTransformationDefinitionRecordParsedSelectorReadyDeferred.promise]).then(function () {
                    var payload;
                    payload = {
                        dataTransformationDefinitionId: mediationDefinitionEntity.CookedFromParsedSettings.TransformationDefinitionId,
                        filter: undefined,
                        selectedIds: mediationDefinitionEntity.CookedFromParsedSettings.ParsedRecordName
                    };
                    VRUIUtilsService.callDirectiveLoad(dataTransformationDefinitionRecordParsedSelectorAPI, payload, dataTransformationDefinitionRecordParsedInsertSelectorLoadDeferred);
                    dataTransformationDefinitionInsertSelectedPromiseDeferred = undefined;
                });


                var dataTransformationDefinitionRecordCookedSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                promises.push(dataTransformationDefinitionRecordCookedSelectorLoadDeferred.promise);
                UtilsService.waitMultiplePromises([dataTransformationDefinitionInsertSelectedPromiseDeferred.promise, dataTransformationDefinitionRecordCookedSelectorReadyDeferred.promise]).then(function () {
                    var payload;
                    payload = {
                        dataTransformationDefinitionId: mediationDefinitionEntity.CookedFromParsedSettings.TransformationDefinitionId,
                        filter: undefined,
                        selectedIds: mediationDefinitionEntity.CookedFromParsedSettings.CookedRecordName
                    };
                    VRUIUtilsService.callDirectiveLoad(dataTransformationDefinitionRecordCookedSelectorAPI, payload, dataTransformationDefinitionRecordCookedSelectorLoadDeferred);
                    dataTransformationDefinitionInsertSelectedPromiseDeferred = undefined;
                });

            }

            //#endregion

            return UtilsService.waitMultiplePromises(promises);
        }

        function loadParsedIdentificationSection() {

        }

        function loadDataStoreSelector() {
            var dataStoreSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            dataStoreSelectorReadyDeferred.promise.then(function () {
                var payload;

                if (mediationDefinitionEntity != undefined && mediationDefinitionEntity.CookedCDRDataStoreSetting != undefined) {
                    payload = {
                        selectedIds: mediationDefinitionEntity.CookedCDRDataStoreSetting.DataRecordStorageId,
                        showaddbutton: true
                    };
                }

                VRUIUtilsService.callDirectiveLoad(dataStoreSelectorAPI, payload, dataStoreSelectorLoadDeferred);
            });

            return dataStoreSelectorLoadDeferred.promise;
        }


        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadDefinitionSection, setTitle, loadParsedToCookedSection, PrepareStatusMappings, loadDataStoreSelector])
                 .catch(function (error) {
                     VRNotificationService.notifyExceptionWithClose(error, $scope);
                 })
                .finally(function () {
                    $scope.scopeModal.isLoading = false;
                });
        }

        function loadDefinitionSection() {
            if (mediationDefinitionEntity != undefined) {
                $scope.scopeModal.name = mediationDefinitionEntity.Name;
            }
        }

        function setTitle() {
            if (isEditMode && mediationDefinitionEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(mediationDefinitionEntity.Name, 'Mediation Setting Definition');
            else
                $scope.title = UtilsService.buildTitleForAddEditor('Mediation Setting Definition');
        }

        function insertMediationDefinition() {
            var mediationDefinitionObject = buildMediationDefinitionObjFromScope();
            return Mediation_Generic_MediationDefinitionAPIService.AddMediationDefinition(mediationDefinitionObject)
            .then(function (response) {
                $scope.scopeModal.isLoading = false;
                if (VRNotificationService.notifyOnItemAdded("Mediation Definition", response, "Name")) {
                    if ($scope.onMediationDefinitionAdded != undefined)
                        $scope.onMediationDefinitionAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });

        }

        function updateMediationDefinition() {
            var mediationDefinitionObject = buildMediationDefinitionObjFromScope();
            Mediation_Generic_MediationDefinitionAPIService.UpdateMediationDefinition(mediationDefinitionObject)
            .then(function (response) {
                $scope.scopeModal.isLoading = false;
                if (VRNotificationService.notifyOnItemUpdated("Mediation Definition", response, "Name")) {
                    if ($scope.onMediationDefinitionUpdated != undefined)
                        $scope.onMediationDefinitionUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }

        function buildMediationDefinitionObjFromScope() {
            var item = {
                Name: $scope.scopeModal.name,
                MediationDefinitionId: mediationDefinitionId,
                ParsedRecordTypeId: dataParsedRecordTypeSelectorAPI.getSelectedIds(),
                CookedRecordTypeId: dataCookedRecordTypeSelectorAPI.getSelectedIds(),
                ParsedRecordIdentificationSetting: getParsedRecordIdentificationSetting(),
                CookedFromParsedSettings: getCookedFromParsedSettings()
            }
            return item;
        }

        function addFilter(dataItem) {

            $scope.scopeModal.isLoading = true;
            loadFields().then(function (response) {
                if (response) {
                    var fields = [];
                    for (var i = 0; i < response.length; i++) {
                        var dataRecordField = response[i];
                        fields.push({
                            FieldName: dataRecordField.Entity.Name,
                            FieldTitle: dataRecordField.Entity.Title,
                            Type: dataRecordField.Entity.Type,
                        });
                    }
                    $scope.scopeModal.isLoading = false;

                    var onDataRecordFieldTypeFilterAdded = function (filter, expression) {
                        var obj = {
                            FilterObj: filter,
                            Expression: expression,
                            Status: dataItem.Status,
                            Id: dataItem.Id
                        };
                        gridAPI.itemUpdated(obj);
                    }
                    VR_GenericData_DataRecordTypeService.addDataRecordTypeFieldFilter(fields, dataItem.FilterObj, onDataRecordFieldTypeFilterAdded);
                }
            });
        }

        function resetFilter(dataItem) {
            dataItem.Expression = undefined;
            dataItem.FilterObj = null;
        }

        function defineMenuActions() {
            $scope.scopeModal.gridMenuActions = [{
                name: "Edit Condition",
                clicked: addFilter
            },
            {
                name: "Clear Condition",
                clicked: resetFilter
            }
            ];
        }

        function loadFields() {
            var obj = { DataRecordTypeId: dataParsedRecordTypeSelectorAPI.getSelectedIds() };
            var serializedFilter = UtilsService.serializetoJson(obj);
            return VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldsInfo(serializedFilter);
        }

        function getParsedRecordIdentificationSetting() {
            return {
                SessionIdField: dataParsedRecordTypeFieldsSessionIdSelectorAPI.getSelectedIds(),
                EventTimeField: dataParsedRecordTypeFieldsTimeSelectorAPI.getSelectedIds(),
                StatusMappings: getStatusMappings()
            };
        }

        function getCookedFromParsedSettings() {
            return {
                TransformationDefinitionId: dataTransformationDefinitionInsertSelectorAPI.getSelectedIds(),
                ParsedRecordName: dataTransformationDefinitionRecordParsedSelectorAPI.getSelectedIds(),
                CookedRecordName: dataTransformationDefinitionRecordCookedSelectorAPI.getSelectedIds()
            };
        }

        function getStatusMappings() {
            var mappings = [];
            for (var i = 0; i < $scope.scopeModal.statusMappings.length; i++) {
                var mapping = $scope.scopeModal.statusMappings[i];
                mappings.push({
                    Status: mapping.Status,
                    FilterGroup: mapping.FilterObj,
                    FilterExpression: mapping.Expression
                });
            }
            return mappings;
        }

        function PrepareStatusMappings() {
            var stagingStatusEnums = UtilsService.getArrayEnum(Mediation_Generic_StorageStagingStatusEnum);
            if (mediationDefinitionEntity != undefined && mediationDefinitionEntity.ParsedRecordIdentificationSetting != undefined) {
                var statusMappings = mediationDefinitionEntity.ParsedRecordIdentificationSetting.StatusMappings;
                for (var i = 0; i < statusMappings.length; i++) {
                    var statusMappingObj = {
                        Status: stagingStatusEnums[i].description,
                        Expression: statusMappings[i].FilterExpression,
                        FilterObj: statusMappings[i].Filters,
                        Id: i
                    };
                    $scope.scopeModal.statusMappings.push(statusMappingObj);
                }
            }
            else {

                for (var i = 0; i < stagingStatusEnums.length; i++) {
                    var statusMappingObj = {
                        Status: stagingStatusEnums[i].description,
                        Expression: undefined,
                        FilterObj: undefined,
                        Id: i
                    };

                    $scope.scopeModal.statusMappings.push(statusMappingObj);
                }
            }
        }
    }

    appControllers.controller('Mediation_Generic_MediationDefinitionEditorController', MediationDefinitionEditorController);
})(appControllers);
