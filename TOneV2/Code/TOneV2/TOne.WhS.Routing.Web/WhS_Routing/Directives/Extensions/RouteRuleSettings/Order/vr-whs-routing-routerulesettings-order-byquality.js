'use strict';

app.directive('vrWhsRoutingRouterulesettingsOrderByquality', ['UtilsService', 'VRUIUtilsService', 'WhS_Routing_QualityConfigurationAPIService',
    function (UtilsService, VRUIUtilsService, WhS_Routing_QualityConfigurationAPIService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new orderByQualityCtor(ctrl, $scope);
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
                return '/Client/Modules/WhS_Routing/Directives/Extensions/RouteRuleSettings/Order/Templates/OrderByQualityDirective.html';
            }
        };

        function orderByQualityCtor(ctrl, $scope) {
            this.initializeController = initializeController;

            var qualityConfigurationSelectorAPI;
            var qualityConfigurationPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.qualityConfigurations = [];

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
                        $type: "TOne.WhS.Routing.Business.RouteRules.Orders.OptionOrderByQuality, TOne.WhS.Routing.Business",
                        QualityConfigurationId: qualityConfigurationSelectorAPI.getSelectedIds()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
        return directiveDefinitionObject;
    }]);

