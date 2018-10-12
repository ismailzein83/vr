﻿'use strict';

app.directive('vrWhsRoutingRouterulesettingsFilterQuality', ['UtilsService', 'VRUIUtilsService', 'WhS_Routing_QualityConfigurationAPIService', 'WhS_Routing_QualityOptionTypeEnum',
function (UtilsService, VRUIUtilsService, WhS_Routing_QualityConfigurationAPIService, WhS_Routing_QualityOptionTypeEnum) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ctor = new filterByQualityCtor(ctrl, $scope);
            ctor.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {

                }
            };
        },
        templateUrl: function (element, attrs) {
            return '/Client/Modules/WhS_Routing/Directives/Extensions/RouteRuleSettings/Filter/Templates/FilterByQualityDirective.html';
        }
    };

    function filterByQualityCtor(ctrl, $scope) {
        this.initializeController = initializeController;

        var qualityConfigurationSelectorAPI;
        var qualityConfigurationPromiseDeferred = UtilsService.createPromiseDeferred();

        function initializeController() {
            $scope.scopeModel = {};
            $scope.qualityConfigurations = [];
            $scope.rateOptionTypes = [];


            $scope.scopeModel.onQualityConfigurationSelectorReady = function (api) {
                qualityConfigurationSelectorAPI = api;
                qualityConfigurationPromiseDeferred.resolve();
            };

            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                var promises = [];

                loadQualityOptionTypes();
                if (payload != undefined) {
                    $scope.scopeModel.qualityOptionType = UtilsService.getEnum(WhS_Routing_QualityOptionTypeEnum, 'value', payload.QualityOptionType);
                    $scope.scopeModel.qualityOptionValue = payload.QualityOptionValue;
                }
                var loadQualityConfigurationSelectorPromise = loadQualityConfigurationSelector();
                promises.push(loadQualityConfigurationSelectorPromise);
                function loadQualityConfigurationSelector() {
                    var loadQualityConfigurationPromiseDeferred = UtilsService.createPromiseDeferred();

                    qualityConfigurationPromiseDeferred.promise.then(function () {

                        var qualityConfigurationPayload = undefined;
                        if (payload != undefined) {
                            qualityConfigurationPayload = { selectedIds: payload.QualityConfigurationId };
                        }

                        VRUIUtilsService.callDirectiveLoad(qualityConfigurationSelectorAPI, qualityConfigurationPayload, loadQualityConfigurationPromiseDeferred);
                    });

                    return loadQualityConfigurationPromiseDeferred.promise;
                }

                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function () {
                return {
                    $type: "TOne.WhS.Routing.Business.RouteRules.Filters.QualityOptionFilter, TOne.WhS.Routing.Business",
                    QualityConfigurationId: qualityConfigurationSelectorAPI.getSelectedIds(),
                    QualityOptionType: $scope.scopeModel.qualityOptionType.value,
                    QualityOptionValue: $scope.scopeModel.qualityOptionValue
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
        function loadQualityOptionTypes() {
            $scope.qualityOptionTypes = UtilsService.getArrayEnum(WhS_Routing_QualityOptionTypeEnum);
        }
    }
    return directiveDefinitionObject;
}]);

