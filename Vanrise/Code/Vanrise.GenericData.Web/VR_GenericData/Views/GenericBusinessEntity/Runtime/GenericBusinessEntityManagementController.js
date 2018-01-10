(function (appControllers) {
    
    'use strict';

    GenericBusinessEntityManagementController.$inject = ['$scope', 'VR_GenericData_GenericBusinessEntityService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService', 'VR_GenericData_GenericBusinessEntityAPIService'];

    function GenericBusinessEntityManagementController($scope,  VR_GenericData_GenericBusinessEntityService, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, VR_GenericData_GenericBusinessEntityAPIService) {

        var definitionId;

        //var filterDirectiveAPI;
        //var filterDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        var viewId;

        var businessEntityDefinitionAPI;
        var businessEntityDefinitionReadyDeferred = UtilsService.createPromiseDeferred();
        var businessEntityDefinitionSelectedDeferred;

        var gridDirectiveAPI;
        var gridDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        var runtimeManagement;

        loadParameters();
        defineScope();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                viewId = parameters.viewId;
            }
        }

        function defineScope() {
            //$scope.onFilterDirectiveReady = function (api) {
            //    filterDirectiveAPI = api;
            //    filterDirectiveReadyDeferred.resolve();
            //};
            $scope.scopeModel = {};

            $scope.scopeModel.onGridDirectiveReady = function (api) {
                gridDirectiveAPI = api;
                gridDirectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.onBusinessEntityDefinitionSelectorReady = function (api) {
                businessEntityDefinitionAPI = api;
                businessEntityDefinitionReadyDeferred.resolve();
            };
       
            //$scope.hasAddBusinessEntityPermission = function () {
            //    return VR_GenericData_GenericBusinessEntityAPIService.DoesUserHaveAddAccess(definitionId);
            //};

            $scope.scopeModel.onBusinessEntityDefinitionSelectionChange = function () {
                if (businessEntityDefinitionAPI.getSelectedIds() != undefined) {
                    if (businessEntityDefinitionSelectedDeferred != undefined)
                        businessEntityDefinitionSelectedDeferred.resolve();
                    else {
                        $scope.scopeModel.isLoading = true;
                        loadSearchCriteria().finally(function () {
                            $scope.scopeModel.isLoading = false;
                        }).catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                    }
                }
            };


            $scope.search = function () {
                return gridDirectiveAPI.load(getGridFilter());;
            };

            $scope.addBusinessEntity = function () {
                var onGenericBusinessEntityAdded = function (addedGenericBusinessEntity) {
                    gridDirectiveAPI.onGenericBEAdded(addedGenericBusinessEntity);
                };
                VR_GenericData_GenericBusinessEntityService.addGenericBusinessEntity(onGenericBusinessEntityAdded, businessEntityDefinitionAPI.getSelectedIds());
            };

            UtilsService.waitMultiplePromises([businessEntityDefinitionReadyDeferred.promise]).then(function () {
                load();
            });

        }

        function load() {
            $scope.isLoading = true;
            loadBEDefinitionsSelectorAndSubsections().finally(function () {
                $scope.scopeModel.isLoading = false;
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
          
        }


        function loadBEDefinitionsSelectorAndSubsections() {
            var loadBEDefinitionsSelectorAndSubsectionsPromiseDeferred = UtilsService.createPromiseDeferred();

            var promises = [];
            businessEntityDefinitionSelectedDeferred = UtilsService.createPromiseDeferred();
            promises.push(businessEntityDefinitionSelectedDeferred.promise);
            promises.push(loadBusinessEntityDefinitionSelector());

            UtilsService.waitMultiplePromises(promises).then(function () {
                businessEntityDefinitionSelectedDeferred = undefined;
                $scope.scopeModel.hideBusinessEntityDefinitionSelector = businessEntityDefinitionAPI.hasSingleItem();
                loadSearchCriteria().then(function () {
                    loadBEDefinitionsSelectorAndSubsectionsPromiseDeferred.resolve();
                }).catch(function (error) {
                    loadBEDefinitionsSelectorAndSubsectionsPromiseDeferred.reject(error);
                });
            }).catch(function (error) {
                loadBEDefinitionsSelectorAndSubsectionsPromiseDeferred.reject(error);
            });

            return loadBEDefinitionsSelectorAndSubsectionsPromiseDeferred.promise;
        }

        function loadSearchCriteria() {
            return UtilsService.waitMultipleAsyncOperations([]).then(function () {
                loadGridDirective();
            });
        }

        function loadGridDirective() {
            gridDirectiveReadyDeferred.promise.then(function () {
                gridDirectiveAPI.load(getGridFilter());
            });
        }

        function getGridFilter() {
            var gridPayload = {
                query: {
                },
                businessEntityDefinitionId: businessEntityDefinitionAPI.getSelectedIds()
            };
            return gridPayload;
        }

        function loadBusinessEntityDefinitionSelector() {
            var payLoad = {
                filter: {
                    Filters: [{
                        $type: "Vanrise.GenericData.Business.GenericBEDefinitionViewFilter, Vanrise.GenericData.Business",
                        ViewId: viewId
                    }]
                },
                selectFirstItem: true
            };
            return businessEntityDefinitionAPI.load(payLoad);
        }



        //function loadAllControls() {
        //    var promises = [];

        //    var getRuntimeManagementPromise = getRuntimeManagement();
        //    promises.push(getRuntimeManagementPromise);

        //    var loadDeferred = UtilsService.createPromiseDeferred();
        //    promises.push(loadDeferred.promise);

        //    getRuntimeManagementPromise.then(function () {
        //        UtilsService.waitMultipleAsyncOperations([loadFilterDirective, loadGridDirective]).then(function () {
        //            loadDeferred.resolve();
        //        }).catch(function (error) {
        //            loadDeferred.reject(error);
        //        });
        //    });

        //    return UtilsService.waitMultiplePromises(promises);

        //    function getRuntimeManagement() {
        //        return VR_GenericData_GenericUIRuntimeAPIService.GetGenericManagementRuntime(definitionId).then(function (response) {
        //            runtimeManagement = response;
        //        });
        //    }
        //    function loadFilterDirective() {
        //        var filterDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

        //        filterDirectiveReadyDeferred.promise.then(function () {
        //            var filterDirectivePayload = {
        //                runtimeFilter: runtimeManagement.Filter
        //            };
        //            VRUIUtilsService.callDirectiveLoad(filterDirectiveAPI, filterDirectivePayload, filterDirectiveLoadDeferred);
        //        });

        //        return filterDirectiveLoadDeferred.promise;
        //    }
        //    function loadGridDirective() {
        //        var gridDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

        //        gridDirectiveReadyDeferred.promise.then(function () {
        //            VRUIUtilsService.callDirectiveLoad(gridDirectiveAPI, getGridDirectivePayload(false), gridDirectiveLoadDeferred);
        //        });

        //        return gridDirectiveLoadDeferred.promise;
        //    }
        //}

        //function getGridDirectivePayload(withFilters) {
        //    return {
        //        runtimeGrid: runtimeManagement.Grid,
        //        gridQuery: getGridQuery(withFilters)
        //    };

        //    function getGridQuery(withFilters) {
        //        var gridQuery = {
        //            BusinessEntityDefinitionId: definitionId
        //        };
        //        if (withFilters) {
        //            gridQuery.FilterValuesByFieldPath = filterDirectiveAPI.getData();
        //        }
        //        return gridQuery;
        //    }
        //}
    }

    appControllers.controller('VR_GenericData_GenericBusinessEntityManagementController', GenericBusinessEntityManagementController);

})(appControllers);