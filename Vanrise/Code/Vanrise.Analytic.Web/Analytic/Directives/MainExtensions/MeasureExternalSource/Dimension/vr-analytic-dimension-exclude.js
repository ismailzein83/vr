"use strict";
app.directive("vrAnalyticDimensionExclude", ["UtilsService","VRUIUtilsService",
    function (UtilsService, VRUIUtilsService) {
        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var excludeDimension = new ExcludeDimension($scope, ctrl, $attrs);
                excludeDimension.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Analytic/Directives/MainExtensions/MeasureExternalSource/Dimension/Templates/AnalyticExcludeDimensionTemplate.html"
        };
        function ExcludeDimension($scope, ctrl, $attrs) {

            var dimensionsSelectorAPI;
            var dimensionsSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onDimensionsSelectorReady = function (api) {
                    dimensionsSelectorAPI = api;
                    dimensionsSelectorReadyDeferred.resolve();
                };

                defineAPI();
            };
            function defineAPI() {
               
                var api = {};
                api.load = function (payload) {

                    var context;
                    var ruleEntity;
                    var tableId;
                    var promises = [];

                    if (payload != undefined) {

                        context = payload.context;
                        ruleEntity = payload.ruleEntity;
                        tableId = payload.tableId;

                        if (ruleEntity != undefined) {
                            var settings = ruleEntity.Entity != undefined ? ruleEntity.Entity.Settings : undefined;
                            var selectedIds = settings != undefined ? settings.ExcludedDimensions : undefined;
                        }
                    }
                    promises.push(loadDimensionsSelector());
                    function loadDimensionsSelector() {

                        var dimensionsSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                        dimensionsSelectorReadyDeferred.promise.then(function () {  

                            var payload = {
                                filter: { TableIds: [tableId] },
                                selectedIds: selectedIds
                            };
                            VRUIUtilsService.callDirectiveLoad(dimensionsSelectorAPI, payload, dimensionsSelectorLoadDeferred);
                        });
                        return dimensionsSelectorLoadDeferred.promise;
                    }
                    
                    return UtilsService.waitMultiplePromises(promises);
                };
                api.getData = function () {
                    return {
                        $type: "Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.DimensionMappingRules.ExcludeDimensions, Vanrise.Analytic.MainExtensions",
                        ExcludedDimensions:dimensionsSelectorAPI.getSelectedIds()
                    };
                };
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function getContext() {
                var currentContext = context;
                if (currentContext == undefined) 
                    currentContext = {};
                
                return currentContext;
            }
        }
        return directiveDefinitionObject;
    }
]);