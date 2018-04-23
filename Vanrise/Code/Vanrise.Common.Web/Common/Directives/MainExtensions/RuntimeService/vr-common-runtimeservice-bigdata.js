"use strict";
app.directive("vrCommonRuntimeserviceBigdata", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
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
        templateUrl: "/Client/Modules/Common/Directives/MainExtensions/RuntimeService/Templates/BigDataRuntimeServiceTemplate.html"
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
                console.log(payload);
                if (payload != undefined) {
                }
            };

            api.getData = function () {
                return {
                    $type: "Vanrise.Common.Business.BigDataRuntimeService,Vanrise.Common.Business",
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}
]);