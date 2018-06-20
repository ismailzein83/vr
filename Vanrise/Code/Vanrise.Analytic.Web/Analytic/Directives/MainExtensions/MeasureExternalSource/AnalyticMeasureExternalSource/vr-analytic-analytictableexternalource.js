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
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, measureGridAPI, payload, setLoader);
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
                            promises.push(loadGrid());
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

                    function loadGrid() {
                        var loadGridPromiseDeferred = UtilsService.createPromiseDeferred();
                        UtilsService.waitMultiplePromises([tableSelectedDeferred.promise, gridReadyDeferred.promise, measureGridReadyDeferred.promise]).then(function () {
                            tableSelectedDeferred = undefined;
                            var dimensionPayload = {
                                context: getContext(),                         
                                tableId: tableId,
                                rules: entity != undefined ? entity.DimensionMappingRules : undefined,

                            };
                            var measurePayload = {
                                context: getContext(),
                                tableId: tableId,
                                rules: entity != undefined ? entity.MeasureMappingRules : undefined,

                            };
                            VRUIUtilsService.callDirectiveLoad(gridAPI, dimensionPayload, loadGridPromiseDeferred);
                            VRUIUtilsService.callDirectiveLoad(measureGridAPI, measurePayload, loadGridPromiseDeferred);

                        });
                        return loadGridPromiseDeferred.promise;

                    }
                    return UtilsService.waitMultiplePromises(promises);
                   
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