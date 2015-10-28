'use strict';
app.directive('vrWhsRoutingRouterulesettingsRegular', ['UtilsService', 'WhS_Routing_RoutRuleSettingsAPIService', 'VRUIUtilsService',
    function (UtilsService, WhS_Routing_RoutRuleSettingsAPIService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new regularCtor(ctrl, $scope);
                ctor.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                }
            },
            templateUrl: function (element, attrs) {
                return '/Client/Modules/WhS_Routing/Directives/Extensions/RouteRuleSettings/Templates/RouteRuleRegularDirective.html';
            }

        };

        function regularCtor(ctrl, $scope) {
            var routeOptionSettingsGroupDirectiveAPI;
            var routeOptionSettingsReadyPromiseDeferred;

            var routeRuleOptionOrderSettingsDirectiveAPI;
            var routeRuleOptionOrderSettingsReadyPromiseDeferred;

            var routeRuleOptionFilterSettingsDirectiveAPI;
            var routeRuleOptionFilterSettingsReadyPromiseDeferred;

            var routeRuleOptionPercentageSettingsDirectiveAPI;
            var routeRuleOptionPercentageSettingsReadyPromiseDeferred;

            function initializeController() {

                $scope.optionOrderSettingsGroupTemplates = [];
                $scope.optionSettingsGroupTemplates = [];
                $scope.optionFilterSettingsGroupTemplates = [];
                $scope.optionPercentageSettingsGroupTemplates = [];

                $scope.onOptionSettingsGroupDirectiveReady = function (api) {
                    routeOptionSettingsGroupDirectiveAPI = api;
                    var setLoader = function (value) { $scope.isLoadingOptionSettingsSection = value };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, routeOptionSettingsGroupDirectiveAPI, undefined, setLoader, routeOptionSettingsReadyPromiseDeferred);
                }

                $scope.onOptionOrderSettingsGroupDirectiveReady = function (api) {
                    routeRuleOptionOrderSettingsDirectiveAPI = api;
                    var setLoader = function (value) { $scope.isLoadingOptionOrderSettingsSection = value };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, routeRuleOptionOrderSettingsDirectiveAPI, undefined, setLoader, routeRuleOptionOrderSettingsReadyPromiseDeferred);
                }

                $scope.onOptionFilterSettingsGroupDirectiveReady = function (api) {
                    routeRuleOptionFilterSettingsDirectiveAPI = api;
                    var setLoader = function (value) { $scope.isLoadingOptionFilterSettingsSection = value };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, routeRuleOptionFilterSettingsDirectiveAPI, undefined, setLoader, routeRuleOptionFilterSettingsReadyPromiseDeferred);
                }

                $scope.onOptionPercentageSettingsGroupDirectiveReady = function (api) {
                    routeRuleOptionPercentageSettingsDirectiveAPI = api;
                    var setLoader = function (value) { $scope.isLoadingOptionPercentageSettingsSection = value };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, routeRuleOptionPercentageSettingsDirectiveAPI, undefined, setLoader, routeRuleOptionPercentageSettingsReadyPromiseDeferred);
                }

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                   
                    return UtilsService.waitMultipleAsyncOperations([loadOptionSettingsGroupSection, loadOptionOrderSettingsGroupSection,
                        loadOptionFilterSettingsGroupSection, loadOptionPercentageSettingsGroupSection]);

                    function loadOptionSettingsGroupSection() {

                        var promises = [];
                        var optionSettingsGroupPayload;

                        if (payload != undefined && payload.OptionsSettingsGroup != null)
                            optionSettingsGroupPayload = payload.OptionsSettingsGroup;

                        var loadOptionSettingsGroupTemplatesPromise = WhS_Routing_RoutRuleSettingsAPIService.GetRouteOptionSettingsGroupTemplates().then(function (response) {
                            angular.forEach(response, function (item) {
                                $scope.optionSettingsGroupTemplates.push(item);
                            });

                            if (optionSettingsGroupPayload != undefined) {
                                $scope.selectedOptionSettingsGroupTemplate = UtilsService.getItemByVal($scope.optionSettingsGroupTemplates, optionSettingsGroupPayload.ConfigId, "TemplateConfigID");
                            }
                        });

                        promises.push(loadOptionSettingsGroupTemplatesPromise);

                        if (optionSettingsGroupPayload != undefined) {
                            routeOptionSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

                            var routeOptionSettingsLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                            promises.push(routeOptionSettingsLoadPromiseDeferred.promise);

                            routeOptionSettingsReadyPromiseDeferred.promise.then(function () {
                                routeOptionSettingsReadyPromiseDeferred = undefined;
                                VRUIUtilsService.callDirectiveLoad(routeOptionSettingsGroupDirectiveAPI, optionSettingsGroupPayload, routeOptionSettingsLoadPromiseDeferred);
                            });
                        }
                        return UtilsService.waitMultiplePromises(promises);
                    }

                    function loadOptionOrderSettingsGroupSection() {
                        var promises = [];
                        var optionOrderSettingsGroupPayload;

                        if (payload != undefined && payload.OptionOrderSettings != null)
                            optionOrderSettingsGroupPayload = payload.OptionOrderSettings;

                        var loadRouteOptionOrderSettingsGroupTemplatesPromise = WhS_Routing_RoutRuleSettingsAPIService.GetRouteOptionOrderSettingsTemplates().then(function (response) {
                            angular.forEach(response, function (item) {
                                $scope.optionOrderSettingsGroupTemplates.push(item);
                            });

                            if (optionOrderSettingsGroupPayload != undefined)
                                $scope.selectedOptionOrderSettingsGroupTemplate = UtilsService.getItemByVal($scope.optionOrderSettingsGroupTemplates, optionOrderSettingsGroupPayload.ConfigId, "TemplateConfigID");
                        });

                        promises.push(loadRouteOptionOrderSettingsGroupTemplatesPromise);

                        if (optionOrderSettingsGroupPayload != undefined) {
                            routeRuleOptionOrderSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

                            var routeRuleOptionOrderSettingsLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                            promises.push(routeRuleOptionOrderSettingsLoadPromiseDeferred.promise);

                            routeRuleOptionOrderSettingsReadyPromiseDeferred.promise.then(function () {
                                routeRuleOptionOrderSettingsReadyPromiseDeferred = undefined;
                                VRUIUtilsService.callDirectiveLoad(routeRuleOptionOrderSettingsDirectiveAPI, optionOrderSettingsGroupPayload, routeRuleOptionOrderSettingsLoadPromiseDeferred);
                            });
                        }
                        return UtilsService.waitMultiplePromises(promises);
                    }

                    function loadOptionFilterSettingsGroupSection() {
                        var promises = [];
                        var optionFilterSettingsGroupPayload;

                        if (payload != undefined && payload.OptionFilterSettings != null)
                            optionFilterSettingsGroupPayload = payload.OptionFilterSettings;

                        var loadRouteOptionFilterSettingsGroupTemplatesPromise = WhS_Routing_RoutRuleSettingsAPIService.GetRouteOptionFilterSettingsTemplates().then(function (response) {
                            angular.forEach(response, function (item) {
                                $scope.optionFilterSettingsGroupTemplates.push(item);
                            });

                            if (optionFilterSettingsGroupPayload != undefined)
                                $scope.selectedOptionFilterSettingsGroupTemplate = UtilsService.getItemByVal($scope.optionFilterSettingsGroupTemplates, optionFilterSettingsGroupPayload.ConfigId, "TemplateConfigID");
                        });

                        promises.push(loadRouteOptionFilterSettingsGroupTemplatesPromise);

                        if (optionFilterSettingsGroupPayload != undefined) {
                            routeRuleOptionFilterSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

                            var routeRuleOptionFilterSettingsLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                            promises.push(routeRuleOptionFilterSettingsLoadPromiseDeferred.promise);

                            routeRuleOptionFilterSettingsReadyPromiseDeferred.promise.then(function () {
                                routeRuleOptionFilterSettingsReadyPromiseDeferred = undefined;
                                VRUIUtilsService.callDirectiveLoad(routeRuleOptionFilterSettingsDirectiveAPI, optionFilterSettingsGroupPayload, routeRuleOptionFilterSettingsLoadPromiseDeferred);
                            });
                        }

                        return UtilsService.waitMultiplePromises(promises);
                    }

                    function loadOptionPercentageSettingsGroupSection() {
                        var promises = [];
                        var optionPercentageSettingsGroupPayload;

                        if (payload != undefined && payload.OptionPercentageSettings != null)
                            optionPercentageSettingsGroupPayload = payload.OptionPercentageSettings;

                        var loadRouteOptionPercentageSettingsGroupTemplatesPromise = WhS_Routing_RoutRuleSettingsAPIService.GetRouteOptionPercentageSettingsTemplates().then(function (response) {
                            angular.forEach(response, function (item) {
                                $scope.optionPercentageSettingsGroupTemplates.push(item);
                            });

                            if (optionPercentageSettingsGroupPayload != undefined)
                                $scope.selectedPercentageSettingsGroupTemplate = UtilsService.getItemByVal($scope.optionPercentageSettingsGroupTemplates, optionPercentageSettingsGroupPayload.ConfigId, "TemplateConfigID");
                        });

                        promises.push(loadRouteOptionPercentageSettingsGroupTemplatesPromise);

                        if (optionPercentageSettingsGroupPayload != undefined) {
                            routeRuleOptionPercentageSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

                            var routeRuleOptionPercentageSettingsLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                            promises.push(routeRuleOptionPercentageSettingsLoadPromiseDeferred.promise);

                            routeRuleOptionPercentageSettingsReadyPromiseDeferred.promise.then(function () {
                                routeRuleOptionPercentageSettingsReadyPromiseDeferred = undefined;
                                VRUIUtilsService.callDirectiveLoad(routeRuleOptionPercentageSettingsDirectiveAPI, optionPercentageSettingsGroupPayload, routeRuleOptionPercentageSettingsLoadPromiseDeferred);
                            });
                        }

                        return UtilsService.waitMultiplePromises(promises);
                    }
                }

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.Routing.Business.RouteRules.RegularRouteRule, TOne.WhS.Routing.Business",
                        OptionsSettingsGroup: VRUIUtilsService.getSettingsFromDirective($scope, routeOptionSettingsGroupDirectiveAPI, 'selectedOptionSettingsGroupTemplate'),
                        OptionOrderSettings: VRUIUtilsService.getSettingsFromDirective($scope, routeRuleOptionOrderSettingsDirectiveAPI, 'selectedOptionOrderSettingsGroupTemplate'),
                        OptionFilterSettings: VRUIUtilsService.getSettingsFromDirective($scope, routeRuleOptionFilterSettingsDirectiveAPI, 'selectedOptionFilterSettingsGroupTemplate'),
                        OptionPercentageSettings: VRUIUtilsService.getSettingsFromDirective($scope, routeRuleOptionPercentageSettingsDirectiveAPI, 'selectedPercentageSettingsGroupTemplate')
                    };
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }]);