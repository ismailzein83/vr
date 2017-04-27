(function (appControllers) {

    "use strict";
    DataRecordFieldChoiceEditorController.$inject = ["$scope", "VR_GenericData_DataRecordFieldChoiceAPIService", "VRNavigationService", "VRNotificationService", "UtilsService", "VRUIUtilsService"];

    function DataRecordFieldChoiceEditorController($scope, VR_GenericData_DataRecordFieldChoiceAPIService, VRNavigationService, VRNotificationService, UtilsService, VRUIUtilsService) {

        var dataRecordFieldChoiceId;
        var dataRecordFieldChoiceEntity;
        var isEditMode;

        var choicesGridAPI;
        var choicesGridReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null)
                dataRecordFieldChoiceId = parameters.DataRecordFieldChoiceId;

            isEditMode = (dataRecordFieldChoiceId != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.values = [];
            $scope.scopeModel.onChoicesGridReady = function (api) {
                choicesGridAPI = api;
                choicesGridReadyDeferred.resolve();
            };
            $scope.scopeModel.saveDataRecordFieldChoice = function () {
                $scope.isLoading = true;
                if (isEditMode)
                    return updateDataRecordFieldChoice();
                else
                    return insertDataRecordFieldChoice();
            };
            $scope.scopeModel.hasSaveDataRecordFieldChoice = function () {
                if (isEditMode) {
                    return VR_GenericData_DataRecordFieldChoiceAPIService.HasUpdateDataRecordFieldChoice();
                }
                else {
                    return VR_GenericData_DataRecordFieldChoiceAPIService.HasUpdateDataRecordFieldChoice();
                }
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };

            function updateDataRecordFieldChoice() {
                var dataRecordFieldChoiceObj = buildDataSToreObjFromScope();
                return VR_GenericData_DataRecordFieldChoiceAPIService.UpdateDataRecordFieldChoice(dataRecordFieldChoiceObj)
                    .then(function (response) {
                        $scope.scopeModel.isLoading = false;
                        if (VRNotificationService.notifyOnItemUpdated("Data Record Field Choice", response, "Name")) {

                            if ($scope.onDataRecordFieldChoiceUpdated != undefined)
                                $scope.onDataRecordFieldChoiceUpdated(response.UpdatedObject);
                            $scope.modalContext.closeModal();
                        }
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    }).finally(function () {
                        $scope.scopeModel.isLoading = false;
                    });
            }

            function insertDataRecordFieldChoice() {
                var dataRecordFieldChoiceObj = buildDataSToreObjFromScope();
                return VR_GenericData_DataRecordFieldChoiceAPIService.AddDataRecordFieldChoice(dataRecordFieldChoiceObj)
                    .then(function (response) {
                        $scope.scopeModel.isLoading = false;
                        if (VRNotificationService.notifyOnItemAdded("Data Record Field Choice", response, "Name")) {
                            if ($scope.onDataRecordFieldChoiceAdded != undefined)
                                $scope.onDataRecordFieldChoiceAdded(response.InsertedObject);
                            $scope.modalContext.closeModal();
                        }
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    }).finally(function () {
                        $scope.scopeModel.isLoading = false;
                    });
            }

            function buildDataSToreObjFromScope() {
                return {
                    DataRecordFieldChoiceId: dataRecordFieldChoiceId,
                    Name: $scope.scopeModel.choiceName,
                    Settings: {
                        Choices: choicesGridAPI.getData()
                    }
                };
            }

        }

        function load() {
            $scope.scopeModel.isLoading = true;
            if (isEditMode) {
                getDataRecordFieldChoice().then(function () {
                    loadAllControls().finally(function () {
                        dataRecordFieldChoiceEntity = undefined;
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                        $scope.scopeModel.isLoading = false;
                    });
                });
            }
            else {
                loadAllControls();
            }

        }

        function getDataRecordFieldChoice() {
            return VR_GenericData_DataRecordFieldChoiceAPIService.GetDataRecordFieldChoice(dataRecordFieldChoiceId).then(function (response) {
                dataRecordFieldChoiceEntity = response;
            });
        }

        function loadAllControls() {

            function setTitle() {
                if (isEditMode && dataRecordFieldChoiceEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(dataRecordFieldChoiceEntity.Name, "Data Record Field Choice");
                else
                    $scope.title = UtilsService.buildTitleForAddEditor("Data Record Field Choice");
            }

            function loadStaticData() {
                if (dataRecordFieldChoiceEntity == undefined)
                    return;
                $scope.scopeModel.choiceName = dataRecordFieldChoiceEntity.Name;
            }

            function loadChoicesGrid() {
                var loadChoicesGridPromiseDeferred = UtilsService.createPromiseDeferred();
                choicesGridReadyDeferred.promise.then(function () {
                    var choicesGridPayLoad;
                    if (dataRecordFieldChoiceEntity != undefined && dataRecordFieldChoiceEntity.Settings != undefined) {
                        choicesGridPayLoad = {
                            choices: dataRecordFieldChoiceEntity.Settings.Choices
                        }
                    }
                    VRUIUtilsService.callDirectiveLoad(choicesGridAPI, choicesGridPayLoad, loadChoicesGridPromiseDeferred);
                });
                return loadChoicesGridPromiseDeferred.promise;
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadChoicesGrid])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.scopeModel.isLoading = false;
              });
        }

    }

    appControllers.controller("VR_GenericData_DataRecordFieldChoiceEditorController", DataRecordFieldChoiceEditorController);

})(appControllers);