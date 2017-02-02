(function (app) {

    'use strict';

    ProvisionerDefinitionsettingsDirective.$inject = ["UtilsService", 'VRUIUtilsService'];

    function ProvisionerDefinitionsettingsDirective(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ProvisionerDefinitionsettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_Teles/Directives/AccountAction/Provisioning/ChangeUsersRGs/Templates/ChangeUsersRGsProvisionerDefinitionSettingsTemplate.html"

        };
        function ProvisionerDefinitionsettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var mainPayload;

            var newRoutingGroupConditionAPI;
            var newRoutingGroupConditionReadyDeferred = UtilsService.createPromiseDeferred();

            var existingRoutingGroupConditionAPI;
            var existingRoutingGroupConditionReadyDeferred = UtilsService.createPromiseDeferred();
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.onNewRoutingGroupConditionReady = function (api) {
                    newRoutingGroupConditionAPI = api;
                    newRoutingGroupConditionReadyDeferred.resolve();
                };
                $scope.scopeModel.onExistingRoutingGroupConditionReady = function (api) {
                    existingRoutingGroupConditionAPI = api;
                    existingRoutingGroupConditionReadyDeferred.resolve();
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var provisionerDefinitionSettings;
                    if (payload != undefined) {
                        mainPayload = payload;
                        provisionerDefinitionSettings = payload.provisionerDefinitionSettings;
                        if(provisionerDefinitionSettings != undefined)
                        {
                            $scope.scopeModel.saveChangesToAccountState = provisionerDefinitionSettings.SaveChangesToAccountState;
                            $scope.scopeModel.actionType = provisionerDefinitionSettings.ActionType;
                        }

                    }

                    var promises = [];

                    function loadNewRoutingGroupCondition() {
                        var newRoutingGroupConditionLoadDeferred = UtilsService.createPromiseDeferred();

                        newRoutingGroupConditionReadyDeferred.promise.then(function () {
                            var newRoutingGroupConditionPayload;
                            if (provisionerDefinitionSettings != undefined) {
                                newRoutingGroupConditionPayload = { routingGroupCondition: provisionerDefinitionSettings.NewRoutingGroupCondition };
                            }
                            VRUIUtilsService.callDirectiveLoad(newRoutingGroupConditionAPI, newRoutingGroupConditionPayload, newRoutingGroupConditionLoadDeferred);
                        });
                        return newRoutingGroupConditionLoadDeferred.promise
                    }
                    function loadExistingRoutingGroupCondition() {
                        var existingRoutingGroupConditionLoadDeferred = UtilsService.createPromiseDeferred();

                        existingRoutingGroupConditionReadyDeferred.promise.then(function () {
                            var existingRoutingGroupConditionPayload;
                            if (provisionerDefinitionSettings != undefined) {
                                existingRoutingGroupConditionPayload = { routingGroupCondition: provisionerDefinitionSettings.ExistingRoutingGroupCondition };
                            }
                            VRUIUtilsService.callDirectiveLoad(existingRoutingGroupConditionAPI, existingRoutingGroupConditionPayload, existingRoutingGroupConditionLoadDeferred);
                        });
                        return existingRoutingGroupConditionLoadDeferred.promise
                    }

                    promises.push(loadNewRoutingGroupCondition());
                    promises.push(loadExistingRoutingGroupCondition());

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var data = {
                        $type: "Retail.Teles.Business.ChangeUsersRGsDefinitionSettings,Retail.Teles.Business",
                        SaveChangesToAccountState:$scope.scopeModel.saveChangesToAccountState,
                        ActionType: $scope.scopeModel.saveChangesToAccountState?$scope.scopeModel.actionType:undefined,
                        NewRoutingGroupCondition: newRoutingGroupConditionAPI.getData(),
                        ExistingRoutingGroupCondition: existingRoutingGroupConditionAPI.getData(),
                    };
                    return data;
                }
            }
        }
    }

    app.directive('retailTelesProvisionerDefinitionsettingsChangeusersrgs', ProvisionerDefinitionsettingsDirective);

})(app);