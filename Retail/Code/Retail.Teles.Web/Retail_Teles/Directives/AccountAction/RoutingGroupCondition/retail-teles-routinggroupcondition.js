(function (app) {

    'use strict';

    TelesRoutingGroupConditioneDirective.$inject = ['Retail_Teles_TelesRoutingGroupAPIService', 'UtilsService', 'VRUIUtilsService'];

    function TelesRoutingGroupConditioneDirective(Retail_Teles_TelesRoutingGroupAPIService, UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                label: '@',
                customvalidate: '=',
                isrequired:'='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new RoutingGroupCondition($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_Teles/Directives/AccountAction/RoutingGroupCondition/Templates/RoutingGroupConditionTemplate.html"
        };

        function RoutingGroupCondition($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var accountBEDefinitionId;

            var selectorAPI;

            var directiveAPI;
            var directiveReadyDeferred;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.templateConfigs = [];
                $scope.scopeModel.selectedTemplateConfig;

                $scope.scopeModel.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    var directivePayload = {
                        accountBEDefinitionId: accountBEDefinitionId
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
                };
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    selectorAPI.clearDataSource();

                    var promises = [];
                    var routingGroupCondition;

                    if (payload != undefined) {
                        accountBEDefinitionId = payload.accountBEDefinitionId;
                        routingGroupCondition = payload.routingGroupCondition;
                    }

                    var getRoutingGroupConditionConfigsPromise = getRoutingGroupConditionConfigs();
                    promises.push(getRoutingGroupConditionConfigsPromise);

                    if (routingGroupCondition != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }


                    function getRoutingGroupConditionConfigs() {
                        return Retail_Teles_TelesRoutingGroupAPIService.GetRoutingGroupConditionConfigs().then(function (response) {
                            if (response != undefined) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }
                                if (routingGroupCondition != undefined) {
                                    $scope.scopeModel.selectedTemplateConfig =
                                        UtilsService.getItemByVal($scope.scopeModel.templateConfigs, routingGroupCondition.ConfigId, 'ExtensionConfigurationId');
                                }
                            }
                        });
                    }
                    function loadDirective() {
                        directiveReadyDeferred = UtilsService.createPromiseDeferred();

                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                        directiveReadyDeferred.promise.then(function () {
                            directiveReadyDeferred = undefined;
                            var directivePayload = {
                                accountBEDefinitionId: accountBEDefinitionId,
                                routingGroupCondition: routingGroupCondition
                            };
                            VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
                        });

                        return directiveLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data;

                    if ($scope.scopeModel.selectedTemplateConfig != undefined && directiveAPI != undefined) {

                        data = directiveAPI.getData();
                        if (data != undefined) {
                            data.ConfigId = $scope.scopeModel.selectedTemplateConfig.ExtensionConfigurationId;
                        }
                    }
                    return data;
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('retailTelesRoutinggroupcondition', TelesRoutingGroupConditioneDirective);

})(app);