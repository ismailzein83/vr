'use strict';

app.directive('vrWhsRoutingRoutingoptimizersettings', ['UtilsService', 'VRUIUtilsService', 'WhS_Routing_RoutRuleSettingsAPIService',
    function (UtilsService, VRUIUtilsService, WhS_Routing_RoutRuleSettingsAPIService) {

        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new BankDetailsSettingsEditor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/WhS_Routing/Directives/Settings/Templates/RoutingOptimizerSettingsTemplate.html"
        };

        function BankDetailsSettingsEditor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;
            var gridAPI;
            function initializeController() {
                ctrl.datasource = [];
                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var routingOptimizerSettings;
                    if (payload != undefined && payload.data != undefined) {
                        routingOptimizerSettings = payload.data;
                    }
                    var promises = [];
                    WhS_Routing_RoutRuleSettingsAPIService.GetRoutingOptimizerSettingsConfigs().then(function (response) {
                        if (response) {
                            for (var i = 0; i < response.length ; i++) {
                                var item = response[i];
                                var filterItem = {
                                    payload: item,
                                    readyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                    loadPromiseDeferred: UtilsService.createPromiseDeferred()
                                };
                                var payloadData;
                                if (routingOptimizerSettings != undefined && routingOptimizerSettings.Items != undefined) {
                                    payloadData = UtilsService.getItemByVal(routingOptimizerSettings.Items, item.ExtensionConfigurationId, "ConfigId");
                                }
                                promises.push(filterItem.loadPromiseDeferred.promise);
                                addFilterItemToGrid(filterItem, payloadData)
                            }
                        }
                    });
                    return UtilsService.waitMultiplePromises(promises);
                };
                
                api.getData = function () {
                    var items;
                    if (ctrl.datasource != undefined && ctrl.datasource != undefined) {
                        items = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            if (currentItem.directiveAPI != undefined) {
                                var data = currentItem.directiveAPI.getData();
                                if (data != undefined) {
                                    data.ConfigId = currentItem.configId;
                                    items.push(data);
                                }
                            }
                        }
                    }
                    return {
                        $type: "TOne.WhS.Routing.Entities.RoutingOptimizerSettings, TOne.WhS.Routing.Entities",
                        Items: items
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);

            }
            function addFilterItemToGrid(filterItem,payload) {
                var dataItem = {
                    id: ctrl.datasource.length + 1,
                    configId: filterItem.payload.ExtensionConfigurationId,
                    editor: filterItem.payload.Editor,
                    name: filterItem.payload.Title
                };
                var dataItemPayload = payload;

                dataItem.onDirectiveReady = function (api) {
                    dataItem.directiveAPI = api;
                    filterItem.readyPromiseDeferred.resolve();
                };

                filterItem.readyPromiseDeferred.promise
                    .then(function () {
                        VRUIUtilsService.callDirectiveLoad(dataItem.directiveAPI, dataItemPayload, filterItem.loadPromiseDeferred);
                    });
                ctrl.datasource.push(dataItem);
            }
        }
    }]);