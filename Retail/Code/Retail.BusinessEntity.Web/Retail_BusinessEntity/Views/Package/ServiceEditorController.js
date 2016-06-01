(function (appControllers) {

    "use strict";

    ServiceEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

    function ServiceEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

        var serviceEntity;
        $scope.scopeModel = {};
        $scope.scopeModel.isEditMode = false;

        var serviceAPI;
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
                serviceAPI = api;
                serviceReadyDeferred.resolve();
            }

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

                function loadServiceDirective()
                {
                    var loadServiceDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                    serviceReadyDeferred.promise.then(function () {
                        var payloadServiceDirective = {
                            serviceEntity: serviceEntity != undefined ? serviceEntity : undefined
                        };
                        VRUIUtilsService.callDirectiveLoad(serviceAPI, payloadServiceDirective, loadServiceDirectivePromiseDeferred);
                    });
                    return loadServiceDirectivePromiseDeferred.promise;
                }

                return UtilsService.waitMultipleAsyncOperations([setTitle, loadServiceDirective]).finally(function () {
                        $scope.scopeModel.isLoading = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            }
        }


        function buildServiceObjectFromScope() {
            var service = serviceAPI !=undefined?serviceAPI.getData():undefined
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
