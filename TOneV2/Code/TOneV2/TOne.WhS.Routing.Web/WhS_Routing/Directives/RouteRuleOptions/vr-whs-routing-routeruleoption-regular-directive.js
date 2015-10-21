'use strict';
app.directive('vrWhsRoutingRouteruleoptionRegular', ['UtilsService', 'WhS_Routing_RoutRuleSettingsAPIService', 'VRUIUtilsService',
    function (UtilsService, WhS_Routing_RoutRuleSettingsAPIService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var beRouteRuleRegularCtor = new beRouteRuleRegularCtor(ctrl, $scope);
                beRouteRuleRegularCtor.initializeController();

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
                return getBeRouteRuleRegularTemplate(attrs);
            }

        };

        function getBeRouteRuleRegularTemplate(attrs) {
            return '/Client/Modules/WhS_BusinessEntity/Directives/RouteRule/Templates/RouteRuleRegularDirectiveTemplate.html';
        }

        function beRouteRuleRegularCtor(ctrl, $scope) {
            var appendixDirectiveData;

            $scope.onOptionSettingsGroupDirectiveReady = function (api) {
                routeOptionSettingsGroupDirectiveAPI = api;

                if (appendixDirectiveData != undefined)
                    tryLoadAppendixDirectives();
                else
                    VRUIUtilsService.loadDirective($scope, routeOptionSettingsGroupDirectiveAPI, 'optionSettingsAppendixLoader');
            }

            $scope.onOptionOrderSettingsGroupDirectiveReady = function (api) {
                routeRuleOptionOrderSettingsDirectiveAPI = api;

                if (appendixDirectiveData != undefined)
                    tryLoadAppendixDirectives();
                else
                    VRUIUtilsService.loadDirective($scope, routeRuleOptionOrderSettingsDirectiveAPI, 'optionOrderSettingsAppendixLoader');
            }

            $scope.onOptionFilterSettingsGroupDirectiveReady = function (api) {
                routeRuleOptionFilterSettingsDirectiveAPI = api;

                if (appendixDirectiveData != undefined)
                    tryLoadAppendixDirectives();
                else
                    VRUIUtilsService.loadDirective($scope, routeRuleOptionFilterSettingsDirectiveAPI, 'optionFilterSettingsAppendixLoader');
            }

            $scope.onOptionPercentageSettingsGroupDirectiveReady = function (api) {
                routeRuleOptionPercentageSettingsDirectiveAPI = api;

                if (appendixDirectiveData != undefined)
                    tryLoadAppendixDirectives();
                else
                    VRUIUtilsService.loadDirective($scope, routeRuleOptionPercentageSettingsDirectiveAPI, 'optionPercentageSettingsAppendixLoader');
            }

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function () {

                    return UtilsService.waitMultipleAsyncOperations(loadOptionSettingsGroupTemplates, loadOptionOrderSettingsGroupTemplates,
                        loadOptionFilterSettingsGroupTemplates, loadOptionPercentageSettingsGroupTemplates);

                    function loadOptionSettingsGroupTemplates() {
                        return WhS_Routing_RoutRuleSettingsAPIService.GetRouteOptionSettingsGroupTemplates().then(function (response) {
                            angular.forEach(response, function (item) {
                                $scope.optionSettingsGroupTemplates.push(item);
                            });
                        });
                    }

                    function loadOptionOrderSettingsGroupTemplates() {
                        return WhS_Routing_RoutRuleSettingsAPIService.GetRouteOptionOrderSettingsTemplates().then(function (response) {
                            angular.forEach(response, function (item) {
                                $scope.optionOrderSettingsGroupTemplates.push(item);
                            });
                        });
                    }

                    function loadOptionFilterSettingsGroupTemplates() {
                        return WhS_Routing_RoutRuleSettingsAPIService.GetRouteOptionFilterSettingsTemplates().then(function (response) {
                            angular.forEach(response, function (item) {
                                $scope.optionFilterSettingsGroupTemnplates.push(item);
                            });
                        });
                    }

                    function loadOptionPercentageSettingsGroupTemplates() {
                        return WhS_Routing_RoutRuleSettingsAPIService.GetRouteOptionPercentageSettingsTemplates().then(function (response) {
                            angular.forEach(response, function (item) {
                                $scope.optionPercentageSettingsGroupTemplates.push(item);
                            });
                        });
                    }
                }

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.Routing.Business.RouteRules.RegularRouteRule, TOne.WhS.Routing.Business.RouteRules",
                        OptionsSettingsGroup: routeOptionSettingsGroupDirectiveAPI.getData(),
                        OptionOrderSettings: routeRuleOptionOrderSettingsDirectiveAPI.getData(),
                        OptionFilterSettings: routeRuleOptionFilterSettingsDirectiveAPI.getData(),
                        OptionPercentageSettings: routeRuleOptionPercentageSettingsDirectiveAPI.getData()
                    };
                }

                api.setData = function (routeRuleSettings) {

                    fillScopeFromRouteRuleSettings(routeRuleSettings);
                    appendixDirectiveData = routeRuleSettings;
                    tryLoadAppendixDirectives();

                    function fillScopeFromRouteRuleSettings(routeRuleSettings) {

                        if(routeRuleSettings.OptionsSettingsGroup != null)
                            $scope.selectedOptionSettingsGroupTemplate = UtilsService.getItemByVal($scope.optionSettingsGroupTemplates, routeRuleSettings.OptionsSettingsGroup.ConfigId, "TemplateConfigID");
                        if(routeRuleSettings.OptionOrderSettings != null)
                            $scope.selectedOptionOrderSettingsGroupTemplate = UtilsService.getItemByVal($scope.optionOrderSettingsGroupTemplates, routeRuleSettings.OptionOrderSettings.ConfigId, "TemplateConfigID");
                        if (routeRuleSettings.OptionFilterSettings != null)
                            $scope.selectedOptionFilterSettingsGroupTemplate = UtilsService.getItemByVal($scope.optionFilterSettingsGroupTemnplates, routeRuleSettings.OptionFilterSettings.ConfigId, "TemplateConfigID");
                        if (routeRuleSettings.OptionPercentageSettings != null)
                            $scope.selectedPercentageFilterSettingsGroupTemplate = UtilsService.getItemByVal($scope.optionPercentageSettingsGroupTemplates, routeRuleSettings.OptionPercentageSettings.ConfigId, "TemplateConfigID");
                    };
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function tryLoadAppendixDirectives() {
                var loadOperations = [];
                var setDirectivesDataOperations = [];

                if ($scope.selectedOptionSettingsGroupTemplate != undefined) {
                    if (routeOptionSettingsGroupDirectiveAPI == undefined)
                        return;

                    loadOperations.push(routeOptionSettingsGroupDirectiveAPI.load);
                    setDirectivesDataOperations.push(setRouteOptionSettings);
                }

                if ($scope.selectedOptionOrderSettingsGroupTemplate != undefined) {
                    if (routeRuleOptionOrderSettingsDirectiveAPI == undefined)
                        return;

                    loadOperations.push(routeRuleOptionOrderSettingsDirectiveAPI.load);
                    setDirectivesDataOperations.push(setRouteRuleOptionOrderSettings);
                }

                if ($scope.selectedOptionFilterSettingsGroupTemplate != undefined) {
                    if (routeRuleOptionFilterSettingsDirectiveAPI == undefined)
                        return;

                    loadOperations.push(routeRuleOptionFilterSettingsDirectiveAPI.load);
                    setDirectivesDataOperations.push(setRouteRuleOptionFilterSettings);
                }

                if ($scope.selectedPercentageFilterSettingsGroupTemplate != undefined) {
                    if (routeRuleOptionPercentageSettingsDirectiveAPI == undefined)
                        return;

                    loadOperations.push(routeRuleOptionPercentageSettingsDirectiveAPI.load);
                    setDirectivesDataOperations.push(setRouteRuleOptionPercentageSettings);
                }

                UtilsService.waitMultipleAsyncOperations(loadOperations).then(function () {
                    setAppendixDirectives();
                });

                function setAppendixDirectives() {
                    function setAppendixDirectives() {
                        UtilsService.waitMultipleAsyncOperations(setDirectivesDataOperations).then(function () {
                            appendixDirectiveData = undefined;
                        });
                    }
                }

                function setRouteOptionSettings() {
                    routeOptionSettingsGroupDirectiveAPI.setData(appendixDirectiveData.OptionsSettingsGroup);
                };
                function setRouteRuleOptionOrderSettings() {
                    routeRuleOptionOrderSettingsDirectiveAPI.setData(appendixDirectiveData.OptionOrderSettings);
                };
                function setRouteRuleOptionFilterSettings() {
                    routeRuleOptionFilterSettingsDirectiveAPI.setData(appendixDirectiveData.OptionFilterSettings);
                };
                function setRouteRuleOptionPercentageSettings() {
                    routeRuleOptionPercentageSettingsDirectiveAPI.setData(appendixDirectiveData.OptionPercentageSettings);
                };
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }]);