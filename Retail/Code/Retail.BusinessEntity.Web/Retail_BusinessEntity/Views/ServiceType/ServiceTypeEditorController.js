(function (appControllers) {

    'use strict';

    ServiceTypeEditorController.$inject = ['$scope', 'Retail_BE_ServiceTypeAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function ServiceTypeEditorController($scope, Retail_BE_ServiceTypeAPIService, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {
        var isEditMode;

        var serviceTypeId;
        var serviceTypeEntity;
        var chargingPolicyAPI;
        var chargingPolicyReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                serviceTypeId = parameters.serviceTypeId;
            }

            isEditMode = (serviceTypeId != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.onChargingPolicyReady = function(api)
            {
                chargingPolicyAPI = api;
                chargingPolicyReadyDeferred.resolve();
            }
            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateServiceType() : insertServiceType();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                getServiceType().then(function () {
                    loadAllControls().finally(function () {
                        serviceTypeEntity = undefined;
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

        function getServiceType() {
            return Retail_BE_ServiceTypeAPIService.GetServiceType(serviceTypeId).then(function (response) {
                serviceTypeEntity = response;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadChargingPolicy]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }


        function loadChargingPolicy() {
            var chargingPolicyLoadDeferred = UtilsService.createPromiseDeferred();

            chargingPolicyReadyDeferred.promise.then(function () {
                var chargingPolicyPayload;

                if (serviceTypeEntity != undefined) {
                }

                VRUIUtilsService.callDirectiveLoad(chargingPolicyAPI, chargingPolicyPayload, chargingPolicyLoadDeferred);
            });

            return chargingPolicyLoadDeferred.promise;
        }

        function setTitle() {
            if (isEditMode) {
                var serviceTypeTitle = (serviceTypeEntity != undefined) ? serviceTypeEntity.Title : undefined;
                $scope.title = UtilsService.buildTitleForUpdateEditor(serviceTypeTitle, 'ServiceType');
            }
            else {
                $scope.title = UtilsService.buildTitleForAddEditor('ServiceType');
            }
        }

        function loadStaticData() {
            if (serviceTypeEntity == undefined)
                return;
            $scope.scopeModel.title = serviceTypeEntity.Title;
            $scope.scopeModel.description = serviceTypeEntity.Settings.Description;

        }

        function updateServiceType() {
            $scope.scopeModel.isLoading = true;

            var serviceTypeObj = buildUpdateServiceTypeObjFromScope();
            return Retail_BE_ServiceTypeAPIService.UpdateServiceType(serviceTypeObj).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('ServiceType', response, 'Name')) {
                    if ($scope.onServiceTypeUpdated != undefined) {
                        $scope.onServiceTypeUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildUpdateServiceTypeObjFromScope() {
            var obj = {
                ServiceTypeId: serviceTypeId,
                Title: $scope.scopeModel.title,
                Description: $scope.scopeModel.description,
                ChargingPolicyDefinitionSettings: chargingPolicyAPI.getData()
            };
            console.log(obj);
            return obj;
        }
    }

    appControllers.controller('Retail_BE_ServiceTypeEditorController', ServiceTypeEditorController);

})(appControllers);