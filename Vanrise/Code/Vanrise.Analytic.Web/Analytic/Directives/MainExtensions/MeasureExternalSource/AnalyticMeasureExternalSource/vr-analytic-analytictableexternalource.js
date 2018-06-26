"use strict";
app.directive("vrAnalyticAnalytictableexternalsource", ["UtilsService","VRUIUtilsService",
    function (UtilsService, VRUIUtilsService) {
        var directiveDefinitionObject = {
            restrict: "E",
            scope: { 
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var analyticTable = new AnalyticTable($scope, ctrl, $attrs);
                analyticTable.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Analytic/Directives/MainExtensions/MeasureExternalSource/AnalyticMeasureExternalSource/Templates/AnalyticTableMeasureExternalSourceTemplate.html"
        };
        function AnalyticTable($scope, ctrl, $attrs) {
            
            var context;
            var entity;
            var tableId;

            var tableSelectorAPI;
            var tableSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var gridAPI;
            var gridReadyDeferred = UtilsService.createPromiseDeferred();

            var measureGridAPI;
            var measureGridReadyDeferred = UtilsService.createPromiseDeferred();

            var tableSelectedDeferred;

            this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.onTableSelectorDirectiveReady = function (api) {
                    tableSelectorAPI = api;
                    tableSelectorReadyDeferred.resolve();
                };
                $scope.scopeModel.onTableSelectionChange = function (value) {

                    if (value != undefined) {
                        if (tableSelectedDeferred != undefined) {
                            tableSelectedDeferred.resolve();
                        }
                        else {
                            var setLoader = function (value) {
                                $scope.scopeModel.isLoadingDirective = value;
                            };

                            var payload = {
                                context: getContext(),
                                tableId: tableId
                            };

                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, gridAPI, payload, setLoader);

                            var measureSetLoader = function (value) {
                                $scope.scopeModel.isLoadingDirective = value;
                            };

                            var measurePayload = {
                                context: getContext(),
                                tableId: tableId
                            };

                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, measureGridAPI, measurePayload, measureSetLoader);
                        }


                    }

                };

                $scope.scopeModel.onGridDirectiveReady = function (api) {
                    gridAPI = api;
                    gridReadyDeferred.resolve();
                };

                $scope.scopeModel.onMeasureGridDirectiveReady = function (api) {
                    measureGridAPI = api;
                    measureGridReadyDeferred.resolve();
                };

                defineAPI();
                
            }
            function defineAPI() {

                var api = {};
                api.load = function (payload) {

                    var promises = [];

                    if (payload != undefined) {
                        context = payload.context;
                        entity = payload.entity;
                        tableId = payload.tableId;

                        if(entity != undefined)
                        {
                            tableSelectedDeferred = UtilsService.createPromiseDeferred();
                            promises.push(loadDimesionGrid());
                            promises.push(loadMeasureGrid());
                        }
                    }
                   
                    promises.push(loadTableSelector());

                    function loadTableSelector() {
                        var loadTableSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                        tableSelectorReadyDeferred.promise.then(function () {
                            var payload = {
                                filter: undefined,
                                selectedIds: entity!=undefined ? entity.AnalyticTableId : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(tableSelectorAPI, payload, loadTableSelectorPromiseDeferred);
                        });
                        return loadTableSelectorPromiseDeferred.promise;
                    }

                    function loadDimesionGrid() {
                        var loadGridPromiseDeferred = UtilsService.createPromiseDeferred();
                        UtilsService.waitMultiplePromises([tableSelectedDeferred.promise, gridReadyDeferred.promise]).then(function () {
                            var dimensionPayload = {
                                context: getContext(),                         
                                tableId: tableId,
                                rules: entity != undefined ? entity.DimensionMappingRules : undefined,
                            };
                            VRUIUtilsService.callDirectiveLoad(gridAPI, dimensionPayload, loadGridPromiseDeferred);
                        });
                        return loadGridPromiseDeferred.promise;
                    }

                    function loadMeasureGrid() {
                        var loadGridPromiseDeferred = UtilsService.createPromiseDeferred();
                        UtilsService.waitMultiplePromises([tableSelectedDeferred.promise,measureGridReadyDeferred.promise]).then(function () {
                            var measurePayload = {
                                context: getContext(),
                                tableId: tableId,
                                rules: entity != undefined ? entity.MeasureMappingRules : undefined,
                            };

                            VRUIUtilsService.callDirectiveLoad(measureGridAPI, measurePayload, loadGridPromiseDeferred);

                        });
                        return loadGridPromiseDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises).then(function () {
                        tableSelectedDeferred = undefined;
                    });
                   
                };
                api.getData = function () {
                   
                    var analyticTable = {
                        $type: "Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.AnalyticTableMeasureExternalSource, Vanrise.Analytic.MainExtensions",
                        AnalyticTableId: tableSelectorAPI.getSelectedIds(),
                        DimensionMappingRules: gridAPI.getData(),
                        MeasureMappingRules: measureGridAPI.getData()
                    };
                    return analyticTable;
                };
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
            function getContext() {
                
                var currentContext = context;
                
                if (currentContext == undefined) 
                    currentContext = {};

                currentContext.getAnalyticTableId = function () {
                    return tableSelectorAPI.getSelectedIds();
                };
              
                return currentContext;
            }
        }
        return directiveDefinitionObject;
    }]);