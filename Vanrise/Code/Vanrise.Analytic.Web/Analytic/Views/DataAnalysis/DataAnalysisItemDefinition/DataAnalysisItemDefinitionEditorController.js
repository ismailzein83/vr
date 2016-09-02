(function (appControllers) {

    "use strict";

    DataAnalysisItemDefinitionEditorController.$inject = ['$scope', 'VR_Analytic_DataAnalysisItemDefinitionAPIService', 'VR_Analytic_DataAnalysisDefinitionAPIService', 'VR_GenericData_DataRecordTypeAPIService', 'VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService'];

    function DataAnalysisItemDefinitionEditorController($scope, VR_Analytic_DataAnalysisItemDefinitionAPIService, VR_Analytic_DataAnalysisDefinitionAPIService, VR_GenericData_DataRecordTypeAPIService, VRNotificationService, UtilsService, VRUIUtilsService, VRNavigationService) {

        var isEditMode;

        var dataAnalysisItemDefinitionId;
        var dataAnalysisItemDefinitionEntity;
        var dataAnalysisDefinitionId;
        var itemDefinitionTypeId;

        var dataRecordTypeId;
        var dataRecordTypeEntity;

        var dataRecordTypeSelectorAPI;
        var dataRecordTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var dataRecordTypeSelectionChangedDeferred;

        var settingsDirectiveAPI;
        var settingsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();


        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                dataAnalysisItemDefinitionId = parameters.dataAnalysisItemDefinitionId;
                dataAnalysisDefinitionId = parameters.dataAnalysisDefinitionId
                itemDefinitionTypeId = parameters.itemDefinitionTypeId
            }

            isEditMode = (dataAnalysisItemDefinitionId != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onDataRecordTypeSelectorReady = function (api) {
                dataRecordTypeSelectorAPI = api;
                dataRecordTypeSelectorReadyDeferred.resolve();
            }
            $scope.scopeModel.onDataRecordTypeSelectionChanged = function () {
                var selectedId = dataRecordTypeSelectorAPI.getSelectedIds();
                if (selectedId != undefined) {

                    if (dataRecordTypeSelectionChangedDeferred != undefined) {
                        dataRecordTypeSelectionChangedDeferred.resolve();
                    }
                    else {
                        dataRecordTypeId = selectedId;

                        var settingsDirectivePayload = {};
                        //settingsDirectivePayload.dataAnalysisItemDefinition = dataAnalysisItemDefinitionEntity;
                        settingsDirectivePayload.dataAnalysisDefinitionId = dataAnalysisDefinitionId;
                        settingsDirectivePayload.itemDefinitionTypeId = itemDefinitionTypeId;
                        settingsDirectivePayload.context = buildContext();

                        var setLoader = function (value) {
                            $scope.scopeModel.isSelectorLoading = value;
                        };
                        //VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataRecordTypeSelectorAPI, undefined, setLoader);
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, settingsDirectiveAPI, settingsDirectivePayload, setLoader);
                    }
                }
            };

            $scope.scopeModel.onSettingsDirectiveReady = function (api) {
                settingsDirectiveAPI = api;
                settingsDirectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.save = function () {
                if (isEditMode) {
                    return update();
                }
                else {
                    return insert();
                }
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };
        }
        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                getDataAnalysisItemDefinition().then(function () {
                    loadAllControls();
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }
        }

        function getDataAnalysisItemDefinition() {
            return VR_Analytic_DataAnalysisItemDefinitionAPIService.GetDataAnalysisItemDefinition(dataAnalysisItemDefinitionId).then(function (response) {
                dataAnalysisItemDefinitionEntity = response;
            });
        }
        function getDataRecordType() {
            return VR_GenericData_DataRecordTypeAPIService.GetDataRecordType(dataRecordTypeId).then(function (response) {
                dataRecordTypeEntity = response;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadDataRecordTypeSelector, loadSettingsDirective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

            function setTitle() {
                if (isEditMode) {
                    var dataAnalysisItemDefinitionName = (dataAnalysisItemDefinitionEntity != undefined) ? dataAnalysisItemDefinitionEntity.Name : null;
                    $scope.title = UtilsService.buildTitleForUpdateEditor(dataAnalysisItemDefinitionName, 'Data Analysis Item Definition');
                }
                else {
                    $scope.title = UtilsService.buildTitleForAddEditor('Data Analysis Item Definition');
                }
            }
            function loadStaticData() {
                if (dataAnalysisItemDefinitionEntity == undefined)
                    return;

                $scope.scopeModel.name = dataAnalysisItemDefinitionEntity.Name;
                $scope.scopeModel.title = dataAnalysisItemDefinitionEntity.Settings.Title;
            }
            function loadDataRecordTypeSelector() {
                var dataRecordTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                dataRecordTypeSelectorReadyDeferred.promise.then(function () {
                    var dataRecordTypeSelectorPayload = {};
                    if (dataAnalysisItemDefinitionEntity != undefined && dataAnalysisItemDefinitionEntity.Settings != undefined) {
                        dataRecordTypeId = dataAnalysisItemDefinitionEntity.Settings.RecordTypeId
                        dataRecordTypeSelectorPayload.selectedIds = dataRecordTypeId;
                    }

                    VRUIUtilsService.callDirectiveLoad(dataRecordTypeSelectorAPI, dataRecordTypeSelectorPayload, dataRecordTypeSelectorLoadDeferred);
                });

                return dataRecordTypeSelectorLoadDeferred.promise;
            }
            function loadSettingsDirective() {
                var settingsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                settingsDirectiveReadyDeferred.promise.then(function () {

                    dataRecordTypeSelectionChangedDeferred = UtilsService.createPromiseDeferred();
                    dataRecordTypeSelectionChangedDeferred.promise.then(function () {
                        dataRecordTypeSelectionChangedDeferred = undefined;

                        var settingsDirectivePayload = {};
                        settingsDirectivePayload.dataAnalysisItemDefinition = dataAnalysisItemDefinitionEntity;
                        settingsDirectivePayload.dataAnalysisDefinitionId = dataAnalysisDefinitionId;
                        settingsDirectivePayload.itemDefinitionTypeId = itemDefinitionTypeId;
                        settingsDirectivePayload.context = buildContext();

                        VRUIUtilsService.callDirectiveLoad(settingsDirectiveAPI, settingsDirectivePayload, settingsDirectiveLoadDeferred);
                    });
                });

                return settingsDirectiveLoadDeferred.promise;
            }

        }

        function insert() {
            $scope.scopeModel.isLoading = true;
            return VR_Analytic_DataAnalysisItemDefinitionAPIService.AddDataAnalysisItemDefinition(buildDataAnalysisItemDefinitionObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('DataAnalysisItemDefinition', response, 'Name')) {
                    if ($scope.onDataAnalysisItemDefinitionAdded != undefined)
                        $scope.onDataAnalysisItemDefinitionAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function update() {
            $scope.scopeModel.isLoading = true;
            return VR_Analytic_DataAnalysisItemDefinitionAPIService.UpdateDataAnalysisItemDefinition(buildDataAnalysisItemDefinitionObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('DataAnalysisItemDefinition', response, 'Name')) {
                    if ($scope.onDataAnalysisItemDefinitionUpdated != undefined) {
                        $scope.onDataAnalysisItemDefinitionUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildContext()
        {
            var context = {
                getFields: function () {
                    var fields = []
                    getDataRecordType().then(function () {
                        for (var i = 0 ; i < dataRecordTypeEntity.Fields.length; i++) {
                            var field = dataRecordTypeEntity.Fields[i];
                            fields.push({
                                FieldName: field.Name,
                                FieldTitle: field.Title,
                                Type: field.Type
                            })
                        }
                    });

                    return fields;
                },
                getDataRecordTypeId: function () {
                    return dataRecordTypeId;
                }
            }
            return context;
        }

        function buildDataAnalysisItemDefinitionObjFromScope() {

            var dataAnalysisItemDefinitionSettings = settingsDirectiveAPI.getData();


            if (dataAnalysisItemDefinitionSettings != undefined) {
                dataAnalysisItemDefinitionSettings.Title = $scope.scopeModel.title;
                dataAnalysisItemDefinitionSettings.RecordTypeId = dataRecordTypeSelectorAPI.getSelectedIds()
            }

            console.log(dataAnalysisItemDefinitionSettings);

            return {
                DataAnalysisItemDefinitionId: dataAnalysisItemDefinitionEntity != undefined ? dataAnalysisItemDefinitionEntity.DataAnalysisItemDefinitionId : undefined,
                DataAnalysisDefinitionId: dataAnalysisItemDefinitionEntity != undefined ? dataAnalysisItemDefinitionEntity.DataAnalysisDefinitionId : dataAnalysisDefinitionId,
                Name: $scope.scopeModel.name,
                //Settings: {
                //    $type: "Vanrise.Analytic.Entities.DataAnalysis.ProfilingAndCalculation.OutputDefinitions.RecordProfilingOutputSettings, Vanrise.Analytic.Entities",
                //    Title: $scope.scopeModel.title,
                //    RecordTypeId: dataRecordTypeSelectorAPI.getSelectedIds()
                //}
                Settings: dataAnalysisItemDefinitionSettings
            };
        }
    }

    appControllers.controller('VR_Analytic_DataAnalysisItemDefinitionEditorController', DataAnalysisItemDefinitionEditorController);

})(appControllers);