(function (appControllers) {
    
    'use strict';

    GenericBusinessEntityManagementController.$inject = ['$scope', 'VR_GenericData_GenericUIRuntimeAPIService', 'VR_GenericData_GenericBusinessEntityService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService', 'VR_GenericData_GenericBusinessEntityAPIService'];

    function GenericBusinessEntityManagementController($scope, VR_GenericData_GenericUIRuntimeAPIService, VR_GenericData_GenericBusinessEntityService, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, VR_GenericData_GenericBusinessEntityAPIService) {

        var definitionId;

        var filterDirectiveAPI;
        var filterDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        var gridDirectiveAPI;
        var gridDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        var runtimeManagement;

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
            $scope.hasAddBusinessEntityPermission = function () {
                return VR_GenericData_GenericBusinessEntityAPIService.DoesUserHaveAddAccess(definitionId);
            }
            $scope.search = function () {
                return gridDirectiveAPI.load(getGridDirectivePayload(true));
            };

            $scope.add = function () {
                var onGenericBusinessEntityAdded = function (addedGenericBusinessEntity) {
                    gridDirectiveAPI.onGenericBusinessEntityAdded(addedGenericBusinessEntity);
                };
                VR_GenericData_GenericBusinessEntityService.addGenericBusinessEntity(definitionId, onGenericBusinessEntityAdded);
            };
        }

        function load() {
            $scope.isLoading = true;
            loadAllControls().catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }

        function loadAllControls() {
            var promises = [];

            var getRuntimeManagementPromise = getRuntimeManagement();
            promises.push(getRuntimeManagementPromise);

            var loadDeferred = UtilsService.createPromiseDeferred();
            promises.push(loadDeferred.promise);

            getRuntimeManagementPromise.then(function () {
                UtilsService.waitMultipleAsyncOperations([loadFilterDirective, loadGridDirective]).then(function () {
                    loadDeferred.resolve();
                }).catch(function (error) {
                    loadDeferred.reject(error);
                });
            });

            return UtilsService.waitMultiplePromises(promises);

            function getRuntimeManagement() {
                return VR_GenericData_GenericUIRuntimeAPIService.GetGenericManagementRuntime(definitionId).then(function (response) {
                    runtimeManagement = response;
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
                    VRUIUtilsService.callDirectiveLoad(gridDirectiveAPI, getGridDirectivePayload(false), gridDirectiveLoadDeferred);
                });

                return gridDirectiveLoadDeferred.promise;
            }
        }

        function getGridDirectivePayload(withFilters) {
            return {
                runtimeGrid: runtimeManagement.Grid,
                gridQuery: getGridQuery(withFilters)
            };

            function getGridQuery(withFilters) {
                var gridQuery = {
                    BusinessEntityDefinitionId: definitionId
                };
                if (withFilters) {
                    gridQuery.FilterValuesByFieldPath = filterDirectiveAPI.getData();
                }
                return gridQuery;
            }
        }
    }

    appControllers.controller('VR_GenericData_GenericBusinessEntityManagementController', GenericBusinessEntityManagementController);

})(appControllers);