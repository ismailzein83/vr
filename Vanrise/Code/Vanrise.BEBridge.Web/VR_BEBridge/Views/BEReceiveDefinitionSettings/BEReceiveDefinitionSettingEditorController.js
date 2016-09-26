(function (appControllers) {
    "use strict";

    function beReceiveDefinitionSettingsEditorController($scope, utilsService, VRNotificationService, vrNavigationService, vruiUtilsService, beRecieveDefinitionApiService) {

        var isEditMode;
        var entitySyncDefinitions = {};
        var targetSynchronizerApi;
        var targetSynchronizerReadyDeferred = utilsService.createPromiseDeferred();

        var targetConvertorApi;
        var targetConvertorReadyDeferred = utilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = vrNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                entitySyncDefinitions = parameters.SettingsField;
            }
            isEditMode = (entitySyncDefinitions != undefined);
        }
        function setTitle() {
            if (isEditMode) {
                var receiveDefinitionName = (entitySyncDefinitions != undefined) ? '' : null;
                $scope.title = utilsService.buildTitleForUpdateEditor(receiveDefinitionName, 'Entity Definition Setting');

            }
            else {
                $scope.title = utilsService.buildTitleForAddEditor('Entity Definition Setting');
            }
        }
        function loadAllControls() {
            return utilsService.waitMultipleAsyncOperations([setTitle, loadTargetSynchronizerDefinitions, loadTargetConvertorDefinitions]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function buildBESettingsObjectObjFromScope() {

            return {
                TargetBESynchronizer: targetSynchronizerApi.getData(),
                TargetBEConvertor: targetConvertorApi.getData()
            };
        }
        function update() {
            var dataRecordFieldObject = buildBESettingsObjectObjFromScope();
            if ($scope.onReceiveDefinitionSettingsUpdated != undefined)
                $scope.onReceiveDefinitionSettingsUpdated(dataRecordFieldObject);
            $scope.modalContext.closeModal();
        }
        function insert() {

            var dataRecordFieldObject = buildBESettingsObjectObjFromScope();
            if ($scope.onReceiveDefinitionSettingsAdded != undefined) {
                $scope.onReceiveDefinitionSettingsAdded(dataRecordFieldObject);

            }
            $scope.modalContext.closeModal();
        }


        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.save = function () {
                if (isEditMode) {
                    return update();
                } else {
                    return insert();
                }
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
            $scope.scopeModel.onTargetSynchronizerDirectiveReady = function (api) {
                targetSynchronizerApi = api;
                targetSynchronizerReadyDeferred.resolve();
            };
            $scope.scopeModel.onTargetConvertorDirectiveReady = function (api) {
                targetConvertorApi = api;
                targetConvertorReadyDeferred.resolve();
            };
        }

        function loadTargetSynchronizerDefinitions() {
            var loadTargetSynchronizerPromiseDeferred = utilsService.createPromiseDeferred();
            targetSynchronizerReadyDeferred.promise.then(function () {
                var payloadDirective;
      
                if (entitySyncDefinitions != undefined) {
                    payloadDirective = {
                        targetBESynchronizer: entitySyncDefinitions.TargetBESynchronizer
                    };
                }
                vruiUtilsService.callDirectiveLoad(targetSynchronizerApi, payloadDirective, loadTargetSynchronizerPromiseDeferred);
            });
            return loadTargetSynchronizerPromiseDeferred.promise;
        }
        function loadTargetConvertorDefinitions() {
            var loadTargetConvertorPromiseDeferred = utilsService.createPromiseDeferred();
            targetConvertorReadyDeferred.promise.then(function () {
                var payloadDirective;
                if (entitySyncDefinitions != undefined) {
                    payloadDirective = {
                        targetBEConvertor: entitySyncDefinitions.TargetBEConvertor
                    };
                }
                vruiUtilsService.callDirectiveLoad(targetConvertorApi, payloadDirective, loadTargetConvertorPromiseDeferred);
            });
            return loadTargetConvertorPromiseDeferred.promise;
        }
    }

    beReceiveDefinitionSettingsEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'VR_BEBridge_BERecieveDefinitionAPIService'];
    appControllers.controller('VR_BEBridge_BEReceiveDefinitionSettingsEditorController', beReceiveDefinitionSettingsEditorController);

})(appControllers);