(function (appControllers) {
    "use strict";

    pageRunTimeManagementController.$inject = ['$scope', 'Demo_Module_PageRunTimeService', 'VRNotificationService', 'UtilsService', 'VRUIUtilsService', "Demo_Module_PageDefinitionAPIService"];

    function pageRunTimeManagementController($scope, Demo_Module_PageRunTimeService, VRNotificationService, UtilsService, VRUIUtilsService, Demo_Module_PageDefinitionAPIService) {

        var gridApi;
        var pageDefinitionEntity;
        var pageDefinitionDirectiveApi;
        var pageDefinitionReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        $scope.scopeModel = {};
        $scope.scopeModel.filters = [];
        defineScope();
        load();

        function defineScope() {

            $scope.scopeModel.search = function () {
               
                return gridApi.load(getFilter());

            };

            $scope.scopeModel.onGridReady = function (api) {
                gridApi = api;
            };

            $scope.scopeModel.addPageRunTime = function () {

                var selectedPageDefinitionId;

                if (pageDefinitionDirectiveApi.getSelectedIds() != undefined) {
                    selectedPageDefinitionId = pageDefinitionDirectiveApi.getSelectedIds();
                    var onPageRunTimeAdded = function (pageRunTime) {
                        if (gridApi != undefined) {
                            gridApi.onPageRunTimeAdded(pageRunTime);
                        }
                    };

                    Demo_Module_PageRunTimeService.addPageRunTime(onPageRunTimeAdded, selectedPageDefinitionId);
                }

            };

            $scope.scopeModel.onPageDefinitionSelectorReady = function (api) {
                pageDefinitionDirectiveApi = api;
                pageDefinitionReadyPromiseDeferred.resolve();

            };

            $scope.scopeModel.onPageDefinitionChanged = function () {
                $scope.scopeModel.filters = [];

                if (pageDefinitionDirectiveApi != undefined) {
                    var pageDefinitionId = pageDefinitionDirectiveApi.getSelectedIds();
                    if (pageDefinitionId != undefined) {
                        getPageDefinition(pageDefinitionId).then(function (response) {
                            gridApi.load(getFilter());

                            if ($scope.scopeModel.filters != undefined) {
                                $scope.scopeModel.isLoading = true;
                                var promises = [];
                                promises.push(loadFilterDirectives());
                                UtilsService.waitMultiplePromises(promises).then(function (response) {
                                    $scope.scopeModel.isLoading = false;
                                });
                            }
                        });

                        
                    }
                }
            };
        }

        function loadFilterDirectives()
        {
            var promises = [];
            if (pageDefinitionEntity != undefined && pageDefinitionEntity.Details != undefined && pageDefinitionEntity.Details.Filters != undefined) {
                for (var i = 0; i < pageDefinitionEntity.Details.Filters.length; i++) {
                    var filter = pageDefinitionEntity.Details.Filters[i];
                    var filterItem = {
                        filter: filter,
                        readyPromiseDeffered: UtilsService.createPromiseDeferred(),
                        loadPromiseDeffered : UtilsService.createPromiseDeferred()
                    };
                    promises.push(filterItem.loadPromiseDeffered.promise);
                    addFilterAPIDirective(filterItem);
                }
            }
            return UtilsService.waitMultiplePromises(promises);
        }

        function addFilterAPIDirective(filterItem)
        {
            var field = UtilsService.getItemByVal(pageDefinitionEntity.Details.Fields, filterItem.filter.fieldName, "Name");
            if (field != undefined) {
                var filter = {
                    runtimeFilterEditor: field.FieldType.RunTimeFilter,
                    fieldTitle: field.Title,
                    fieldName: field.Name
                };
                filter.onRunTimeFilterReady = function (api) {
                    filter.runTimeFilterDirectiveApi = api;
                    filterItem.readyPromiseDeffered.resolve();
                };

                filterItem.readyPromiseDeffered.promise.then(function (response) {
                    VRUIUtilsService.callDirectiveLoad(filter.runTimeFilterDirectiveApi, undefined, filterItem.loadPromiseDeffered);
                });
                $scope.scopeModel.filters.push(filter);
            } 
        }

        //function prepareFilter(filter) {

        //    filter.runTimeFilterReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        //    filter.onRunTimeFilterReady = function (api) {
        //        filter.runTimeFilterDirectiveApi = api;
        //        filter.runTimeFilterReadyPromiseDeferred.resolve();
        //    }
        //    filter.runTimeFilterLoadPromiseDeferred = UtilsService.createPromiseDeferred();
        //}

        //function loadRunTimeFilterDirective(filter) {

        //    filter.runTimeFilterReadyPromiseDeferred.promise.then(function (response) {
        //        filter.runTimeFilterPayload = {};
        //        VRUIUtilsService.callDirectiveLoad(filter.runTimeFilterDirectiveApi, filter.runTimeFilterPayload, filter.runTimeFilterLoadPromiseDeferred);
        //    });
        //    return filter.runTimeFilterLoadPromiseDeferred.promise;
        //}

        function getPageDefinition(pageDefinitionId) {
           
            return Demo_Module_PageDefinitionAPIService.GetPageDefinitionById(pageDefinitionId).then(function (response) {
                pageDefinitionEntity = response; 
                //if (pageDefinitionEntity != undefined && pageDefinitionEntity.Details!=undefined && pageDefinitionEntity.Details.Filters != undefined) {
                //    for (var i = 0; i < pageDefinitionEntity.Details.Filters.length; i++) {

                //        var filter = pageDefinitionEntity.Details.Filters[i];
                //        for (var j = 0; j < pageDefinitionEntity.Details.Fields.length; j++) {

                //            if (pageDefinitionEntity.Details.Fields[j].Name == filter.fieldName) {
                //                $scope.scopeModel.filters.push(pageDefinitionEntity.Details.Fields[j]);
                //                break;

                //            }
                //        }
                //    }
                //}
            });
        }

        function load() {

            $scope.scopeModel.isLoading = true;
            loadAllControls();

        }

        function loadAllControls() {

            function loadPageDefinitionSelector() {
                var pageDefinitionLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                pageDefinitionReadyPromiseDeferred.promise.then(function () {
                    var pageDefinitionDirectivePayload = {};
                
                    VRUIUtilsService.callDirectiveLoad(pageDefinitionDirectiveApi, pageDefinitionDirectivePayload, pageDefinitionLoadPromiseDeferred);
                });
               
                return pageDefinitionLoadPromiseDeferred.promise;
            };


            return UtilsService.waitMultipleAsyncOperations([loadPageDefinitionSelector])
              .catch(function (error) {
                  VRNotificationService.notifyExceptionWithClose(error, $scope);
              })
             .finally(function () {
                 $scope.scopeModel.isLoading = false;
             });
        }

        function getFilter() {

            var object = {};
            var filters = {};
            var filtersValues = {}

            if ($scope.scopeModel.filters != undefined) {
            for (var j = 0; j < $scope.scopeModel.filters.length; j++) {
                var filter = $scope.scopeModel.filters[j];
                var key = filter.fieldName;
                if (filter.runTimeFilterDirectiveApi != undefined)
                    filtersValues[key] = filter.runTimeFilterDirectiveApi.getData();
            } 
            }
            object.query = { pageDefinitionId: pageDefinitionEntity.PageDefinitionId, Filters: filtersValues };
            object.fields = [];
            object.subviews = [];

            if (pageDefinitionEntity.Details != undefined) {
                if (pageDefinitionEntity.Details.Fields != undefined) {
                    for (var i = 0; i < pageDefinitionEntity.Details.Fields.length; i++) {
                        var field = pageDefinitionEntity.Details.Fields[i];
                        object.fields.push(field);
                    }
                }

                if (pageDefinitionEntity.Details.SubViews != undefined) {
                    for (var i = 0; i < pageDefinitionEntity.Details.SubViews.length; i++) {
                        var subview = pageDefinitionEntity.Details.SubViews[i];
                        object.subviews.push(subview);
                    }
                }
            }
            return object;
        }

    };

    appControllers.controller('Demo_Module_PageRunTimeManagementController', pageRunTimeManagementController);
})(appControllers);