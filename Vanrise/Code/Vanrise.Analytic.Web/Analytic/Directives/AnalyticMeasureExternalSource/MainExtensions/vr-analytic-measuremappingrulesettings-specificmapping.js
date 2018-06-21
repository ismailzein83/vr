"use strict";

app.directive("vrAnalyticMeasuremappingrulesettingsSpecificmapping", ["UtilsService", "VRUIUtilsService",
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
        templateUrl: "/Client/Modules/Analytic/Directives/AnalyticMeasureExternalSource/MainExtensions/Templates/SpecificMappingMeasureTemplate.html"
    };
    function SpecificMapping($scope, ctrl, $attrs) {

        var measuresSelectorAPI;
        var mappedMeasuresSelectorAPI;
        var measuresSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var mappedMeasuresSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        this.initializeController = initializeController;
        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.onMeasuresSelectorReady = function (api) {
                measuresSelectorAPI = api;
                measuresSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onMappedMeasuresSelectorReady = function (api) {
                mappedMeasuresSelectorAPI = api;
                mappedMeasuresSelectorReadyDeferred.resolve();
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
                promises.push(loadMeasuresSelector());
                promises.push(loadMappedMeasuresSelector());

                function loadMeasuresSelector() {

                    var measuresSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    measuresSelectorReadyDeferred.promise.then(function () {

                        var payload = {
                            filter: { TableIds: [tableId] },
                            selectedIds: settings != undefined ? settings.MeasureName : undefined,
                        };
                        VRUIUtilsService.callDirectiveLoad(measuresSelectorAPI, payload, measuresSelectorLoadDeferred);
                    });
                    return measuresSelectorLoadDeferred.promise;
                }

                function loadMappedMeasuresSelector() {

                    var measuresSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    mappedMeasuresSelectorReadyDeferred.promise.then(function () {

                        var payload = {
                            filter: { TableIds: [context.getAnalyticTableId()] },
                            selectedIds: settings != undefined ? settings.MappedMeasures : undefined,
                        };
                        VRUIUtilsService.callDirectiveLoad(mappedMeasuresSelectorAPI, payload, measuresSelectorLoadDeferred);
                    });
                    return measuresSelectorLoadDeferred.promise;
                }

                return UtilsService.waitMultiplePromises(promises);
            };
            api.getData = function () {
               
                return {
                    $type: "Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.MeasureMappingRules.SpecificMapping, Vanrise.Analytic.MainExtensions",
                    MappedMeasures: mappedMeasuresSelectorAPI.getSelectedIds(),
                    MeasureName: measuresSelectorAPI.getSelectedIds()
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