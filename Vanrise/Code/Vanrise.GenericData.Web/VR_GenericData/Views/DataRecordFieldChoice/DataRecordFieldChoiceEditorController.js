(function (appControllers) {

    "use strict";
    DataRecordFieldChoiceEditorController.$inject = ["$scope", "VR_GenericData_DataRecordFieldChoiceAPIService", "VRNavigationService", "VRNotificationService", "UtilsService", "VRUIUtilsService"];

    function DataRecordFieldChoiceEditorController($scope, VR_GenericData_DataRecordFieldChoiceAPIService, VRNavigationService, VRNotificationService, UtilsService, VRUIUtilsService) {

        var dataRecordFieldChoiceId;
        var dataRecordFieldChoiceEntity;
        var isEditMode;

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

            $scope.scopeModel.Id = $scope.scopeModel.values.length + 1;
            $scope.scopeModel.isValid = function () {
                if ($scope.scopeModel.values != undefined && $scope.scopeModel.values.length > 0)
                    return null;
                return "You Should Add At Least One Choice."
            };
            $scope.scopeModel.disableAddButton = true;
            $scope.scopeModel.addValue = function () {
                $scope.scopeModel.values.push(AddChoice($scope.scopeModel.value));
                $scope.scopeModel.Id = $scope.scopeModel.values.length + 1;
                $scope.scopeModel.value = undefined;
                $scope.scopeModel.disableAddButton = true;
            };
            $scope.scopeModel.onValueChange = function (value) {
                $scope.scopeModel.disableAddButton = value == undefined || (UtilsService.getItemIndexByVal($scope.scopeModel.values, $scope.scopeModel.value, "Text") != -1 || UtilsService.getItemIndexByVal($scope.scopeModel.values, $scope.scopeModel.Id, "Value") != -1);
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
        }
        function AddChoice(choice) {
            var obj = {
                Value: $scope.scopeModel.Id,
                Text: choice
            };
            return obj;
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
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.scopeModel.isLoading = false;
              });
        }

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
            if(dataRecordFieldChoiceEntity.Settings != undefined && dataRecordFieldChoiceEntity.Settings.Choices !=undefined)
            {
                for(var i=0; i<dataRecordFieldChoiceEntity.Settings.Choices.length;i++)
                {
                   
                    var choice = dataRecordFieldChoiceEntity.Settings.Choices[i];
                    $scope.scopeModel.Id = choice.Value + 1;
                    $scope.scopeModel.values.push({
                        Value: choice.Value,
                        Text: choice.Text
                    });
                }
            }


        }

       
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
            var settings = {
                Choices: $scope.scopeModel.values
            };
            return {
                DataRecordFieldChoiceId: dataRecordFieldChoiceId,
                Name: $scope.scopeModel.choiceName,
                Settings: settings
            };
        }
    }

    appControllers.controller("VR_GenericData_DataRecordFieldChoiceEditorController", DataRecordFieldChoiceEditorController);

})(appControllers);