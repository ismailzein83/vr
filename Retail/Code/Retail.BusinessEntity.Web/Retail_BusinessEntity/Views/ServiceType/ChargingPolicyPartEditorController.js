(function (appControllers) {

    'use strict';

    ChargingPolicyPartEditorController.$inject = ['$scope', 'Retail_BE_ServiceTypeAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function ChargingPolicyPartEditorController($scope, Retail_BE_ServiceTypeAPIService, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {
        var isEditMode;

        var partEntity;

        var chargingPolicyPartTypeAPI;
        var chargingPolicyPartTypeReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                partEntity = parameters.partEntity;
            }

            isEditMode = (partEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.partTypes = [];

            $scope.scopeModel.onChargingPolicyPartTypesReady = function (api) {
                chargingPolicyPartTypeAPI = api;
                chargingPolicyPartTypeReadyDeferred.resolve();
            }

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
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadChargingPolicyPartTypes]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function loadChargingPolicyPartTypes() {
            return Retail_BE_ServiceTypeAPIService.GetChargingPolicyPartTypeTemplateConfigs().then(function (response) {
                if (response != null) {
                    for (var i = 0; i < response.length; i++) {
                        $scope.scopeModel.partTypes.push(response[i]);
                    }
                    if (partEntity != undefined)
                        $scope.scopeModel.selectedPartType = UtilsService.getItemByVal($scope.scopeModel.partTypes, partEntity.ConfigId, 'ExtensionConfigurationId');
                    else
                        $scope.scopeModel.selectedPartType = $scope.scopeModel.partTypes[0];
                }
            });
        }

        function setTitle() {
            if (isEditMode) {
                var serviceTypeTitle = (serviceTypeEntity != undefined) ? serviceTypeEntity.Title : undefined;
                $scope.title = UtilsService.buildTitleForUpdateEditor(serviceTypeTitle, 'Charging Policy Part Type');
            }
            else {
                $scope.title = UtilsService.buildTitleForAddEditor('Charging Policy Part Type');
            }
        }

        function loadStaticData() {
            if (partEntity == undefined)
                return;
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
            var obj = {
                ServiceTypeId: serviceTypeId,
                Title: $scope.scopeModel.title,
                Description: $scope.scopeModel.description,
                ChargingPolicySettings: undefined
            };
            return obj;
        }
    }

    appControllers.controller('Retail_BE_ChargingPolicyPartEditorController', ChargingPolicyPartEditorController);

})(appControllers);