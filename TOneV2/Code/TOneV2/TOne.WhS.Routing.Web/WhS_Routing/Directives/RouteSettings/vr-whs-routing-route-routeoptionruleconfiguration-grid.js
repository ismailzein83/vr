'use strict';

app.directive('vrWhsRoutingRouteRouteoptionruleconfigurationGrid', ['UtilsService', 'VRNotificationService', 'WhS_Routing_RouteOptionRuleAPIService',
    function (UtilsService, VRNotificationService, WhS_Routing_RouteOptionRuleAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                processtype: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var routeOptionRuleconfigurationGrid = new RouteOptionRuleconfigurationGrid($scope, ctrl, $attrs);
                routeOptionRuleconfigurationGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_Routing/Directives/RouteSettings/Templates/RouteOptionRuleConfigurationGridTemplate.html'
        };

        function RouteOptionRuleconfigurationGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.routeOptionRuleSettings = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

            }

            function getRouteOptionRuleSettings(routeOptionRuleTypeConfiguration, routingProcessType) {
                var promise = WhS_Routing_RouteOptionRuleAPIService.GetRouteOptionRuleSettingsTemplatesByProcessType(routingProcessType).then(function (response) {
                    if (response) {
                        for (var index = 0; index < response.length; index++) {
                            var currentItem = response[index];
                            currentItem.IsExcludedDisabled = !currentItem.CanExclude;

                            if (routeOptionRuleTypeConfiguration) {
                                for (var itm in routeOptionRuleTypeConfiguration) {

                                    if (itm == currentItem.ExtensionConfigurationId) {
                                        currentItem.IsExcluded = routeOptionRuleTypeConfiguration[itm].IsExcluded;
                                        break;
                                    }
                                }
                            }
                            $scope.scopeModel.routeOptionRuleSettings.push(currentItem);
                        }
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });

                return promise;
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var routeOptionRuleTypeConfiguration;
                    var routingProcessType;

                    if (payload) {
                        routeOptionRuleTypeConfiguration = payload.routeOptionRuleTypeConfiguration;
                        routingProcessType = payload.routingProcessType;
                    }

                    if (routingProcessType == undefined) {
                        var loadPromiseDeferred = UtilsService.createPromiseDeferred();
                        loadPromiseDeferred.reject();
                        return loadPromiseDeferred.promise;
                    }

                    var promises = [];

                    var routeOptionRuleSettingsPromise = getRouteOptionRuleSettings(routeOptionRuleTypeConfiguration, routingProcessType);
                    promises.push(routeOptionRuleSettingsPromise);


                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    var routeOptionRuleTypeConfiguration = {};
                    for (var index = 0 ; index < $scope.scopeModel.routeOptionRuleSettings.length; index++) {

                        var currentItem = $scope.scopeModel.routeOptionRuleSettings[index];
                        routeOptionRuleTypeConfiguration[currentItem.ExtensionConfigurationId] = {
                            $type: "TOne.WhS.Routing.Entities.RouteOptionRuleTypeConfiguration, TOne.WhS.Routing.Entities",
                            IsExcluded: currentItem.IsExcluded
                        };
                    }

                    return routeOptionRuleTypeConfiguration;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }


        }
    }]);
