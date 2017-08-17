(function (appControllers) {

    "use strict";

    didEditorController.$inject = ['$scope', 'Retail_BE_DIDAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

    function didEditorController($scope, Retail_BE_DIDAPIService, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

        var isEditMode;

        var didId;
        var didEntity;
        var description;
        var didNumberType;

        var didNumberTypeSelectorAPI;
        var didNumberTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var directiveAPI;
        var directiveReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                didId = parameters.dIDId;
            }

            isEditMode = (didId != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onDIDNumberTypeSelectorReady = function (api) {
                didNumberTypeSelectorAPI = api;
                didNumberTypeSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onDirectiveReady = function (api) {
                directiveAPI = api;

                if (directiveReadyDeferred != undefined) {
                    directiveReadyDeferred.resolve();
                }
                else {
                    var didNumberTypeDirectivePayload = { isEditMode: isEditMode };
                    VRUIUtilsService.callDirectiveLoad(directiveAPI, didNumberTypeDirectivePayload, undefined);
                }
            };

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
                            didEntity = undefined;
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
            return Retail_BE_DIDAPIService.GetDIDRuntimeEditor(didId).then(function (response) {
                didEntity = response.DID;
                description = response.Description;
                didNumberType = response.DIDNumberType;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadDIDNumberTypeSelector, loadDIDNumberTypeDirective])
               .catch(function (error) {

                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
               .finally(function () {
                   $scope.scopeModel.isLoading = false;
               });
        }
        function setTitle() {
            $scope.title = isEditMode ? UtilsService.buildTitleForUpdateEditor(description ? description : undefined, 'DID') : UtilsService.buildTitleForAddEditor('DID');
        };
        function loadStaticData() {
            if (didEntity == undefined)
                return;

            //$scope.scopeModel.number = didEntity.Number;
            $scope.scopeModel.isInternational = didEntity.Settings.IsInternational;
            $scope.scopeModel.numberOfChannels = didEntity.Settings.NumberOfChannels;
            $scope.scopeModel.soNumber = didEntity.Settings.DIDSo;
        }
        function loadDIDNumberTypeSelector() {
            var didNumberTypeSelectorDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

            didNumberTypeSelectorReadyDeferred.promise.then(function () {
                var didNumberTypePayload = { setDefaultValue: true };
                if (didNumberType != undefined) {
                    didNumberTypePayload.selectedIds = didNumberType;
                }
                VRUIUtilsService.callDirectiveLoad(didNumberTypeSelectorAPI, didNumberTypePayload, didNumberTypeSelectorDirectiveLoadDeferred);
            });
            return didNumberTypeSelectorDirectiveLoadDeferred.promise;
        };
        function loadDIDNumberTypeDirective() {
            if (didEntity == undefined)
                return;

            var directiveLoadDeferred = UtilsService.createPromiseDeferred();

            directiveReadyDeferred.promise.then(function () {
                directiveReadyDeferred = undefined;

                var didNumberTypeDirectivePayload = {
                    didObj: didEntity,
                    isEditMode: isEditMode
                };
                VRUIUtilsService.callDirectiveLoad(directiveAPI, didNumberTypeDirectivePayload, directiveLoadDeferred);
            });

            return directiveLoadDeferred.promise;
        };

        function insertDID() {
            $scope.scopeModel.isLoading = true;
            var didObjToAdd = buildDIDObjFromScope();

            return Retail_BE_DIDAPIService.AddDID(didObjToAdd).then(function (response) {
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
            var didObjToUpdate = buildDIDObjFromScope();

            return Retail_BE_DIDAPIService.UpdateDID(didObjToUpdate).then(function (response) {
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
                DIDId: didId,
                Settings: {
                    IsInternational: $scope.scopeModel.isInternational,
                    NumberOfChannels: $scope.scopeModel.numberOfChannels,
                    DIDSo: $scope.scopeModel.soNumber
                }
            };
            directiveAPI.setData(obj);
            return obj;
        }
    }

    appControllers.controller('Retail_BE_DIDEditorController', didEditorController);

})(appControllers);
