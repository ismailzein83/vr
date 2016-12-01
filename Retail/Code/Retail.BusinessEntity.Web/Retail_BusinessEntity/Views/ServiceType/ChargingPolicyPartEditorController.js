(function (appControllers) {

    'use strict';

    ChargingPolicyPartEditorController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function ChargingPolicyPartEditorController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {
        var isEditMode;

        var partEntity;
        var context;
        var partTypeAPI;
        var partTypeReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                partEntity = parameters.partEntity;
                context = parameters.context;
            }

            isEditMode = (partEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};
 
            $scope.scopeModel.partTypeDirectiveReady = function (api) {
                partTypeAPI = api;
                partTypeReadyDeferred.resolve();
            };

            $scope.scopeModel.save = function () {
                return (isEditMode) ? updatePartType() : insertPartType();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };

        }

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadChargingPolicyPartTypes]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function loadChargingPolicyPartTypes() {
            var loadPartTypePromiseDeferred = UtilsService.createPromiseDeferred();
            partTypeReadyDeferred.promise.then(function () {
                var payloadDirective = {context:context, partType : partEntity };
                VRUIUtilsService.callDirectiveLoad(partTypeAPI, payloadDirective, loadPartTypePromiseDeferred);
            });
            return loadPartTypePromiseDeferred.promise;
        }

        function setTitle() {
            if (isEditMode) {
                var serviceTypeTitle = (partEntity != undefined) ? partEntity.PartTypeTitle : undefined;
                $scope.title = UtilsService.buildTitleForUpdateEditor(serviceTypeTitle, 'Charging Policy Part Type');
            }
            else {
                $scope.title = UtilsService.buildTitleForAddEditor('Charging Policy Part Type');
            }
        }

        function updatePartType() {
            var partTypeObj = buildPartTypeObjFromScope();
            
            if ($scope.onPartTypeUpdated != undefined) {
                $scope.onPartTypeUpdated(partTypeObj);
            }
            $scope.modalContext.closeModal();
        }

        function insertPartType() {
            var partTypeObj = buildPartTypeObjFromScope();
            if ($scope.onPartTypeAdded != undefined) {
                $scope.onPartTypeAdded(partTypeObj);
            }
            $scope.modalContext.closeModal();
        }

        function buildPartTypeObjFromScope()
        {
            var part = partTypeAPI.getData();
            var partTypeObj = {
                Part: part
            };
            return partTypeObj;
        }
    }

    appControllers.controller('Retail_BE_ChargingPolicyPartEditorController', ChargingPolicyPartEditorController);

})(appControllers);