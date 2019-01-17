﻿'use strict';
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
                };
            },
            templateUrl: function (element, attrs) {
                return '/Client/Modules/WhS_Routing/Directives/Extensions/RouteRuleSettings/Templates/RouteRuleRegularDirective.html';
            }

        };

        function regularCtor(ctrl, $scope) {
            var context;
            var routeOptionSettingsGroupDirectiveAPI;
            var routeOptionSettingsReadyPromiseDeferred;

            var routeRuleOptionOrderSettingsDirectiveAPI;
            var routeRuleOptionOrderSettingsReadyPromiseDeferred;

            var routeRuleOptionFilterSettingsDirectiveAPI;
            var routeRuleOptionFilterSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var optionOrderTypeDirectiveAPI;
            var optionOrderTypeReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var routeRuleOptionPercentageSettingsDirectiveAPI;
            var routeRuleOptionPercentageSettingsReadyPromiseDeferred;

            var optionTypeSelectorAPI;
            var percentageOptionsSelectorAPI;

            var supplierFilterSettings;

            function initializeController() {
                $scope.scopeModel = [];
                $scope.optionOrderSettingsGroupTemplates = [];
                $scope.optionSettingsGroupTemplates = [];
                $scope.optionFilterSettingsGroupTemplates = [];
                $scope.scopeModel.optionPercentageSettingsGroupTemplates = [];

                $scope.scopeModel.showPercentage = true;
                $scope.scopeModel.showPercentageSettings = true;

                $scope.onOptionSettingsGroupDirectiveReady = function (api) {
                    routeOptionSettingsGroupDirectiveAPI = api;
                    var setLoader = function (value) { $scope.isLoadingOptionSettingsSection = value; };

                    var optionSettingsGroupPayload = { context: getContext() };

                    if (supplierFilterSettings != undefined)
                        optionSettingsGroupPayload.SupplierFilterSettings = supplierFilterSettings;

                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, routeOptionSettingsGroupDirectiveAPI, optionSettingsGroupPayload, setLoader, routeOptionSettingsReadyPromiseDeferred);
                };

                //$scope.onOptionOrderSettingsGroupDirectiveReady = function (api) {
                //    routeRuleOptionOrderSettingsDirectiveAPI = api;
                //    var setLoader = function (value) { $scope.isLoadingOptionOrderSettingsSection = value };
                //    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, routeRuleOptionOrderSettingsDirectiveAPI, undefined, setLoader, routeRuleOptionOrderSettingsReadyPromiseDeferred);
                //}

                $scope.onOptionFilterSettingsGroupDirectiveReady = function (api) {
                    routeRuleOptionFilterSettingsDirectiveAPI = api;
                    routeRuleOptionFilterSettingsReadyPromiseDeferred.resolve();
                };

                $scope.onOptionOrderTypeDirectiveReady = function (api) {
                    optionOrderTypeDirectiveAPI = api;
                    optionOrderTypeReadyPromiseDeferred.resolve();
                };

                $scope.onOptionTypeSelectorReady = function (api) {
                    optionTypeSelectorAPI = api;
                };
                $scope.onPercentageSettingsSelectorReady = function (api) {
                    percentageOptionsSelectorAPI = api;
                };

                $scope.scopeModel.onOptionPercentageSettingsGroupDirectiveReady = function (api) {
                    routeRuleOptionPercentageSettingsDirectiveAPI = api;
                    var setLoader = function (value) { $scope.scopeModel.isLoadingOptionPercentageSettingsSection = value; };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, routeRuleOptionPercentageSettingsDirectiveAPI, undefined, setLoader, routeRuleOptionPercentageSettingsReadyPromiseDeferred);
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined && payload.isExtendingSuppliers) {
                        var promises = [];
                        if (payload.OptionsSettingsGroup != undefined != undefined) {

                            var routeOptionSettingsLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                            promises.push(routeOptionSettingsLoadPromiseDeferred.promise);
                            var optionSettingsGroupPayload = {
                                SupplierFilterSettings: supplierFilterSettings,
                                OptionsSettingsGroup: payload.RouteRuleSettings.OptionsSettingsGroup,
                                context: payload.context
                            };
                            VRUIUtilsService.callDirectiveLoad(routeOptionSettingsGroupDirectiveAPI, optionSettingsGroupPayload, routeOptionSettingsLoadPromiseDeferred);
                        }
                        return UtilsService.waitMultiplePromises(promises);
                    }
                    else {
                        if (optionTypeSelectorAPI != undefined)
                            optionTypeSelectorAPI.clearDataSource();

                        if (percentageOptionsSelectorAPI != undefined)
                            percentageOptionsSelectorAPI.clearDataSource();

                        return UtilsService.waitMultipleAsyncOperations([loadOptionSettingsGroupSection,
                            loadOptionFilterSettingsGroupSection, loadOptionOrderTypeGroupSection, loadOptionPercentageSettingsGroupSection]);
                    }
                    function loadOptionSettingsGroupSection() {

                        var promises = [];
                        var optionSettingsGroupPayload;

                        if (payload != undefined) {
                            supplierFilterSettings = payload.SupplierFilterSettings;
                            context = payload.context;

                            if (payload.RouteRuleSettings != undefined && payload.RouteRuleSettings.OptionsSettingsGroup != null) {
                                optionSettingsGroupPayload = {
                                    SupplierFilterSettings: supplierFilterSettings,
                                    OptionsSettingsGroup: payload.RouteRuleSettings.OptionsSettingsGroup
                                };
                            }
                        }

                        var loadOptionSettingsGroupTemplatesPromise = WhS_Routing_RoutRuleSettingsAPIService.GetRouteOptionSettingsGroupTemplates().then(function (response) {
                            angular.forEach(response, function (item) {
                                $scope.optionSettingsGroupTemplates.push(item);
                            });

                            if (optionSettingsGroupPayload != undefined) {
                                $scope.selectedOptionSettingsGroupTemplate = UtilsService.getItemByVal($scope.optionSettingsGroupTemplates, optionSettingsGroupPayload.OptionsSettingsGroup.ConfigId, "ExtensionConfigurationId");
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

                    //function loadOptionOrderSettingsGroupSection() {
                    //    var promises = [];
                    //    var optionOrderSettingsGroupPayload;

                    //    if (payload != undefined && payload.RouteRuleSettings != undefined && payload.RouteRuleSettings.OptionOrderSettings != null)
                    //        optionOrderSettingsGroupPayload = payload.RouteRuleSettings.OptionOrderSettings;

                    //    var loadRouteOptionOrderSettingsGroupTemplatesPromise = WhS_Routing_RoutRuleSettingsAPIService.GetRouteOptionOrderSettingsTemplates().then(function (response) {
                    //        angular.forEach(response, function (item) {
                    //            $scope.optionOrderSettingsGroupTemplates.push(item);
                    //        });

                    //        if (optionOrderSettingsGroupPayload != undefined)
                    //            $scope.selectedOptionOrderSettingsGroupTemplate = UtilsService.getItemByVal($scope.optionOrderSettingsGroupTemplates, optionOrderSettingsGroupPayload.ConfigId, "TemplateConfigID");
                    //    });

                    //    promises.push(loadRouteOptionOrderSettingsGroupTemplatesPromise);

                    //    if (optionOrderSettingsGroupPayload != undefined) {
                    //        routeRuleOptionOrderSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

                    //        var routeRuleOptionOrderSettingsLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    //        promises.push(routeRuleOptionOrderSettingsLoadPromiseDeferred.promise);

                    //        routeRuleOptionOrderSettingsReadyPromiseDeferred.promise.then(function () {
                    //            routeRuleOptionOrderSettingsReadyPromiseDeferred = undefined;
                    //            VRUIUtilsService.callDirectiveLoad(routeRuleOptionOrderSettingsDirectiveAPI, optionOrderSettingsGroupPayload, routeRuleOptionOrderSettingsLoadPromiseDeferred);
                    //        });
                    //    }
                    //    return UtilsService.waitMultiplePromises(promises);
                    //}

                    function loadOptionFilterSettingsGroupSection() {

                        var loadOptionFilterSettingsPromiseDeferred = UtilsService.createPromiseDeferred();

                        routeRuleOptionFilterSettingsReadyPromiseDeferred.promise.then(function () {
                            var optionFiltersGroupPayload;

                            if (payload != undefined && payload.RouteRuleSettings != undefined && payload.RouteRuleSettings.OptionFilters != null)
                                optionFiltersGroupPayload = payload.RouteRuleSettings.OptionFilters;

                            VRUIUtilsService.callDirectiveLoad(routeRuleOptionFilterSettingsDirectiveAPI, optionFiltersGroupPayload, loadOptionFilterSettingsPromiseDeferred);
                        });

                        return loadOptionFilterSettingsPromiseDeferred.promise;
                    }

                    function loadOptionOrderTypeGroupSection() {
                        var loadOptionOrderTypeReadyPromiseDeferred = UtilsService.createPromiseDeferred();

                        optionOrderTypeReadyPromiseDeferred.promise.then(function () {

                            var optionOrderPayload = { context: getContext() };
                            if (payload != undefined && payload.RouteRuleSettings != undefined) {
                                optionOrderPayload.optionOrderSettings = payload.RouteRuleSettings.OptionOrderSettings;
                                optionOrderPayload.orderType = payload.RouteRuleSettings.OrderType;
                            }

                            VRUIUtilsService.callDirectiveLoad(optionOrderTypeDirectiveAPI, optionOrderPayload, loadOptionOrderTypeReadyPromiseDeferred);
                        });

                        return loadOptionOrderTypeReadyPromiseDeferred.promise;
                    }


                    function loadOptionPercentageSettingsGroupSection() {
                        var promises = [];
                        var optionPercentageSettingsGroupPayload;

                        if (payload != undefined && payload.RouteRuleSettings != undefined && payload.RouteRuleSettings.OptionPercentageSettings != null)
                            optionPercentageSettingsGroupPayload = payload.RouteRuleSettings.OptionPercentageSettings;
                        var loadRouteOptionPercentageSettingsGroupTemplatesPromise = WhS_Routing_RoutRuleSettingsAPIService.GetRouteOptionPercentageSettingsTemplates().then(function (response) {
                            angular.forEach(response, function (item) {
                                $scope.scopeModel.optionPercentageSettingsGroupTemplates.push(item);
                            });

                            if (optionPercentageSettingsGroupPayload != undefined)
                                $scope.scopeModel.selectedPercentageSettingsGroupTemplate = UtilsService.getItemByVal($scope.scopeModel.optionPercentageSettingsGroupTemplates, optionPercentageSettingsGroupPayload.ConfigId, "ExtensionConfigurationId");
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

                };

                api.getData = function () {

                    var optionOrderTypeDirectiveData = optionOrderTypeDirectiveAPI.getData();
                    var orderType, optionOrderSettings;
                    if (optionOrderTypeDirectiveData != undefined) {
                        orderType = optionOrderTypeDirectiveData.OrderType;
                        optionOrderSettings = optionOrderTypeDirectiveData.OptionOrderSettings;
                    }
                    return {
                        $type: "TOne.WhS.Routing.Business.RegularRouteRule, TOne.WhS.Routing.Business",
                        OptionsSettingsGroup: VRUIUtilsService.getSettingsFromDirective($scope, routeOptionSettingsGroupDirectiveAPI, 'selectedOptionSettingsGroupTemplate'),
                        OrderType: orderType,
                        OptionOrderSettings: optionOrderSettings,
                        OptionFilters: routeRuleOptionFilterSettingsDirectiveAPI.getData(),
                        OptionPercentageSettings: $scope.scopeModel.showPercentageSettings ? VRUIUtilsService.getSettingsFromDirective($scope.scopeModel, routeRuleOptionPercentageSettingsDirectiveAPI, 'selectedPercentageSettingsGroupTemplate') : null
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            function getContext() {
                var currentContext = context != undefined ? context : {};
                currentContext.showPercentageSettings = showPercentageSettings;
                return context;
            }

            function showPercentageSettings(value) {
                $scope.scopeModel.showPercentageSettings = value;
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);