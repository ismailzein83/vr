"use strict";
app.directive("vrRuntimeRuntimenodeconfigurationScheduler", ["UtilsService", "VRNotificationService", "VRRuntime_RuntimeNodeConfigurationAPIService", "VRRuntime_RuntimeNodeConfigurationService", "VRUIUtilsService", "VRRuntime_RuntimeServiceConfigsAPIService",
function (UtilsService, VRNotificationService, VRRuntime_RuntimeNodeConfigurationAPIService, VRRuntime_RuntimeNodeConfigurationService, VRUIUtilsService, VRRuntime_RuntimeServiceConfigsAPIService) {
    var directiveDefinitionObject =  {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var scheduler = new Scheduler($scope, ctrl, $attrs);
                scheduler.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Runtime/Directives/MainExtensions/Templates/SchedulerServiceTemplate.html"
    };


    function Scheduler($scope, ctrl, $attrs) {
        this.initializeController = initializeController;

        function initializeController() {

            $scope.scopeModel = {};
            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {

                if (payload != undefined) {
                }
            };

            api.getData = function () {

                return {
                    $type: "Vanrise.Runtime.SchedulerService,Vanrise.Runtime",
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
    }
]);