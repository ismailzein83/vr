"use strict";
app.directive("vrMobilenetworkOnbeforesavehandler", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new Handler($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_MobileNetwork/Directives/MobileNetwork/Templates/MobileNetworkOnBeforeSaveHandler.html"
        };

        function Handler($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = {};

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.getData = function () {

                    return {
                        $type: "Vanrise.MobileNetwork.Business.MobileNetworkOnBeforeSaveHandler,Vanrise.MobileNetwork.Business"
                    };
                };

                api.load = function (payload) {
                    if (payload != undefined) { }
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;

    }
]);