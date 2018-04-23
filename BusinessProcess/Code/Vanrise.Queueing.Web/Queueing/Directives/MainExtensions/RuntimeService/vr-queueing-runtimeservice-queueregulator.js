﻿"use strict";
app.directive("vrQueueingRuntimeserviceQueueregulator", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
function (UtilsService, VRNotificationService, VRUIUtilsService) {
    var directiveDefinitionObject = {
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
        templateUrl: "/Client/Modules/Queueing/Directives/MainExtensions/RuntimeService/Templates/QueueRegulatorRuntimeServiceTemplate.html"
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
                    $type: "Vanrise.Queueing.QueueRegulatorRuntimeService,Vanrise.Queueing",
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}
]);