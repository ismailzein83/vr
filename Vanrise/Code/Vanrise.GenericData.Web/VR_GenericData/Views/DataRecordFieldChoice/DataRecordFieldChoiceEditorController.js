//(function (appControllers) {

//    "use strict";
//    DataRecordFieldChoiceEditorController.$inject = ["$scope", "VR_GenericData_DataRecordFieldChoiceAPIService", "VRNavigationService", "VRNotificationService", "UtilsService", "VRUIUtilsService"];

//    function DataRecordFieldChoiceEditorController($scope, VR_GenericData_DataRecordFieldChoiceAPIService, VRNavigationService, VRNotificationService, UtilsService, VRUIUtilsService) {

//        var dataRecordFieldChoiceId;
//        var dataRecordFieldChoiceEntity;
//        var isEditMode;
//        var dataRecordFieldChoiceSelectorAPI;
//        var dataRecordFieldChoiceReadyPromiseDeferred = UtilsService.createPromiseDeferred();
//        var dataRecordFieldChoiceDirectiveAPI;
//        var dataRecordFieldChoiceConfigReadyPromiseDeferred; // = UtilsService.createPromiseDeferred();
//        loadParameters();
//        defineScope();
//        load();

//        function loadParameters() {
//            var parameters = VRNavigationService.getParameters($scope);

//            if (parameters != undefined && parameters != null)
//                dataRecordFieldChoiceId = parameters.DataRecordFieldChoiceId;

//            isEditMode = (dataRecordFieldChoiceId != undefined);
//        }

//        function defineScope() {

//            $scope.onReadyDataRecordFieldChoiceConfigSelector = function (api) {
//                dataRecordFieldChoiceSelectorAPI = api;
//                dataRecordFieldChoiceReadyPromiseDeferred.resolve();
//            }
//            $scope.onReadyDataRecordFieldChoiceConfigDirective = function (api) {
//                dataRecordFieldChoiceDirectiveAPI = api;

//                var setLoader = function (value) { $scope.isLoadingConfigDirective = value };
//                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataRecordFieldChoiceDirectiveAPI, undefined, setLoader, dataRecordFieldChoiceConfigReadyPromiseDeferred);
//            }
//            $scope.saveDataRecordFieldChoice = function () {
//                $scope.isLoading = true;
//                if (isEditMode)
//                    return updateDataRecordFieldChoice();
//                else
//                    return insertDataRecordFieldChoice();
//            };
//            $scope.hasSaveDataRecordFieldChoice = function () {
//                if (isEditMode) {
//                    return VR_GenericData_DataRecordFieldChoiceAPIService.HasUpdateDataRecordFieldChoice();
//                }
//                else {
//                    return VR_GenericData_DataRecordFieldChoiceAPIService.HasUpdateDataRecordFieldChoice();
//                }
//            };
//            $scope.close = function () {
//                $scope.modalContext.closeModal()
//            }
//        }

//        function load() {
//            $scope.isLoading = true;
//            if (isEditMode) {
//                GetDataRecordFieldChoice().then(function () {
//                    loadAllControls().finally(function () {
//                        dataRecordFieldChoiceEntity = undefined;
//                    }).catch(function (error) {
//                        VRNotificationService.notifyExceptionWithClose(error, $scope);
//                        $scope.isLoading = false;
//                    });
//                });
//            }
//            else {
//                loadAllControls();
//            }

//        }

//        function GetDataRecordFieldChoice() {
//            return VR_GenericData_DataRecordFieldChoiceAPIService.GetDataRecordFieldChoice(dataRecordFieldChoiceId).then(function (response) {
//                dataRecordFieldChoiceEntity = response;
//            });
//        }

//        function loadAllControls() {
//            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadDataRecordFieldChoiceConfigSelector, loadDataRecordFieldChoiceConfigDirective])
//               .catch(function (error) {
//                   VRNotificationService.notifyExceptionWithClose(error, $scope);
//               })
//              .finally(function () {
//                  $scope.isLoading = false;
//              });
//        }

//        function setTitle() {
//            if (isEditMode && dataRecordFieldChoiceEntity != undefined)
//                $scope.title = UtilsService.buildTitleForUpdateEditor(dataRecordFieldChoiceEntity.Name, "Data Store");
//            else
//                $scope.title = UtilsService.buildTitleForAddEditor("Data Store");
//        }

