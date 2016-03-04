(function (appControllers) {
    
    'use strict';

    GenericBusinessEntityManagementController.$inject = ['$scope', 'VR_GenericData_GenericUIRuntimeAPIService', 'VR_GenericData_GenericBusinessEntityService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function GenericBusinessEntityManagementController($scope, VR_GenericData_GenericUIRuntimeAPIService, VR_GenericData_GenericBusinessEntityService, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var definitionId;

        var filterDirectiveAPI;
        var filterDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        var gridDirectiveAPI;
        var gridDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                definitionId = parameters.businessEntityDefinitionId;
            }
        }

        function defineScope() {
            $scope.onFilterDirectiveReady = function (api) {
                filterDirectiveAPI = api;
                filterDirectiveReadyDeferred.resolve();
            };

            $scope.onGridDirectiveReady = function (api) {
                gridDirectiveAPI = api;
                gridDirectiveReadyDeferred.resolve();
            };

            $scope.add = function () {
                var onGenericBusinessEntityAdded = function (addedGenericBusinessEntity) {
                    gridDirectiveAPI.onGenericBusinessEntityAdded(addedGenericBusinessEntity);
                };
                VR_GenericData_GenericBusinessEntityService.addGenericBusinessEntity(definitionId, onGenericBusinessEntityAdded);
            };
        }

        function load() {
            loadAllControls();

            function loadAllControls() {
                $scope.isLoading = true;
                var promises = [];
                var runtimeManagement;

                var getRuntimeManagementPromise = getRuntimeManagement();
                promises.push(getRuntimeManagementPromise);

                var loadDeferred = UtilsService.createPromiseDeferred();
                promises.push(loadDeferred.promise);

                getRuntimeManagementPromise.then(function () {
                    UtilsService.waitMultipleAsyncOperations([loadFilterDirective]).then(function () {
                        loadDeferred.resolve();
                    }).catch(function (error) {
                        loadDeferred.reject(error);
                    });
                });

                return UtilsService.waitMultiplePromises(promises).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                }).finally(function () {
                    $scope.isLoading = false;
                });

                function getRuntimeManagement() {
                    return VR_GenericData_GenericUIRuntimeAPIService.GetGenericManagementRuntime(definitionId).then(function (response) {
                        runtimeManagement = response;
                        console.log(runtimeManagement);
                    });
                }
                function loadFilterDirective() {
                    var filterDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                    filterDirectiveReadyDeferred.promise.then(function () {
                        var filterDirectivePayload = {
                            runtimeFilter: runtimeManagement.Filter
                        };
                        VRUIUtilsService.callDirectiveLoad(filterDirectiveAPI, filterDirectivePayload, filterDirectiveLoadDeferred);
                    });

                    return filterDirectiveLoadDeferred.promise;
                }
                function loadGridDirective() {
                    var gridDirectiveLoadDeferred = UtilsService.createPromiseDeferred();
                    
                    gridDirectiveReadyDeferred.promise.then(function () {
                        var gridDirectivePayload = {
                            runtimeGrid: runtimeManagement.Grid
                        };
                        VRUIUtilsService.callDirectiveLoad(gridDirectiveAPI, gridDirectivePayload, gridDirectiveLoadDeferred);
                    });

                    return gridDirectiveLoadDeferred.promise;
                }
            }
        }
    }

    appControllers.controller('VR_GenericData_GenericBusinessEntityManagementController', GenericBusinessEntityManagementController);

})(appControllers);