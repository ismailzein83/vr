(function (appControllers) {
    
    'use strict';

    GenericBusinessEntityManagementController.$inject = ['$scope', 'VR_GenericData_GenericBusinessEntityService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService', 'VR_GenericData_GenericBEDefinitionAPIService','VR_GenericData_RecordQueryLogicalOperatorEnum'];

    function GenericBusinessEntityManagementController($scope, VR_GenericData_GenericBusinessEntityService, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, VR_GenericData_GenericBEDefinitionAPIService,VR_GenericData_RecordQueryLogicalOperatorEnum) {

        var definitionId;

        var filterDirectiveAPI;
        var filterDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        var viewId;

        var genericBEDefinitionSettings;

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
            $scope.scopeModel = {};

            $scope.scopeModel.onFilterDirectiveReady = function (api) {
                filterDirectiveAPI = api;
                filterDirectiveReadyDeferred.resolve();
            };

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

                        loadBusinessEntityDefinitionSettings().then(function () {
                            loadSearchCriteria().finally(function () {
                                $scope.scopeModel.isLoading = false;
                            }).catch(function (error) {
                                VRNotificationService.notifyException(error, $scope);
                            });
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
                var editorSize = undefined;//genericBEDefinitionSettings != undefined ? genericBEDefinitionSettings.editorSize : undefined;
                VR_GenericData_GenericBusinessEntityService.addGenericBusinessEntity(onGenericBusinessEntityAdded, businessEntityDefinitionAPI.getSelectedIds(), editorSize);
            };

            UtilsService.waitMultiplePromises([businessEntityDefinitionReadyDeferred.promise]).then(function () {
                load();
            });

        }

        function load() {
            $scope.isLoading = true;
            loadBEDefinitionsSelectorAndSubsections().catch(function (error) {
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

                loadBusinessEntityDefinitionSettings().then(function () {
                    loadSearchCriteria().then(function () {
                        loadBEDefinitionsSelectorAndSubsectionsPromiseDeferred.resolve();
                    }).catch(function (error) {
                        loadBEDefinitionsSelectorAndSubsectionsPromiseDeferred.reject(error);
                    });
                }).catch(function (error) {
                    loadBEDefinitionsSelectorAndSubsectionsPromiseDeferred.reject(error);
                });

            }).catch(function (error) {
                loadBEDefinitionsSelectorAndSubsectionsPromiseDeferred.reject(error);
            });

            return loadBEDefinitionsSelectorAndSubsectionsPromiseDeferred.promise;
        }

        function loadBusinessEntityDefinitionSettings() {
            var businessEntityDefinitionId = businessEntityDefinitionAPI.getSelectedIds();
            return VR_GenericData_GenericBEDefinitionAPIService.GetGenericBEDefinitionSettings(businessEntityDefinitionId).then(function (response) {
                genericBEDefinitionSettings = response;
                if(genericBEDefinitionSettings != undefined)
                {
                    if(genericBEDefinitionSettings.FilterDefinition != undefined && genericBEDefinitionSettings.FilterDefinition.Settings != undefined)
                    {
                        $scope.scopeModel.filterDirective = genericBEDefinitionSettings.FilterDefinition.Settings.RuntimeEditor;
                    }
                }
            });
        }

        function loadSearchCriteria() {
            return UtilsService.waitMultipleAsyncOperations([loadFilterDirective]).then(function () {
                loadGridDirective();
            });
        }

        function loadFilterDirective() {
            if ($scope.scopeModel.filterDirective != undefined) {
                var filterDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                filterDirectiveReadyDeferred.promise.then(function () {
                    var filterDirectivePayload = {
                        settings: genericBEDefinitionSettings.FilterDefinition.Settings,
                        dataRecordTypeId: genericBEDefinitionSettings.DataRecordTypeId
                    };
                    VRUIUtilsService.callDirectiveLoad(filterDirectiveAPI, filterDirectivePayload, filterDirectiveLoadDeferred);
                });
                return filterDirectiveLoadDeferred.promise;
            }
        }

        function loadGridDirective() {
            gridDirectiveReadyDeferred.promise.then(function () {
                gridDirectiveAPI.load(getGridFilter());
            });
        }

        function getGridFilter() {
            var filterData = filterDirectiveAPI != undefined ? filterDirectiveAPI.getData() : undefined;
            var filterGroup;
            var filters;

            if (filterData != undefined) {
                if (filterData.RecordFilter != undefined) {
                    if (filterData.RecordFilter.$type.indexOf("RecordFilterGroup") < 0) {
                        filterGroup = filterData.RecordFilter;
                    } else {
                        filterGroup = {
                            $type: "Vanrise.GenericData.Entities.RecordFilterGroup, Vanrise.GenericData.Entities",
                            LogicalOperator: VR_GenericData_RecordQueryLogicalOperatorEnum.And.value,
                            Filters: [filterData.RecordFilter]
                        };
                    }
                }

                if (filterData.Filters != undefined) {
                    filters = [];
                    for (var i = 0; i < filterData.Filters.length; i++) {
                        filters.push(filterData.Filters[i]);
                    }
                }
            }


            var gridPayload = {
                query: {
                    FilterGroup: filterGroup,
                    FromTime: filterData != undefined ? filterData.FromTime : undefined,
                    ToTime: filterData != undefined ? filterData.ToTime : undefined,
                    Filters: filters
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