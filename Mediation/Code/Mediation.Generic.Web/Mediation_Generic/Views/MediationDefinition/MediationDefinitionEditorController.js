(function (appControllers) {

    "use strict";

    MediationDefinitionEditorController.$inject = ['$scope', 'Mediation_Generic_MediationDefinitionAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'VR_GenericData_DataRecordFieldAPIService', 'Mediation_Generic_StorageStagingStatusEnum', 'VR_GenericData_DataRecordTypeService', 'Mediation_Generic_OutPutHandlerService'];

    function MediationDefinitionEditorController($scope, Mediation_Generic_MediationDefinitionAPIService, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, VR_GenericData_DataRecordFieldAPIService, Mediation_Generic_StorageStagingStatusEnum, VR_GenericData_DataRecordTypeService, Mediation_Generic_OutPutHandlerService) {

        var isEditMode;
        var mediationDefinitionId;
        var mediationDefinitionEntity;
        var gridAPI;
        var configTypes = [];
        //#region Definition Tab

        var dataParsedRecordTypeSelectorAPI;
        var dataParsedRecordTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var dataParsedRecordTypeSelectedPromiseDeferred;

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

            $scope.scopeModel = {};
            $scope.scopeModel.statusMappings = [];
            $scope.scopeModel.selectedStatusMappings = [];
           

            //#region Definition Tab

            $scope.scopeModel.selectedParsedDataRecordType;
            $scope.scopeModel.onParsedDataRecordTypeSelectorReady = function (api) {
                dataParsedRecordTypeSelectorAPI = api;
                dataParsedRecordTypeSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.selectedCookedDataRecordType;
           

            $scope.scopeModel.onParsedRecordTypeSelectionChanged = function () {
                var selectedParsedDataRecordTypeId = dataParsedRecordTypeSelectorAPI.getSelectedIds();
                if (selectedParsedDataRecordTypeId != undefined) {

                    var setSessionIdLoader = function (value) { $scope.scopeModel.isLoadingParsedSessionID = value };
                    var sessionIdPayload = {
                        dataRecordTypeId: selectedParsedDataRecordTypeId
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataParsedRecordTypeFieldsSessionIdSelectorAPI, sessionIdPayload, setSessionIdLoader, dataParsedRecordTypeSelectedPromiseDeferred);


                    var setTimeLoader = function (value) { $scope.scopeModel.isLoadingParsedSessionID = value };
                    var timePayload = {
                        dataRecordTypeId: selectedParsedDataRecordTypeId
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataParsedRecordTypeFieldsTimeSelectorAPI, timePayload, setTimeLoader, dataParsedRecordTypeSelectedPromiseDeferred);
                }
            }

            //#endregion

            //#region ParsedToCooked Tab

            $scope.scopeModel.selectedDataTransformationDefinitionInsert;
            $scope.scopeModel.onDataTransformationDefinitionInsertReady = function (api) {
                dataTransformationDefinitionInsertSelectorAPI = api;
                dataTransformationDefinitionInsertSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onDataTransformationDefinitionInsertSelectionChanged = function () {
                var selectedParsedDataRecordTypeId = dataParsedRecordTypeSelectorAPI.getSelectedIds();

                var selectedDataTransformationDefinitionInsertId = dataTransformationDefinitionInsertSelectorAPI.getSelectedIds();
                if (selectedDataTransformationDefinitionInsertId != undefined) {
                    var setLoaderDataTransformationDefinitionInsertParsed = function (value) { $scope.scopeModel.isLoadingDataTransformationDefinitionInsertParsed = value };
                    var payloadfornDataTransformationDefinitionInsertParsed = {
                        dataTransformationDefinitionId: selectedDataTransformationDefinitionInsertId,
                        filter: { DataRecordTypeIds: [selectedParsedDataRecordTypeId], IsArray: true }
                    };

                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataTransformationDefinitionRecordParsedSelectorAPI, payloadfornDataTransformationDefinitionInsertParsed, setLoaderDataTransformationDefinitionInsertParsed, dataTransformationDefinitionInsertSelectedPromiseDeferred);

                }
                else {
                    if (dataTransformationDefinitionRecordParsedSelectorAPI != undefined)
                        dataTransformationDefinitionRecordParsedSelectorAPI.clearDataSource();
                   }
            }

            $scope.scopeModel.selectedDataTransformationDefinitionParsedRecord;
            $scope.scopeModel.onDataTransformationDefinitionParsedRecordReady = function (api) {
                dataTransformationDefinitionRecordParsedSelectorAPI = api;
                dataTransformationDefinitionRecordParsedSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.selectedDataTransformationDefinitionCookedRecord;
          
            //#endregion

            //#region Parsed Identification Tab

            $scope.scopeModel.selectedParsedDataRecordTypeSessionId;
            $scope.scopeModel.onParsedDataRecordSessionIDFieldSelectorReady = function (api) {
                dataParsedRecordTypeFieldsSessionIdSelectorAPI = api;
                dataParsedRecordTypeFieldsSessionIdSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.selectedParsedDataRecordTypeTimeField;
            $scope.scopeModel.onParsedDataRecordTimeFieldSelectorReady = function (api) {
                dataParsedRecordTypeFieldsTimeSelectorAPI = api;
                dataParsedRecordTypeFieldsTimeSelectorReadyDeferred.resolve();
            };

            //#endregion

            //#region handlers Grid
            $scope.scopeModel.handlers = [];
            $scope.scopeModel.addHandler = function () {
                var onOutPutHandlerAdded = function (obj) {
                    console.log(obj)
                    $scope.scopeModel.handlers.push(buildHandlerRecord(obj));
                };
                Mediation_Generic_OutPutHandlerService.addOutPutHandler(onOutPutHandlerAdded);
            };
            $scope.scopeModel.handlersGridMenuActions = function () {
                return [{
                    name: "Edit",
                    clicked: function (dataItem) {
                        var onOutPutHandlerUpdated = function (obj) {
                            console.log(obj)
                            $scope.scopeModel.handlers[$scope.scopeModel.handlers.indexOf(dataItem)] = buildHandlerRecord(obj);
                        };
                        Mediation_Generic_OutPutHandlerService.editOutPutHandler(dataItem, onOutPutHandlerUpdated);
                    }
                }];
            };
            $scope.scopeModel.removeHandler = function (obj) {
                $scope.scopeModel.handlers.splice($scope.scopeModel.handlers.indexOf(obj), 1);
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

            $scope.scopeModel.SaveMediationDefinition = function () {
                $scope.scopeModel.isLoading = true;
                if (isEditMode) {
                    return updateMediationDefinition();
                }
                else {
                    return insertMediationDefinition();
                }
            };

           
            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
            }

            $scope.scopeModel.ValidateStatusConditions = function () {

                if ($scope.scopeModel.selectedStatusMappings.length > 0) {
                    var listLength = $scope.scopeModel.selectedStatusMappings.length;
                    var firstItem = $scope.scopeModel.selectedStatusMappings[0];
                    var lastItem = $scope.scopeModel.selectedStatusMappings[listLength - 1];
                    if (firstItem.FilterObj == undefined)
                        return 'First Event Condition should be filled.';
                    else if (listLength > 0 && lastItem.FilterObj != undefined)
                        return 'Last Event Condition should be empty.';
                }
                else
                    return 'At least one event should be selected.';
                return null;

            }

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };

        }

        function load() {

            $scope.scopeModel.isLoading = true;
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
                    $scope.scopeModel.isLoading = false;
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
          
            var payloadCookedRecordTypeSelector;
            if (mediationDefinitionEntity != undefined && mediationDefinitionEntity.CookedRecordTypeId != undefined) {
                dataCookedRecordTypeSelectedPromiseDeferred = UtilsService.createPromiseDeferred();

                payloadCookedRecordTypeSelector = {
                    selectedIds: mediationDefinitionEntity.CookedRecordTypeId
                };


                var dataSessionIdSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                promises.push(dataSessionIdSelectorLoadDeferred.promise);
                UtilsService.waitMultiplePromises([dataParsedRecordTypeSelectedPromiseDeferred.promise, dataParsedRecordTypeFieldsSessionIdSelectorReadyDeferred.promise]).then(function () {
                    var payloadDataRecordTypeSessionId;
                    payloadDataRecordTypeSessionId = {
                        dataRecordTypeId: mediationDefinitionEntity.ParsedRecordTypeId,
                        selectedIds: mediationDefinitionEntity.ParsedRecordIdentificationSetting.SessionIdField
                    };
                    VRUIUtilsService.callDirectiveLoad(dataParsedRecordTypeFieldsSessionIdSelectorAPI, payloadDataRecordTypeSessionId, dataSessionIdSelectorLoadDeferred);
                    dataParsedRecordTypeSelectedPromiseDeferred = undefined;
                });


                var dataTimeFieldSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                promises.push(dataTimeFieldSelectorLoadDeferred.promise);
                UtilsService.waitMultiplePromises([dataParsedRecordTypeSelectedPromiseDeferred.promise, dataParsedRecordTypeFieldsTimeSelectorReadyDeferred.promise]).then(function () {
                    var payloadDataRecordTypeTimeFields = {
                        dataRecordTypeId: mediationDefinitionEntity.ParsedRecordTypeId,
                        selectedIds: mediationDefinitionEntity.ParsedRecordIdentificationSetting.EventTimeField
                    };
                    VRUIUtilsService.callDirectiveLoad(dataParsedRecordTypeFieldsTimeSelectorAPI, payloadDataRecordTypeTimeFields, dataTimeFieldSelectorLoadDeferred);
                    dataParsedRecordTypeSelectedPromiseDeferred = undefined;
                });
            }


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
                        filter: { IsArray: true },
                        selectedIds: mediationDefinitionEntity.CookedFromParsedSettings.ParsedRecordName
                    };
                    VRUIUtilsService.callDirectiveLoad(dataTransformationDefinitionRecordParsedSelectorAPI, payload, dataTransformationDefinitionRecordParsedInsertSelectorLoadDeferred);
                    dataTransformationDefinitionInsertSelectedPromiseDeferred = undefined;
                });


               
                UtilsService.waitMultiplePromises([dataTransformationDefinitionInsertSelectedPromiseDeferred.promise]).then(function () {
                    var payload;
                    payload = {
                        dataTransformationDefinitionId: mediationDefinitionEntity.CookedFromParsedSettings.TransformationDefinitionId,
                        filter: { IsArray: false },
                        selectedIds: mediationDefinitionEntity.CookedFromParsedSettings.CookedRecordName
                    };
                    dataTransformationDefinitionInsertSelectedPromiseDeferred = undefined;
                });

            }

            //#endregion

            return UtilsService.waitMultiplePromises(promises);
        }

        function loadParsedIdentificationSection() {

        }

        

        function loadHandlerGrid() {
            
            return Mediation_Generic_MediationDefinitionAPIService.GetMediationHandlerConfigTypes().then(function (response) {
               
                configTypes = response;


                if(mediationDefinitionEntity!=undefined && mediationDefinitionEntity.OutputHandlers!=null && mediationDefinitionEntity.OutputHandlers.length >0  )
                    $scope.scopeModel.handlers =  buildHandlersGridData(mediationDefinitionEntity.OutputHandlers);
            });
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadDefinitionSection, setTitle, loadParsedToCookedSection, PrepareStatusMappings ,loadHandlerGrid])
                 .catch(function (error) {
                     VRNotificationService.notifyExceptionWithClose(error, $scope);
                 })
                .finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
        }

        function loadDefinitionSection() {
            if (mediationDefinitionEntity != undefined) {
                $scope.scopeModel.name = mediationDefinitionEntity.Name;
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
                $scope.scopeModel.isLoading = false;
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
                $scope.scopeModel.isLoading = false;
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
                Name: $scope.scopeModel.name,
                MediationDefinitionId: mediationDefinitionId,
                ParsedRecordTypeId: dataParsedRecordTypeSelectorAPI.getSelectedIds(),
                ParsedRecordIdentificationSetting: getParsedRecordIdentificationSetting(),
                CookedFromParsedSettings: getCookedFromParsedSettings(),
                OutputHandlers:buildHandlersGridData($scope.scopeModel.handlers)
            }
            return item;
        }
        function buildHandlersGridData(handlers) {
            var tab = [];
            for (var i = 0 ; i < handlers.length ; i++) {
                var obj = buildHandlerRecord(handlers[i]);
                tab.push(obj);
            }
            return tab;
        }
        function buildHandlerRecord(rec) {
            var handlerTypeConfig =( rec.Handler!=null)? UtilsService.getItemByVal(configTypes, rec.Handler.ConfigId, "ExtensionConfigurationId") : null;
            return {
                OutputRecordName:rec.OutputRecordName,
                Handler: rec.Handler,
                HandlerType: handlerTypeConfig!=null && handlerTypeConfig.Title || null
            };
        }

        function addFilter(dataItem) {

            $scope.scopeModel.isLoading = true;
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
                    $scope.scopeModel.isLoading = false;

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
            $scope.scopeModel.gridMenuActions = [{
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
                ParsedRecordName: dataTransformationDefinitionRecordParsedSelectorAPI.getSelectedIds()
            };
        }

        function getCookedCDRDataStoreSetting() {
            return {
                DataRecordStorageId: dataStoreSelectorAPI.getSelectedIds()
            };
        }

        function getStatusMappings() {
            var mappings = [];
            for (var i = 0; i < $scope.scopeModel.selectedStatusMappings.length; i++) {
                var mapping = $scope.scopeModel.selectedStatusMappings[i];
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

            for (var i = 0; i < stagingStatusEnums.length; i++) {
                var statusMappingObj = {
                    Status: stagingStatusEnums[i].description,
                    Expression: undefined,
                    FilterObj: undefined,
                    Id: stagingStatusEnums[i].value
                };

                $scope.scopeModel.statusMappings.push(statusMappingObj);

            }

            if (mediationDefinitionEntity != undefined && mediationDefinitionEntity.ParsedRecordIdentificationSetting != undefined) {
                var statusMappings = mediationDefinitionEntity.ParsedRecordIdentificationSetting.StatusMappings;
                for (var i = 0; i < statusMappings.length; i++) {
                    var index = statusMappings[i].Status;
                    var statusMappingObj = {
                        Status: stagingStatusEnums[index].description,
                        Expression: statusMappings[i].FilterExpression,
                        FilterObj: statusMappings[i].FilterGroup,
                        Id: stagingStatusEnums[index].value
                    };
                    $scope.scopeModel.selectedStatusMappings.push(statusMappingObj);
                }
            }
            else {
                loadDefaultEventStatusMappings();
            }

        }

        function loadDefaultEventStatusMappings() {
            var stagingStatusEnums = UtilsService.getArrayEnum(Mediation_Generic_StorageStagingStatusEnum)
            $scope.scopeModel.selectedStatusMappings.push(getStatusFromEnumObj(stagingStatusEnums, 'Start'));
            $scope.scopeModel.selectedStatusMappings.push(getStatusFromEnumObj(stagingStatusEnums, 'Stop'));
        }

        function getStatusFromEnumObj(enumArray, filterValue) {
            var enumObj = UtilsService.getEnum(enumArray, 'description', filterValue);
            return {
                Status: enumObj.description,
                Expression: undefined,
                FilterObj: undefined,
                Id: enumObj.value
            };
        }
    }

    appControllers.controller('Mediation_Generic_MediationDefinitionEditorController', MediationDefinitionEditorController);
})(appControllers);