//        function loadStaticData() {
//            if (dataRecordFieldChoiceEntity == undefined)
//                return;
//            $scope.name = dataRecordFieldChoiceEntity.Name;
//        }

//        function loadDataRecordFieldChoiceConfigSelector() {
//            var loadDataRecordFieldChoicePromiseDeferred = UtilsService.createPromiseDeferred();
//            dataRecordFieldChoiceReadyPromiseDeferred.promise.then(function () {
//                if (dataRecordFieldChoiceEntity != undefined && dataRecordFieldChoiceEntity.Settings != undefined)
//                    var payLoad = {
//                        selectedIds: (dataRecordFieldChoiceEntity != undefined && dataRecordFieldChoiceEntity.Settings != undefined && dataRecordFieldChoiceEntity.Settings.ConfigId != undefined) ? dataRecordFieldChoiceEntity.Settings.ConfigId : undefined
//                    }
//                VRUIUtilsService.callDirectiveLoad(dataRecordFieldChoiceSelectorAPI, payLoad, loadDataRecordFieldChoicePromiseDeferred);
//            });
//            return loadDataRecordFieldChoicePromiseDeferred.promise;
//        }
//        function loadDataRecordFieldChoiceConfigDirective() {
//            if (dataRecordFieldChoiceEntity != undefined && dataRecordFieldChoiceEntity.Settings != undefined) {
//                dataRecordFieldChoiceConfigReadyPromiseDeferred = UtilsService.createPromiseDeferred();
//                var loadDataRecordFieldChoiceConfigPromiseDeferred = UtilsService.createPromiseDeferred();
//                dataRecordFieldChoiceConfigReadyPromiseDeferred.promise.then(function () {
//                    dataRecordFieldChoiceConfigReadyPromiseDeferred = undefined;
//                    var directivePayLoad = {
//                        data: dataRecordFieldChoiceEntity.Settings
//                    }
//                    VRUIUtilsService.callDirectiveLoad(dataRecordFieldChoiceDirectiveAPI, directivePayLoad, loadDataRecordFieldChoiceConfigPromiseDeferred);
//                });
//                return loadDataRecordFieldChoiceConfigPromiseDeferred.promise;
//            }


//        }


//        function updateDataRecordFieldChoice() {
//            var dataRecordFieldChoiceObj = buildDataSToreObjFromScope();
//            return VR_GenericData_DataRecordFieldChoiceAPIService.UpdateDataRecordFieldChoice(dataRecordFieldChoiceObj)
//                .then(function (response) {
//                    $scope.isLoading = false;
//                    if (VRNotificationService.notifyOnItemUpdated("Data Store", response, "Name")) {

//                        if ($scope.onDataRecordFieldChoiceUpdated != undefined)
//                            $scope.onDataRecordFieldChoiceUpdated(response.UpdatedObject);

//                        $scope.modalContext.closeModal();
//                    }
//                })
//                .catch(function (error) {
//                    VRNotificationService.notifyException(error, $scope);
//                }).finally(function () {
//                    $scope.isLoading = false;
//                });
//        }

//        function insertDataRecordFieldChoice() {
//            var dataRecordFieldChoiceObj = buildDataSToreObjFromScope();
//            return VR_GenericData_DataRecordFieldChoiceAPIService.AddDataRecordFieldChoice(dataRecordFieldChoiceObj)
//                .then(function (response) {
//                    $scope.isLoading = false;
//                    if (VRNotificationService.notifyOnItemAdded("Data Store", response, "Name")) {
//                        if ($scope.onDataRecordFieldChoiceAdded != undefined)
//                            $scope.onDataRecordFieldChoiceAdded(response.InsertedObject);

//                        $scope.modalContext.closeModal();
//                    }
//                })
//                .catch(function (error) {
//                    VRNotificationService.notifyException(error, $scope);
//                }).finally(function () {
//                    $scope.isLoading = false;
//                });
//        }

//        function buildDataSToreObjFromScope() {
//            var settings = dataRecordFieldChoiceDirectiveAPI.getData();
//            settings.ConfigId = dataRecordFieldChoiceSelectorAPI.getSelectedIds();
//            return {
//                DataRecordFieldChoiceId: dataRecordFieldChoiceId,
//                Name: $scope.name,
//                Settings: settings
//            };
//        }
//    }

//    appControllers.controller("VR_GenericData_DataRecordFieldChoiceEditorController", DataRecordFieldChoiceEditorController);

//})(appControllers);