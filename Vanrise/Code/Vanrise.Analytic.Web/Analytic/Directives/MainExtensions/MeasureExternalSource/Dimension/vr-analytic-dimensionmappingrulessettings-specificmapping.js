"use strict";

app.directive("vrAnalyticDimensionmappingrulessettingsSpecificmapping", ["UtilsService", "VRUIUtilsService",
function (UtilsService, VRUIUtilsService) {
    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var specificMapping = new SpecificMapping($scope, ctrl, $attrs);
            specificMapping.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/Analytic/Directives/MainExtensions/MeasureExternalSource/Dimension/Templates/SpecificMappingDimensionTemplate.html"
    };
    function SpecificMapping($scope, ctrl, $attrs) {

        var dimensionsSelectorAPI;
        var mappedDimensionsSelectorAPI;
        var dimensionsSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var mappedDimensionsSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        this.initializeController = initializeController;
        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.onDimensionsSelectorReady = function (api) {
                dimensionsSelectorAPI = api;
                dimensionsSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onMappedDimensionsSelectorReady = function (api) {
                mappedDimensionsSelectorAPI = api;
                mappedDimensionsSelectorReadyDeferred.resolve();
            };

            defineAPI();
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {

                var context;
                var ruleEntity;
                var tableId;
                var promises = [];
                var settings;

                if (payload != undefined) {
                    context = payload.context;
                    ruleEntity = payload.ruleEntity;
                    tableId = payload.tableId;
                    if (ruleEntity != undefined) {
                       settings = ruleEntity.Entity != undefined ? ruleEntity.Entity.Settings : undefined;
                    }
                }
                promises.push(loadDimensionsSelector());
                promises.push(loadMappedDimensionsSelector());

                function loadDimensionsSelector() {

                    var dimensionsSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    dimensionsSelectorReadyDeferred.promise.then(function () {

                        var payload = {
                            filter: { TableIds: [tableId] },
                            selectedIds: settings != undefined ? settings.DimensionName : undefined,
                        };
                        VRUIUtilsService.callDirectiveLoad(dimensionsSelectorAPI, payload, dimensionsSelectorLoadDeferred);
                    });
                    return dimensionsSelectorLoadDeferred.promise;
                }

                function loadMappedDimensionsSelector() {

                    var dimensionsSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    mappedDimensionsSelectorReadyDeferred.promise.then(function () {

                        var payload = {
                            filter: { TableIds: [context.getAnalyticTableId()] },
                            selectedIds: settings != undefined ? settings.MappedDimensionName : undefined,
                        };
                        VRUIUtilsService.callDirectiveLoad(mappedDimensionsSelectorAPI, payload, dimensionsSelectorLoadDeferred);
                    });
                    return dimensionsSelectorLoadDeferred.promise;
                }

                return UtilsService.waitMultiplePromises(promises);
            };
            api.getData = function () {
                return {
                    $type: "Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.DimensionMappingRules.SpecificDimensionMapping, Vanrise.Analytic.MainExtensions",
                    MappedDimensionName: mappedDimensionsSelectorAPI.getSelectedIds(),
                    DimensionName: dimensionsSelectorAPI.getSelectedIds()
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