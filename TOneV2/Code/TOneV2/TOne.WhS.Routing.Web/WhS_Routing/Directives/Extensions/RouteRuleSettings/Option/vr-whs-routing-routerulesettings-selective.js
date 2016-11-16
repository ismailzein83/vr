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

                var ctor = new selectiveOptionCtor(ctrl, $scope);
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
                return '/Client/Modules/WhS_Routing/Directives/Extensions/RouteRuleSettings/Option/Templates/SelectiveOptionDirective.html';
            }

        };

        function selectiveOptionCtor(ctrl, $scope) {

            var carrierAccountDirectiveAPI;
            var carrierAccountReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                ctrl.onCarrierAccountDirectiveReady = function (api) {
                    carrierAccountDirectiveAPI = api;
                    carrierAccountReadyPromiseDeferred.resolve();
                };

                ctrl.removeFilter = function (dataItem) {
                    var index = UtilsService.getItemIndexByVal(ctrl.selectedSuppliers, dataItem.CarrierAccountId, 'CarrierAccountId');
                    ctrl.selectedSuppliers.splice(index, 1);
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var loadCarrierAccountPromiseDeferred = UtilsService.createPromiseDeferred();

                    carrierAccountReadyPromiseDeferred.promise.then(function () {
                        var carrierAccountPayload = {
                            filter: { SupplierFilterSettings: payload != undefined ? payload.SupplierFilterSettings : undefined },
                            selectedIds: []
                        };
                        if (payload != undefined && payload.OptionsSettingsGroup != undefined) {
                            for (var i = 0; i < payload.OptionsSettingsGroup.Options.length; i++) {
                                carrierAccountPayload.selectedIds.push(payload.OptionsSettingsGroup.Options[i].SupplierId);
                            }
                        }

                        VRUIUtilsService.callDirectiveLoad(carrierAccountDirectiveAPI, carrierAccountPayload, loadCarrierAccountPromiseDeferred);
                    });

                    return loadCarrierAccountPromiseDeferred.promise;
                };

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.Routing.Business.RouteRules.OptionSettingsGroups.SelectiveOptions, TOne.WhS.Routing.Business",
                        Options: getOptions()
                    };

                    function getOptions() {
                        var options = [];
                        for (var i = 0; i < ctrl.selectedSuppliers.length; i++) {
                            options.push({
                                SupplierId: ctrl.selectedSuppliers[i].CarrierAccountId,
                                Percentage: null,
                                Filter: null
                            });
                        }

                        return options;
                    }
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }]);