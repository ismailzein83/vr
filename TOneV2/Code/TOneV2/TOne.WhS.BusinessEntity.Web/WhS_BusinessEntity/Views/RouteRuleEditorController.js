(function (appControllers) {

    "use strict";

    routeRuleEditorController.$inject = ['$scope', 'WhS_BE_RouteRuleAPIService', 'WhS_BE_RoutingProductAPIService', 'WhS_BE_SaleZoneAPIService',
        'UtilsService', 'VRNotificationService', 'VRNavigationService'];

    function routeRuleEditorController($scope, WhS_BE_RouteRuleAPIService, WhS_BE_RoutingProductAPIService, WhS_BE_SaleZoneAPIService,
        UtilsService, VRNotificationService, VRNavigationService) {

            var editMode;
            var routeRuleId;

            var saleZoneGroupSettingsDirectiveAPI;
            var saleZoneGroupSettings;

            loadParameters();
            defineScope();
            load();

            function loadParameters() {
                var parameters = VRNavigationService.getParameters($scope);

                if (parameters != undefined && parameters != null) {
                    routeRuleId = parameters.routeRuleId;
                }
                editMode = (routeRuleId != undefined);
            }

            function defineScope() {
                $scope.onSaleZoneGroupSettingsDirectiveLoaded = function (api) {
                    saleZoneGroupSettingsDirectiveAPI = api;

                    if (saleZoneGroupSettings != undefined)
                    {
                        saleZoneGroupSettingsDirectiveAPI.setData(saleZoneGroupSettings);
                        saleZoneGroupSettings = undefined;
                    }
                }

                $scope.SaveRouteRule = function () {
                    if (editMode) {
                        return updateRouteRule();
                    }
                    else {
                        return insertRouteRule();
                    }
                };

                $scope.close = function () {
                    $scope.modalContext.closeModal()
                };

                $scope.routingProductsOnSelectionChanged = function () {
                    if ($scope.selectedSaleZonePackage != undefined) {
                        //Hide the section of customer and code
                    }
                }

                $scope.saleZoneGroupTemplates = [];
                $scope.selectedSaleZoneGroupTemplate = undefined;

                $scope.routingProducts = [];
                $scope.selectedRoutingProduct = undefined;

                $scope.saleZoneGroupSettings = {};
            }

            function load() {
                $scope.isGettingData = true;
                return UtilsService.waitMultipleAsyncOperations([loadRoutingProducts, loadSaleZoneGroupTemplates]).then(function () {
                    if (editMode) {
                        getRouteRule();
                    }
                    else {
                        $scope.isGettingData = false;
                    }

                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.isGettingData = false;
                });
            }

            function getRouteRule() {
                return WhS_BE_RouteRuleAPIService.GetRouteRule(routeRuleId).then(function (routeRule) {
                    fillScopeFromRouteRuleObj(routeRule);

                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                }).finally(function () {
                    $scope.isGettingData = false;
                });
            }

            function loadRoutingProducts() {
                return WhS_BE_RoutingProductAPIService.GetRoutingProducts().then(function (response) {
                    angular.forEach(response, function (item) {
                        $scope.routingProducts.push(item);
                    });
                });
            }

            function loadSaleZoneGroupTemplates() {
                return WhS_BE_SaleZoneAPIService.GetSaleZoneGroupTemplates().then(function (response) {

                    var defSaleZoneSelection = { TemplateConfigID: -1, Name: 'No Filter', TemplateURL: '' };
                    $scope.saleZoneGroupTemplates.push(defSaleZoneSelection);

                    angular.forEach(response, function (item) {
                        $scope.saleZoneGroupTemplates.push(item);
                    });

                    $scope.selectedSaleZoneGroupTemplate = defSaleZoneSelection;
                });
            }

            function buildRouteRuleObjFromScope() {
                var routeRule = {
                    RouteRuleId: (routeRuleId != null) ? routeRuleId : 0,
                    RouteCriteria: {
                        RoutingProductId: $scope.selectedRoutingProduct.RoutingProductId,
                        SaleZoneGroupConfigId: $scope.selectedSaleZoneGroupTemplate.TemplateConfigID != -1 ? $scope.selectedSaleZoneGroupTemplate.TemplateConfigID : null,
                        SaleZoneGroupSettings: saleZoneGroupSettingsDirectiveAPI.getData()
                    }
                };

                return routeRule;
            }

            function fillScopeFromRouteRuleObj(routeRuleObj) {

                if (routeRuleObj.RouteCriteria.RoutingProductId != null)
                    $scope.selectedRoutingProduct = UtilsService.getItemByVal($scope.routingProducts, routeRuleObj.RouteCriteria.RoutingProductId, "RoutingProductId");

                if (routeRuleObj.RouteCriteria.SaleZoneGroupConfigId != null)
                {
                    $scope.selectedSaleZoneGroupTemplate = UtilsService.getItemByVal($scope.saleZoneGroupTemplates, routeRuleObj.RouteCriteria.SaleZoneGroupConfigId, "TemplateConfigID");
                    saleZoneGroupSettings = routeRuleObj.RouteCriteria.SaleZoneGroupSettings;
                }
            }

            function insertRouteRule() {
                var routeRuleObject = buildRouteRuleObjFromScope();
                return WhS_BE_RouteRuleAPIService.AddRouteRule(routeRuleObject)
                .then(function (response) {
                    if (VRNotificationService.notifyOnItemAdded("Route Rule", response)) {
                        if ($scope.onRouteRuleAdded != undefined)
                            $scope.onRouteRuleAdded(response.InsertedObject);
                        $scope.modalContext.closeModal();
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });

            }

            function updateRouteRule() {
                var routeRuleObject = buildRouteRuleObjFromScope();
                WhS_BE_RouteRuleAPIService.UpdateRouteRule(routeRuleObject)
                .then(function (response) {
                    if (VRNotificationService.notifyOnItemUpdated("Route Rule", response)) {
                        if ($scope.onRouteRuleUpdated != undefined)
                            $scope.onRouteRuleUpdated(response.UpdatedObject);
                        $scope.modalContext.closeModal();
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
            }

    }

    appControllers.controller('WhS_BE_RouteRuleEditorController', routeRuleEditorController);
})(appControllers);
