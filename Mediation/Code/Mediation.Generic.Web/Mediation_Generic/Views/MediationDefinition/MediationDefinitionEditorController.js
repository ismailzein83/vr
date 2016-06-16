(function (appControllers) {

    "use strict";

    MediationDefinitionEditorController.$inject = ['$scope', 'Mediation_Generic_MediationDefinitionAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

    function MediationDefinitionEditorController($scope, Mediation_Generic_MediationDefinitionAPIService, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

        var isEditMode;
        var mediationDefinitionId;
        var mediationDefinitionEntity;

        //#region Definition Tab

        var dataParsedRecordTypeSelectorAPI;
        var dataParsedRecordTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var dataParsedRecordTypeSelectedPromiseDeferred;

        var dataCookedRecordTypeSelectorAPI;
        var dataCookedRecordTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var dataCookedRecordTypeSelectedPromiseDeferred;

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

            $scope.scopeModal.close = function () {
                $scope.modalContext.closeModal()
            };

        }

        function load() {

            $scope.scopeModal.isLoading = true;

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
            if (mediationDefinitionEntity != undefined && mediationDefinitionEntity.ParsedItemRecordTypeId != undefined) {
                dataParsedRecordTypeSelectedPromiseDeferred = UtilsService.createPromiseDeferred();

                payloadParsedRecordTypeSelector = {
                    selectedIds: mediationDefinitionEntity.ParsedItemRecordTypeId
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
            if (mediationDefinitionEntity != undefined && mediationDefinitionEntity.CookedItemRecordTypeId != undefined) {
                dataCookedRecordTypeSelectedPromiseDeferred = UtilsService.createPromiseDeferred();

                payloadCookedRecordTypeSelector = {
                    selectedIds: mediationDefinitionEntity.CookedItemRecordTypeId
                };
            }

            dataCookedRecordTypeSelectorReadyDeferred.promise.then(function () {

                VRUIUtilsService.callDirectiveLoad(dataCookedRecordTypeSelectorAPI, payloadCookedRecordTypeSelector, dataCookedRecordTypeSelectorLoadDeferred);
            });

            if (mediationDefinitionEntity != undefined) {


            }
            //#endregion

            return UtilsService.waitMultiplePromises(promises);
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadDefinitionSection, setTitle, loadParsedToCookedSection])
                 .catch(function (error) {
                     VRNotificationService.notifyExceptionWithClose(error, $scope);
                 })
                .finally(function () {
                    $scope.scopeModal.isLoading = false;
                });
        }

        function loadParsedToCookedSection() {


            var promises = [];

            var dataTransformationDefinitionInsertSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            promises.push(dataTransformationDefinitionInsertSelectorLoadDeferred.promise);

            var payload;
            if (mediationDefinitionEntity != undefined && mediationDefinitionEntity.CookedFromParsedSettings != undefined) {
                dataTransformationDefinitionInsertSelectedPromiseDeferred = UtilsService.createPromiseDeferred();

                payload = {
                    selectedIds: mediationDefinitionEntity.CookedFromParsedSettings.CookedFromParsedSettings.TransformationDefinitionId
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
                CookedFromParsedSettings: {
                    TransformationDefinitionId: dataTransformationDefinitionInsertSelectorAPI.getSelectedIds(),
                    ParsedRecordName: dataTransformationDefinitionRecordParsedSelectorAPI.getSelectedIds(),
                    CookedRecordName: dataTransformationDefinitionRecordCookedSelectorAPI.getSelectedIds()
                }
            }
            return item;
        }

    }

    appControllers.controller('Mediation_Generic_MediationDefinitionEditorController', MediationDefinitionEditorController);
})(appControllers);
