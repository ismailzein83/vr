'use strict';
app.directive('vrWhsRoutingRouterulesettingsSelective', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var routingSelectiveOptionsCtor = new routingSelectiveOptions(ctrl, $scope);
                routingSelectiveOptionsCtor.initializeController();

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
                return getRoutingSelectiveOptionsTemplate(attrs);
            }

        };

        function getRoutingSelectiveOptionsTemplate(attrs) {
            return '/Client/Modules/WhS_Routing/Directives/Extensions/RouteRuleSettings/Templates/SelectiveOptionDirective.html';
        }

        function routingSelectiveOptions(ctrl, $scope) {

            var carrierAccountDirectiveAPI;
            var carrierAccountReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.onCarrierAccountDirectiveReady = function (api) {
                    carrierAccountDirectiveAPI = api;
                    carrierAccountReadyPromiseDeferred.resolve();
                }

                $scope.removeSupplier = function ($event, supplier) {
                    $event.preventDefault();
                    $event.stopPropagation();
                    var index = UtilsService.getItemIndexByVal($scope.selectedSuppliers, supplier.CarrierAccountId, 'CarrierAccountId');
                    $scope.selectedSuppliers.splice(index, 1);
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (routeOptionSettingsGroup) {

                    var loadCarrierAccountPromiseDeferred = UtilsService.createPromiseDeferred();

                    carrierAccountReadyPromiseDeferred.promise.then(function () {
                        var directivePayload = [];
                        if (routeOptionSettingsGroup != undefined)
                        {
                            var supplierIds = [];
                            for (var i = 0; i < routeOptionSettingsGroup.Options.length; i++) {
                                directivePayload.push(routeOptionSettingsGroup.Options[i].SupplierId);
                            }
                        }
                            
                        VRUIUtilsService.callDirectiveLoad(carrierAccountDirectiveAPI, directivePayload, loadCarrierAccountPromiseDeferred);
                    });

                    return loadCarrierAccountPromiseDeferred.promise;
                }

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.Routing.Business.RouteRules.OptionSettingsGroups.SelectiveOptions, TOne.WhS.Routing.Business",
                        Options: getOptions()
                    };

                    function getOptions() {
                        var options = [];
                        for (var i = 0; i < $scope.selectedSuppliers.length; i++) {
                            options.push({
                                SupplierId: $scope.selectedSuppliers[i].CarrierAccountId,
                                Percentage: null,
                                Filter: null
                            });
                        }

                        return options;
                    }
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }]);