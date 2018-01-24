﻿'use strict';

app.directive('vrWhsRoutingRouterulecriteriaDefault', ['UtilsService', 'VRUIUtilsService', 'WhS_Routing_RouteRuleAPIService', 'WhS_Routing_RouteRuleCriteriaTypeEnum',
    function (UtilsService, VRUIUtilsService, WhS_Routing_RouteRuleAPIService, WhS_Routing_RouteRuleCriteriaTypeEnum) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new RouteRuleCriteriaCtor(ctrl, $scope);
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
                return '/Client/Modules/WhS_Routing/Directives/Extensions/RouteRuleCriteria/Templates/DefaultRouteRuleCriteriaDirective.html';
            }
        };

        function RouteRuleCriteriaCtor(ctrl, $scope) {
            this.initializeController = initializeController;

            var routingProductId;
            var routeRuleCriteria;
            var sellingNumberPlanId;
            var linkedCode;
            var isLinkedRouteRule;

            var saleZoneGroupSettingsAPI;
            var saleZoneGroupSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var codeCriteriaGroupSettingsAPI;
            var codeCriteriaGroupSettingsReadyPromiseDeferred;

            var customerGroupSettingsAPI;
            var customerGroupSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var excludedDestinationsDirectiveAPI;
            var excludedDestinationsDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();


            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.saleZoneGroupTemplates = [];
                $scope.scopeModel.codeCriteriaGroupTemplates = [];
                $scope.scopeModel.customerGroupTemplates = [];

                $scope.scopeModel.onSaleZoneGroupSettingsDirectiveReady = function (api) {
                    saleZoneGroupSettingsAPI = api;
                    var saleZoneGroupPayload = {
                        sellingNumberPlanId: sellingNumberPlanId != undefined ? sellingNumberPlanId : undefined,
                        saleZoneFilterSettings: { RoutingProductId: routingProductId },
                    };
                    var setLoader = function (value) {
                        setTimeout(function () { $scope.scopeModel.isLoadingSaleZoneGroup = value; UtilsService.safeApply($scope); });
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, saleZoneGroupSettingsAPI, saleZoneGroupPayload, setLoader, saleZoneGroupSettingsReadyPromiseDeferred);
                };
                $scope.scopeModel.onCodeCriteriaGroupSettingsDirectiveReady = function (api) {
                    codeCriteriaGroupSettingsAPI = api;
                    var setLoader = function (value) { $scope.scopeModel.isLoadingCustomersSection = value; };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, codeCriteriaGroupSettingsAPI, undefined, setLoader, codeCriteriaGroupSettingsReadyPromiseDeferred);
                };
                $scope.scopeModel.onCustomerGroupSettingsDirectiveReady = function (api) {
                    customerGroupSettingsAPI = api;
                    customerGroupSettingsReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onExcludedDestinationsDirectiveReady = function (api) {
                    excludedDestinationsDirectiveAPI = api;
                    excludedDestinationsDirectiveReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onRouteRuleCriteriaTypeSelectionChanged = function () {

                    if ($scope.scopeModel.selectedRouteRuleCriteriaType == undefined)
                        return;

                    if ($scope.scopeModel.selectedRouteRuleCriteriaType == WhS_Routing_RouteRuleCriteriaTypeEnum.SaleZone) {
                        $scope.scopeModel.showSaleZoneSection = $scope.scopeModel.showCustomerSection = $scope.scopeModel.showExcludedCodeSection = true;
                        $scope.scopeModel.showIncludedCodeSection = false;

                    }
                    else {
                        $scope.scopeModel.selectedCodeCriteriaGroupTemplate = undefined;
                        $scope.scopeModel.showIncludedCodeSection = $scope.scopeModel.showCustomerSection = $scope.scopeModel.showExcludedCodeSection = true;
                        $scope.scopeModel.showSaleZoneSection = false;
                    }
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    routingProductId = payload.routingProductId;
                    sellingNumberPlanId = payload.sellingNumberPlanId;
                    routeRuleCriteria = payload.routeRuleCriteria;
                    linkedCode = payload.linkedCode;
                    isLinkedRouteRule = payload.isLinkedRouteRule;

                    $scope.scopeModel.disableCriteria = isLinkedRouteRule;

                    displaySectionsBasedOnParameters();
                    loadFilterBySection();

                    var loadSaleZoneGroupSectionPromise = loadSaleZoneGroupSection();
                    if (loadSaleZoneGroupSectionPromise != undefined)
                        promises.push(loadSaleZoneGroupSectionPromise);

                    var loadCodeCriteriaGroupSectionPromise = loadCodeCriteriaGroupSection();
                    promises.push(loadCodeCriteriaGroupSectionPromise);

                    var loadCustomerGroupSectionPromise = loadCustomerGroupSection();
                    promises.push(loadCustomerGroupSectionPromise);

                    var loadExcludedDestinationsDirectivePromise = loadExcludedDestinationsDirective();
                    promises.push(loadExcludedDestinationsDirectivePromise);

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    return {
                        $type: "TOne.WhS.Routing.Entities.RouteRuleCriteria, TOne.WhS.Routing.Entities",
                        RoutingProductId: routingProductId,
                        ExcludedDestinations: excludedDestinationsDirectiveAPI.getData(),
                        SaleZoneGroupSettings: $scope.scopeModel.showSaleZoneSection ? saleZoneGroupSettingsAPI.getData() : undefined, //VRUIUtilsService.getSettingsFromDirective($scope, saleZoneGroupSettingsAPI, 'selectedSaleZoneGroupTemplate'),
                        CustomerGroupSettings: customerGroupSettingsAPI.getData(),
                        CodeCriteriaGroupSettings: $scope.scopeModel.showIncludedCodeSection ? VRUIUtilsService.getSettingsFromDirective($scope.scopeModel, codeCriteriaGroupSettingsAPI, 'selectedCodeCriteriaGroupTemplate') : undefined
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function displaySectionsBasedOnParameters() {
                $scope.scopeModel.showSaleZoneSection = routingProductId != undefined;
                $scope.scopeModel.showRouteRuleTypeFilterSection = $scope.scopeModel.showCustomerSection = $scope.scopeModel.showExcludedCodeSection = $scope.scopeModel.showIncludedCodeSection = !$scope.scopeModel.showSaleZoneSection;
            }

            function loadFilterBySection() {
                if (!$scope.scopeModel.showRouteRuleTypeFilterSection)
                    return;

                $scope.scopeModel.routeRuleCriteriaTypes = UtilsService.getArrayEnum(WhS_Routing_RouteRuleCriteriaTypeEnum);
                if (routeRuleCriteria != undefined && routeRuleCriteria.CodeCriteriaGroupSettings != undefined)
                    $scope.scopeModel.selectedRouteRuleCriteriaType = UtilsService.getEnum(WhS_Routing_RouteRuleCriteriaTypeEnum, 'value', WhS_Routing_RouteRuleCriteriaTypeEnum.Code.value);
                else
                    $scope.scopeModel.selectedRouteRuleCriteriaType = UtilsService.getEnum(WhS_Routing_RouteRuleCriteriaTypeEnum, 'value', WhS_Routing_RouteRuleCriteriaTypeEnum.SaleZone.value);
            }

            function loadSaleZoneGroupSection() {

                if (routeRuleCriteria == undefined || routeRuleCriteria.SaleZoneGroupSettings == undefined) {
                    saleZoneGroupSettingsReadyPromiseDeferred = undefined;
                    return;
                }

                var promises = [];

                if (saleZoneGroupSettingsReadyPromiseDeferred == undefined)
                    saleZoneGroupSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

                var saleZoneGroupSettingsLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                promises.push(saleZoneGroupSettingsLoadPromiseDeferred.promise);

                saleZoneGroupSettingsReadyPromiseDeferred.promise.then(function () {
                    var saleZoneGroupPayload = {
                        sellingNumberPlanId: sellingNumberPlanId != undefined ? sellingNumberPlanId : undefined,
                        saleZoneFilterSettings: { RoutingProductId: routingProductId },
                    };

                    if (routeRuleCriteria != undefined) {
                        saleZoneGroupPayload.sellingNumberPlanId = sellingNumberPlanId != undefined ? sellingNumberPlanId : routeRuleCriteria.SaleZoneGroupSettings != undefined ? routeRuleCriteria.SaleZoneGroupSettings.SellingNumberPlanId : undefined;
                        saleZoneGroupPayload.saleZoneGroupSettings = routeRuleCriteria.SaleZoneGroupSettings;
                    }
                    saleZoneGroupSettingsReadyPromiseDeferred = undefined;

                    VRUIUtilsService.callDirectiveLoad(saleZoneGroupSettingsAPI, saleZoneGroupPayload, saleZoneGroupSettingsLoadPromiseDeferred);
                });

                return UtilsService.waitMultiplePromises(promises);
            }

            function loadCodeCriteriaGroupSection() {
                var promises = [];
                var codeCriteriaGroupPayload;

                if (routeRuleCriteria != undefined && routeRuleCriteria.CodeCriteriaGroupSettings != undefined)
                    codeCriteriaGroupPayload = routeRuleCriteria.CodeCriteriaGroupSettings;

                var loadCodeCriteriaGroupTemplatesPromise = WhS_Routing_RouteRuleAPIService.GetCodeCriteriaGroupTemplates().then(function (response) {
                    angular.forEach(response, function (item) {
                        $scope.scopeModel.codeCriteriaGroupTemplates.push(item);
                    });

                    if (codeCriteriaGroupPayload != undefined)
                        $scope.scopeModel.selectedCodeCriteriaGroupTemplate = UtilsService.getItemByVal($scope.scopeModel.codeCriteriaGroupTemplates, codeCriteriaGroupPayload.ConfigId, "ExtensionConfigurationId");
                });

                promises.push(loadCodeCriteriaGroupTemplatesPromise);

                if (codeCriteriaGroupPayload != undefined) {
                    codeCriteriaGroupSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

                    var codeCriteriaGroupSettingsLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    promises.push(codeCriteriaGroupSettingsLoadPromiseDeferred.promise);

                    codeCriteriaGroupSettingsReadyPromiseDeferred.promise.then(function () {
                        codeCriteriaGroupSettingsReadyPromiseDeferred = undefined;
                        VRUIUtilsService.callDirectiveLoad(codeCriteriaGroupSettingsAPI, codeCriteriaGroupPayload, codeCriteriaGroupSettingsLoadPromiseDeferred);
                    });
                }

                return UtilsService.waitMultiplePromises(promises);
            }

            function loadCustomerGroupSection() {
                var promises = [];
                if (customerGroupSettingsReadyPromiseDeferred == undefined)
                    customerGroupSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

                var customerGroupSettingsLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                promises.push(customerGroupSettingsLoadPromiseDeferred.promise);

                customerGroupSettingsReadyPromiseDeferred.promise.then(function () {
                    var customerGroupPayload;
                    if (routeRuleCriteria != undefined && routeRuleCriteria.CustomerGroupSettings != null)
                        customerGroupPayload = routeRuleCriteria.CustomerGroupSettings;

                    customerGroupSettingsReadyPromiseDeferred = undefined;
                    VRUIUtilsService.callDirectiveLoad(customerGroupSettingsAPI, customerGroupPayload, customerGroupSettingsLoadPromiseDeferred);
                });

                return UtilsService.waitMultiplePromises(promises);
            }

            function loadExcludedDestinationsDirective() {
                var loadExcludedDestinationsDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

                excludedDestinationsDirectiveReadyPromiseDeferred.promise.then(function () {

                    var excludedDestinationsDirectivePayload = {
                        excludedDestinations: routeRuleCriteria != undefined ? routeRuleCriteria.ExcludedDestinations : undefined,
                        isLinkedRouteRule: isLinkedRouteRule,
                        linkedCode: linkedCode
                    };
                    VRUIUtilsService.callDirectiveLoad(excludedDestinationsDirectiveAPI, excludedDestinationsDirectivePayload, loadExcludedDestinationsDirectivePromiseDeferred);
                });

                return loadExcludedDestinationsDirectivePromiseDeferred.promise;
            }
        }

        return directiveDefinitionObject;
    }]);