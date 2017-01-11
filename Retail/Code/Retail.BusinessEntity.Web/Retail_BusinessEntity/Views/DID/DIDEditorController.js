(function (appControllers) {

    "use strict";

    dIDEditorController.$inject = ['$scope', 'Retail_BE_DIDAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

    function dIDEditorController($scope, Retail_BE_DIDAPIService, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

        var isEditMode;

        var dIDId;
        var dIDEntity;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                dIDId = parameters.dIDId;
            }

            isEditMode = (dIDId != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};
          

            $scope.scopeModel.save = function () {
                if (isEditMode) {
                    return updateDID();
                }
                else {
                    return insertDID();
                }
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };

            $scope.scopeModel.hasSaveDIDPermission = function () {
                if ($scope.scopeModel.isEditMode)
                    return Retail_BE_DIDAPIService.HasUpdateDIDPermission();
                else
                    return Retail_BE_DIDAPIService.HasAddDIDPermission();
            };
        }
        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                getDID().then(function () {
                    loadAllControls()
                        .finally(function () {
                            dIDEntity = undefined;
                        });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }
        }

        function getDID() {
            return Retail_BE_DIDAPIService.GetDID(dIDId).then(function (response) {
                dIDEntity = response;
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
            $scope.title = isEditMode ? UtilsService.buildTitleForUpdateEditor(dIDEntity ? dIDEntity.Number : undefined, 'DID') : UtilsService.buildTitleForAddEditor('DID');
        }
        function loadStaticData() {
            if (dIDEntity == undefined)
                return;

            $scope.scopeModel.number = dIDEntity.Number;
            $scope.scopeModel.isInternational = dIDEntity.Settings.IsInternational;
            $scope.scopeModel.numberOfChannels = dIDEntity.Settings.NumberOfChannels;
        }
      
        function insertDID() {
            $scope.scopeModel.isLoading = true;

            return Retail_BE_DIDAPIService.AddDID(buildDIDObjFromScope())
                .then(function (response) {
                    if (VRNotificationService.notifyOnItemAdded("DID", response, "Number")) {
                        if ($scope.onDIDAdded != undefined)
                            $scope.onDIDAdded(response.InsertedObject);
                        $scope.modalContext.closeModal();
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
        }
        function updateDID() {
            $scope.scopeModel.isLoading = true;

            return Retail_BE_DIDAPIService.UpdateDID(buildDIDObjFromScope())
                .then(function (response) {
                    if (VRNotificationService.notifyOnItemUpdated("DID", response, "Number")) {
                        if ($scope.onDIDUpdated != undefined)
                            $scope.onDIDUpdated(response.UpdatedObject);

                        $scope.modalContext.closeModal();
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
        }

        function buildDIDObjFromScope() {
            var obj = {
                DIDId: dIDId,
                Number: $scope.scopeModel.number,
                Settings: {
                    IsInternational: $scope.scopeModel.isInternational,
                    NumberOfChannels: $scope.scopeModel.numberOfChannels
                }
            };
            return obj;
        }
    }

    appControllers.controller('Retail_BE_DIDEditorController', dIDEditorController);

})(appControllers);
