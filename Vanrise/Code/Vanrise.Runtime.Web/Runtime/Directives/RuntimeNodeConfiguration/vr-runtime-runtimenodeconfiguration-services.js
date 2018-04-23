
"use strict";

app.directive("vrRuntimeRuntimenodeconfigurationServices", ["UtilsService", "VRNotificationService", "VRRuntime_RuntimeNodeConfigurationAPIService", "VRRuntime_RuntimeNodeConfigurationService", "VRUIUtilsService",
function (UtilsService, VRNotificationService, VRRuntime_RuntimeNodeConfigurationAPIService, VRRuntime_RuntimeNodeConfigurationService, VRUIUtilsService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
            enableadd: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var vrRuntimeNodeConfiguration = new VRRuntimeNodeConfiguration($scope, ctrl);
            vrRuntimeNodeConfiguration.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {
        },
        templateUrl: "/Client/Modules/Runtime/Directives/RuntimeNodeConfiguration/Templates/RuntimeNodeConfigurationServices.html"
    };
    function VRRuntimeNodeConfiguration($scope, ctrl) {
        this.initializeController = initializeController;


        function initializeController() {
            ctrl.datasource = [];

            ctrl.isValid = function () {
                if (ctrl.datasource.length > 0)
                    return null;
                return "You Should Add at least one Service.";
            };

            ctrl.onAddRuntimeNodeConfiguration = function () {
                var onRuntimeNodeConfigurationAdded = function (addedRuntimeNodeConfiguration) {
                    ctrl.datasource.push({ Entity: addedRuntimeNodeConfiguration });
                };
                VRRuntime_RuntimeNodeConfigurationService.addRuntimeNodeConfigurationServices(onRuntimeNodeConfigurationAdded, ctrl.datasource);
            };
            ctrl.onRemoveRuntimeNodeConfiguration = function (runtimeNodeConfiguration) {
                var index = UtilsService.getItemIndexByVal(ctrl.datasource, runtimeNodeConfiguration.Entity.RuntimeServiceConfigurationId, 'Entity.RuntimeServiceConfigurationId');
                ctrl.datasource.splice(index, 1);
            };
            defineMenuActions();
            defineAPI();
        }

        function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var runtimeNodeConfigurationSettingsPayload;
                    if (payload != undefined && payload.services != undefined) {
                        runtimeNodeConfigurationSettingsPayload = payload.services;
                    }
                    if (runtimeNodeConfigurationSettingsPayload != undefined) {
                        for (var key in runtimeNodeConfigurationSettingsPayload) {
                            if (key != '$type') {
                                var item = runtimeNodeConfigurationSettingsPayload[key];
                                item.RuntimeServiceConfigurationId = key;
                                ctrl.datasource.push({ Entity: item });
                            }
                        }
                    }
                };

                api.getData = function () {
                    var runtimeNodesConfigurations;
                    if (ctrl.datasource != undefined) {
                        runtimeNodesConfigurations = {};
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            runtimeNodesConfigurations[currentItem.Entity.RuntimeServiceConfigurationId] = currentItem.Entity;
                        }
                    }
                    return runtimeNodesConfigurations;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
        }

        function defineMenuActions() {
            ctrl.gridMenuActions = [{
                name: "Edit",
                clicked: editRuntimeNodeConfiguration,
            }];
        }

        function editRuntimeNodeConfiguration(runtimeNodeConfigurationObj) {
            var onRuntimeNodeConfigurationUpdated = function (runtimeNodeConfiguration) {
                var index = ctrl.datasource.indexOf(runtimeNodeConfigurationObj);
                ctrl.datasource[index] = { Entity: runtimeNodeConfiguration };
            };
            VRRuntime_RuntimeNodeConfigurationService.editRuntimeNodeConfigurationServices(runtimeNodeConfigurationObj.Entity, onRuntimeNodeConfigurationUpdated);
        }
    }
}]);