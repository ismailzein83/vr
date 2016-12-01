(function (appControllers) {

    "use strict";

    ServiceEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

    function ServiceEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

        var serviceEntity;
        $scope.scopeModel = {};
        $scope.scopeModel.isEditMode = false;

        var serviceSettingsAPI;
        var serviceReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                serviceEntity = parameters.serviceEntity;
            }
            $scope.scopeModel.isEditMode = (serviceEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModel.onServiceDirectiveReady = function (api) {
                serviceSettingsAPI = api;
                serviceReadyDeferred.resolve();
            };

            $scope.scopeModel.saveService = function () {
                if ($scope.scopeModel.isEditMode) {
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

            loadAllControls();

            function loadAllControls() {

                function setTitle() {
                    if ($scope.scopeModel.isEditMode && serviceEntity != undefined)
                        $scope.title = UtilsService.buildTitleForUpdateEditor("Service");
                    else
                        $scope.title = UtilsService.buildTitleForAddEditor("Service");
                }

                function loadServiceSettingsDirective()
                {
                    var loadServiceSettingsDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                    serviceReadyDeferred.promise.then(function () {
                        var payload = {
                            serviceSettings: serviceEntity != undefined ? serviceEntity.Settings : undefined
                        };
                        VRUIUtilsService.callDirectiveLoad(serviceSettingsAPI, payload, loadServiceSettingsDirectivePromiseDeferred);
                    });
                    return loadServiceSettingsDirectivePromiseDeferred.promise;
                }

                function loadStaticData()
                {
                    if(serviceEntity == undefined)
                        return;

                    $scope.scopeModel.name = serviceEntity.Name;
                    $scope.scopeModel.description = serviceEntity.Description;
                    $scope.scopeModel.enabled = serviceEntity.Enabled;
                }

                return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadServiceSettingsDirective]).finally(function () {
                        $scope.scopeModel.isLoading = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            }
        }


        function buildServiceObjectFromScope() {
            var service = {
                Name: $scope.scopeModel.name,
                Description: $scope.scopeModel.description,
                Enabled: $scope.scopeModel.enabled,
                Settings: serviceSettingsAPI.getData()
            };
            return service;
        }

        function insert() {
            var serviceObj = buildServiceObjectFromScope();
            if ($scope.onServiceAdded != undefined)
                $scope.onServiceAdded(serviceObj);
            $scope.modalContext.closeModal();
        }

        function update() {
            var serviceObj = buildServiceObjectFromScope();
            if ($scope.onServiceUpdated != undefined)
                $scope.onServiceUpdated(serviceObj);
            $scope.modalContext.closeModal();

        }

    }

    appControllers.controller('Retail_BE_ServiceEditorController', ServiceEditorController);
})(appControllers);
