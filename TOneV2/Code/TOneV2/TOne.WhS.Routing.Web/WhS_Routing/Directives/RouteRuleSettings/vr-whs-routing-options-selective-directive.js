'use strict';
app.directive('vrWhsRoutingOptionsSelective', ['UtilsService',
    function (UtilsService) {

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
            return '/Client/Modules/WhS_Routing/Directives/RouteRuleSettings/Templates/SelectiveOptionDirectiveTemplate.html';
        }

        function routingSelectiveOptions(ctrl, $scope) {

            var carrierAccountDirectiveAPI;

            function initializeController() {
                $scope.onCarrierAccountDirectiveReady = function (api) {
                    carrierAccountDirectiveAPI = api;
                    declareDirectiveAsReady()
                }

                $scope.removeSupplier = function ($event, supplier) {
                    $event.preventDefault();
                    $event.stopPropagation();
                    var index = UtilsService.getItemIndexByVal($scope.selectedSuppliers, supplier.CarrierAccountId, 'CarrierAccountId');
                    $scope.selectedSuppliers.splice(index, 1);
                };
            }

            function declareDirectiveAsReady() {
                if (carrierAccountDirectiveAPI == undefined)
                    return;

                defineAPI();
            }


            function defineAPI() {
                var api = {};

                api.load = function () {
                    return carrierAccountDirectiveAPI.load();
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

                api.setData = function (routeOptionSettingsGroup) {
                    var supplierIds = [];
                    for (var i = 0; i < routeOptionSettingsGroup.Options.length; i++) {
                        supplierIds.push(routeOptionSettingsGroup.Options[i].SupplierId);
                    }

                    carrierAccountDirectiveAPI.setData(supplierIds);

                    //for (var i = 0; i < $scope.selectedSuppliers.length; i++) {
                    //    $scope.selectedSuppliers[i].percentage = routeOptionSettingsGroup.Options[i].Percentage;
                    //}
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }]);