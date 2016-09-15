(function (appControllers) {

    "use strict";

    DataAnalysisItemDefinitionEditorController.$inject = ['$scope', 'VR_Analytic_DataAnalysisItemDefinitionAPIService', 'VR_Analytic_DataAnalysisDefinitionAPIService', 'VR_GenericData_DataRecordTypeAPIService', 'VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService'];

    function DataAnalysisItemDefinitionEditorController($scope, VR_Analytic_DataAnalysisItemDefinitionAPIService, VR_Analytic_DataAnalysisDefinitionAPIService, VR_GenericData_DataRecordTypeAPIService, VRNotificationService, UtilsService, VRUIUtilsService, VRNavigationService) {

        var isEditMode;

        var dataAnalysisItemDefinitionId;
        var dataAnalysisItemDefinitionEntity;
        var dataAnalysisDefinitionId;
        var dataAnalysisDefinitionEntity;
        var itemDefinitionTypeId;

        var dataRecordTypeId;
        var dataRecordTypeEntity;

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
                var promises = [getDataAnalysisItemDefinition(), getDataAnalysisDefinition()];

                UtilsService.waitMultiplePromises(promises).then(function () {
                    getDataRecordType().then(function () {
                        loadAllControls();
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                        $scope.scopeModel.isLoading = false;
                    });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            else {
                getDataAnalysisDefinition().then(function () {
                    getDataRecordType().then(function () {
                        loadAllControls();
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                        $scope.scopeModel.isLoading = false;
                    });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
        }

        function getDataAnalysisItemDefinition() {
            return VR_Analytic_DataAnalysisItemDefinitionAPIService.GetDataAnalysisItemDefinition(dataAnalysisItemDefinitionId).then(function (response) {
                dataAnalysisItemDefinitionEntity = response;
            });
        }
        function getDataAnalysisDefinition() {
            return VR_Analytic_DataAnalysisDefinitionAPIService.GetDataAnalysisDefinition(dataAnalysisDefinitionId).then(function (response) {
                dataAnalysisDefinitionEntity = response;
                dataRecordTypeId = dataAnalysisDefinitionEntity.Settings.DataRecordTypeId;
            });
        }
        function getDataRecordType() {
            return VR_GenericData_DataRecordTypeAPIService.GetDataRecordType(dataRecordTypeId).then(function (response) {
                dataRecordTypeEntity = response;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadSettingsDirective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.scopeModel.isLoading = false;
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            })

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
            function loadSettingsDirective() {
                var settingsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                settingsDirectiveReadyDeferred.promise.then(function () {

                        var settingsDirectivePayload = {};
                        settingsDirectivePayload.dataAnalysisItemDefinition = dataAnalysisItemDefinitionEntity;
                        settingsDirectivePayload.dataAnalysisDefinitionId = dataAnalysisDefinitionId;
                        settingsDirectivePayload.itemDefinitionTypeId = itemDefinitionTypeId;
                        settingsDirectivePayload.context = buildContext();

                        VRUIUtilsService.callDirectiveLoad(settingsDirectiveAPI, settingsDirectivePayload, settingsDirectiveLoadDeferred);
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

                    for (var i = 0 ; i < dataRecordTypeEntity.Fields.length; i++) {
                        var field = dataRecordTypeEntity.Fields[i];
                        fields.push({
                            FieldName: field.Name,
                            FieldTitle: field.Title,
                            Type: field.Type
                        })
                    }
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
            }

            return {
                DataAnalysisItemDefinitionId: dataAnalysisItemDefinitionEntity != undefined ? dataAnalysisItemDefinitionEntity.DataAnalysisItemDefinitionId : undefined,
                DataAnalysisDefinitionId: dataAnalysisItemDefinitionEntity != undefined ? dataAnalysisItemDefinitionEntity.DataAnalysisDefinitionId : dataAnalysisDefinitionId,
                Name: $scope.scopeModel.name,
                Settings: dataAnalysisItemDefinitionSettings
            };
        }
    }

    appControllers.controller('VR_Analytic_DataAnalysisItemDefinitionEditorController', DataAnalysisItemDefinitionEditorController);

})(appControllers);